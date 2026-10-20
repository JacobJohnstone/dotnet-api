# Repositories

## Notes

---

#### Query formatting
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Majority of queries will be formatted as 
{dbContext}.{DbSet}.{operation**Async**(LINQ)}, or a series of chained SQL
operations for example: 
```
// Where the context is injected, and Users is a DbSet:
var activeUsers = await _context.Users.Where(u => u.IsActive)
    .OrderBy(u => u.LastName)
    .FirstOrDefaultAsync();
```

---

#### Asynchronous Methods
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Comparing to front-end development, just like we have to 
await the response when fetching data from an API endpoint, in the backend, we have to await
the results of our queries. While we use the await/async keywords still, we no longer call
these asynchronous methods **Promises** and instead call them **Tasks**, but they operate
the same way. A return type of `Promise<User>` for an asynchronous function returning 
a User type, is equivalently a `Task<User>` return type. If no data is to be returned,
for example in a delete operation, the return type would simply be `Task` (equivalent
to `Promise<void>`). For example:
```
// A function that queries the DB for a specific user based on ID and returns the user
public async Task<User> getUserById(Guid userId) {}

// A function that returns all users
public asyync Task<List<User>> getUsers() {}

```

---

#### Saving Data: Nuance to Entity Framework

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; When processing database operations, we have to remember there
are two different mental models at play. The first is remembering that entity framework (EF) operates
using a DbContext, with in memory DbSets, etc. so when changes within EF are made, they're all
local and temporary. It is only when we **SAVE** our changes that they are persisted to the actual
DB instance. In order to do so we have to use something like **saveChangesAsync()**:
```
// submit user to the database ONLY EF TRACKING
await _context.Users.AddAsync(newUser);

// REQUIRED: push your tracked changes to the DB
await _context.SaveChangesAsync();
```

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Additionally, updating entities has its own workflow. Since entity tracks
object changes in memory, when fetching entities from the db it tracks them to watch for changes.
In this scenario, we have to explicitly save the changes to the db when we are done out updates, using 
something like `_context.SaveChangesAsync()`. Sometimes though, we might need a reference from the db, but
not want to make any changes to it, for example a read-only operation. In this case, we don't need EF to 
be tracking our object, as we don't plan on making any changes. Here we can increase performance by explicitly
telling EF to not listen to out changes using the `.AsNoTracking()` method. For example:

```
// USER LOGIN //

// Check if a user exists with this email (and only one instance exists)
var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);

// ... handle errors ...
// ... verify password ...

// Update "last logged in" flag
user.LastLogin = DateTime.UtcNow;

// Persist login timestamp update
try
{
    _context.SaveChangesAsync();
}
catch
{
    _logger.LogError($"Failed to update user's ({user.Email}) last logged in timeout.");
}

return user;
```

Above we see an instance where we make a change to the user timestamp directly on the object
(no `.Update()` method or anything like that), and then persist the change by calling
`.SaveChangesAsync()`. Now, an example where we don't care to track the user we fetch:

```
// REGISTER NEW USER

// use asNoTracking as this is a readOnly operation
var alreadyExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Email == user.Email);

// ... "email already exists" error ...
// ... hash password ...

// instantiate new user object
var newUser = new User
{
    Id = Guid.NewGuid(),
    FirstName = user.FirstName,
    LastName = user.LastName,
    Email = user.Email,
    ProfilePictureUrl = user.ProfilePictureUrl,
    Password =  passwordHash,
};

// submit user to the database (ONLY EF TRACKING)
await _context.Users.AddAsync(newUser);

// REQUIRED: push your tracked changes to the actual DB
await _context.SaveChangesAsync();

// return new user object
return newUser;
```

Lastly to note, we can make local changes in a service, then call .SaveChangesAsync() in
the respository (which has the db context to do so) and it will save the changes
made in the service, assuming they were all tracked by EF. (verify this point).

---

#### Handling Exceptions

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; There are two main scenarios when we will want to
throw an exception. The first is when there is simply a logic error/conflict, where
all the code worked, but maybe the user was not found for example. The second is
when a genuine error occurs and your code throws an exception, like if the database
connection is unaccessible and your `FindAsync()` function call throws an error. In
the first instance we want to manually throw a descriptive exception, using the proper
exception type and appending a helpful message as follows:
```
// During sign-up, check if the provided email is already in use
var alreadyExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

// if a user already exists with this email, return an error.
if (alreadyExists)
{
    _logger.LogError($"User {user.Email} already exists");
    throw new InvalidOperationException("User already exists");
}
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; In the second instance, this is where a generic
exception can be thrown and a 500 error would be the response to the client. 
For example:
```
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    // We catch the specific database failure, log the technical details, 
    // and throw a descriptive message up to the controller.
    _logger.LogCritical(ex, "Database save failed.");
    throw new Exception("The database is currently unavailable.", ex);
}
```
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; The advantage to this method is the controller
can handle each exception type specifically. So if we use proper exceptions,
the controller can use multiple catch blocks as so:
```
try
{
    var result = await _userService.Register(dto);
    return Ok(result);
}
catch (InvalidOperationException ex)
{
    // The business rule was broken, send a client error with the message returned by the exception.
    return Conflict(new { message = ex.Message }); // HTTP 409 Conflict
}
catch (Exception ex)
{
    // Something broke on our servers. Send a 500 error.
    return StatusCode(500, new { message = "An internal server error occurred." }); // HTTP 500
}
```