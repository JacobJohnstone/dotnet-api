using System.ComponentModel;
using DotnetApi.Contexts;
using DotnetApi.Models.Authentication;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Repositories.Authentication;

public class RefreshTokenRepository(
    ILogger<RefreshTokenRepository> logger,
    BudgetContext context
) : IRefreshTokenRepository
{
    
    // Fields
    private readonly ILogger<RefreshTokenRepository> _logger = logger;
    private readonly BudgetContext _context = context;

    #region IRefreshTokenRepositoryImplementations

    /// <inheritdoc/>
    public async Task<RefreshToken?> GetTokenByHashAsync(string refreshTokenHash)
    {
        _logger.LogInformation("Querying refresh tokens for token {refreshTokenHash}", refreshTokenHash);
        
        // query based on passed hash and return null or valid token
        return await _context.RefreshTokens
            .Where(rt => rt.TokenHash == refreshTokenHash)
            .SingleOrDefaultAsync();
    }

    /// <inheritdoc/>
    public async Task<RefreshToken?> GetTokenByIdAsync(Guid id)
    {
        _logger.LogInformation("Querying refresh tokens for token {refreshTokenId}", id);
        
        // query based on passed hash and return null or valid token
        return await _context.RefreshTokens
            .Where(rt => rt.Id == id)
            .SingleOrDefaultAsync();
    }
    
    /// <inheritdoc/>
    public async Task<List<RefreshToken>> GetTokensByUserIdAsync(Guid userId, bool? active = true)
    {
        _logger.LogInformation("Getting user tokens");
        
        // query all tokens related to a specific user
        List<RefreshToken> userTokens = [];

        // If inactive, don't consider additional query params, else, query only relevant tokens
        if (!active.Value)
        {
            _logger.LogInformation("Querying all refresh tokens for a user unconditionally");
            // Return all refresh tokens related to user id
            userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }
        else
        {
            _logger.LogInformation("Querying all non-expired/non-revoked/non-replaced refresh tokens for a user");
            // Only return non-revoked, non-replaced tokens matching user id
            userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .Where(rt => rt.RevokedAt == null)
                .Where(rt => rt.ReplacedByTokenHash == null)
                .Where(rt => rt.ExpiresAt >= DateTime.UtcNow)
                .ToListAsync();
        }
        
        _logger.LogInformation("Fetched tokens: {userTokens}", userTokens);
        
        return userTokens;
    }

    /// <inheritdoc/>
    public async Task<RefreshToken> UpdateTokenReplacedByAsync(Guid previousTokenId, string refreshTokenHash)
    {
        _logger.LogInformation("Updating `replacedByTokenHash` field on refrehs token {hash}", refreshTokenHash);
        
        // Find related token
        var refreshToken = await GetTokenByIdAsync(previousTokenId);
        if (refreshToken == null)
        {
            _logger.LogError("Failed to update field, previous token not found for ID {previousId}.", previousTokenId);
            throw new KeyNotFoundException("Failed to update field, previous token not found.");
        }

        // Update token
        refreshToken.ReplacedByTokenHash = refreshTokenHash;
        
        // Save changes and return
        await SaveChangesAsync();

        return refreshToken;
    }

    /// <inheritdoc/>
    public async Task SaveChangesAsync()
    {
        // save changes
        await _context.SaveChangesAsync();
    }
    
    #endregion

    #region LocalHelpers

    

    #endregion
}