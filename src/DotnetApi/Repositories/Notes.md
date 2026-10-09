# Repositories

### Questions ###

- are "repository" and "service" synonymous?
- repositories would be dependency injected into controllers.. right?

## Notes

---

#### Query formatting
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Majority of queries will be formatted as 
{dbContext}.{DbSet}.{operation**Async**(LINQ)}, or a series of chained SQL
operations for example: 
```
// Where the context is injected, and Users is a DbSet:
var activeUsers = _context.Users.Where(u => u.IsActive)
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