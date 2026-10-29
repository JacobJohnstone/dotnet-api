using DotnetApi.Models.Authentication;

namespace DotnetApi.Repositories.Authentication;

public interface IRefreshTokenRepository
{
    /// <summary>
    ///     Queries based on provided hash and returns matching refresh token entity
    /// </summary>
    /// <param name="refreshTokenHash">hashed string of related refresh token</param>
    /// <returns>Null or the RefreshToken</returns>
    public Task<RefreshToken?> GetTokenByHashAsync(string refreshTokenHash);
    
    /// <summary>
    ///     Fetches a refresh token by its primary key guid ID value
    /// </summary>
    /// <param name="id">Guid of token</param>
    /// <returns>RefreshToken or Null</returns>
    public Task<RefreshToken?> GetTokenByIdAsync(Guid id);
    
    /// <summary>
    ///     Fetches and returns all refresh tokens corresponding to a specific user.
    ///     Provide an optional parameter, defaulted to true, to fetch only active,
    ///     non expired, non revoked refresh tokens
    /// </summary>
    /// <param name="userId">The ID of the user being queried on</param>
    /// <returns>A list of refresh tokens</returns>
    public Task<List<RefreshToken>> GetTokensByUserIdAsync(Guid userId, bool? active = true);
    
    /// <summary>
    ///     Safely, using a local transaction, updates the replacedTokenHash and revokedAt fields
    /// </summary>
    /// <param name="previousTokenId">guid of expired/previous token</param>
    /// <param name="replacementTokenHash">hash of newly generated token</param>
    /// <returns>Refresh token after updates</returns>
    public Task<bool> TryRotateRefreshTokenAsync(Guid previousTokenId, string replacementTokenHash);
    
    /// <summary>
    ///     When creating a new refresh token, adds it to the DbSet
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public Task AddRefreshTokenAsync(RefreshToken refreshToken);

    /// <summary>
    ///     Revokes a single refresh token, specifying the selected token by id.
    ///     Sets the revocation date to DateTime.UtcNow
    /// </summary>
    /// <param name="id">guid primary key of refresh token</param>
    /// <returns></returns>
    public Task RevokeRefreshTokenByIdAsync(Guid id);

    /// <summary>
    ///     Revokes all refresh tokens assigned to a specific user.
    ///     Useful for "log out of fall devices" functionality.
    ///     Sets all revocation date fields to DateTime.UtcNow
    /// </summary>
    /// <param name="userId">user's guid primary key</param>
    /// <returns>void</returns>
    public Task RevokeTokensByUserIdAsync(Guid userId);

    /// <summary>
    ///     Commits EF tracked changes made within the refresh token repository
    /// </summary>
    /// <returns></returns>
    public Task SaveChangesAsync();
}