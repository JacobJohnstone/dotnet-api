# JWT and Authentication

---

#### JSON Web Tokens

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; JWT info
Because JWTs are often short-lived, instead of requiring the user to login again

every 5 - 15 minutes, a refresh token and endpoint are provided. The need for this comes from the
inability to revoke a JWT. If an attacker somehow steals an authenticated user's JWT, it's only useful
for the short duration. If we detect this, we can revoke the refresh token manually to prevent another
JWT from being issued after its expiry.
---

#### Refresh Tokens

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Refresh tokens are used to validate the creation of a new JWT
for a given user. These usually have longer expiration times on them (i.e., 7 days) and allow
the user to stay authenticated for longer, while not risking a 7-day window for a compromised jwt.
Typically, without additional configuration the user would need to login after those 7-days, but
there are strategies that can make this more seamless and prevent login. For example, everytime the
user's token is refreshed, we can reset the refresh token's expiry date, or issue a new refresh token.
This means a user that is frequently online and active won't ever have to login, but a user who
does not authenticate within a 7-day window will need to login again next time they try to access
the application.