# Updating Refresh Tokens

### Topics
- Concurrency
  - Simultaneous requests modifying shared data
- Race conditions
- ACID Database Transactions
  - Atomic, consistent, isolated, durable
  - Ensuring DB transactions either succeed or roll back
- EF Core
  - ExecuteUpdateAsync instead of fetching, modifying, and saving changes
- Fault Tolerance and Distributed Systems
  - Handling failures between database commits and HTTP responses
- Preventing previous TOCTOU race condition bug

---

#### Concurrency
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; The issue presented was updating and replacing
a refresh token (setting the `replacedByTokenHash` field and returning
a newly generated token) allowed the possibility of two simultaneous
requests trying to perform the same operation(s) on the same refresh token.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; In the previous implementation if the first
request was processed, the second request could throw between creating a new token
and trying to update the previous token. Or, before using `ExecuteUpdateAsync` and we 
were making the changes to the object model, both requests would call `SaveChangesAsync`
successfully updating the row, but only the last change would take effect (leaving the
first request "succeeding" but having its changes overwritten).

#### Transactions
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; A transaction is useful if multiple writes are involved --
without one the first operation could succeed (adding the new token), while updating the 
previous token fails.



&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; While the refresh token example transaction is locally scoped
to the method with the `using` keyword and implicitly disposed when the code execution leaves
the function, some transactions can span multiple repositories. Typically these transactions
are managed by the service. Example: Since they all use the same dbContext (from DI), all
database operations are tracked within the same transaction

```
public class RegistrationService(
    AppDbContext context,
    UserRepository userRepository,
    RefreshTokenRepository tokenRepository,
    SettingsRepository settingsRepository)
{
    public async Task RegisterAsync(
        User user,
        RefreshToken token,
        UserSettings settings)
    {
        await using var transaction =
            await context.Database.BeginTransactionAsync();

        try
        {
            await userRepository.AddAsync(user);
            await tokenRepository.AddAsync(token);
            await settingsRepository.AddAsync(settings);

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```
In the above example, no repository should individually commit transaction changes,
but rather just track their changes, letting `SaveChangesAsync()` and
`transaction.CommitAsync()` be called once at the very end.