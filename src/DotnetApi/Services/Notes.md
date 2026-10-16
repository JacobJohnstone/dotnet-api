# Services

## Notes

---

#### Handling Exceptions - (Repeated from repository notes)

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