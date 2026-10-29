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
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; The issue presented was when updating and replacing
a refresh token (setting the `replacedByTokenHash` field and returning
a newly generated token), it was possible that two simultaneous
requests tried to perform the same operations on the same refresh token.

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; In the previous implementation if the first
request was processed, the second request could generate a new
token, but then throw trying to update the previous token (leaving the new token
hanging with no rollback in place). Or, before using `ExecuteUpdateAsync` and we were
making the changes to the object model, both requests would call `SaveChangesAsync`
successfully updating the row, but only the last change would take effect (leaving the
first request "succeeding" but having its changes overwritten).