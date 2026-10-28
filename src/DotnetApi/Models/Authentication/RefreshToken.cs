namespace DotnetApi.Models.Users;

public class RefreshToken
{
    /// <summary>
    ///     Token primary key
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    ///     The date and time in which the token will expire
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    ///     The actual refresh token hash value.
    ///     Keep it hashed, as anyone with the actual token can use it to obtain
    ///     a new JWT.
    ///
    ///     Encrypted with sha-256 instead of bcrypt (password hasher), as
    ///     password hashers are intended for a different problem
    /// </summary>
    public required string TokenHash { get; set; }
    
    /// <summary>
    ///     A nullable timestamp to indicate when/if a token has been revoked.
    ///     Checking revoked: if(token.RevokedAt.HasValue)
    /// </summary>
    public DateTime? RevokedAt { get; set; }
    
    /// <summary>
    ///     The time this refresh token was issued
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    ///     The hash value of the token which replaced this one during refresh token rotation/updating
    ///     (Assists in debugging + additional validity checks, ensuring this hasn't been replaced if provided to the refresh endpoint)
    /// </summary>
    public string? ReplacedByTokenHash { get; set; }
}