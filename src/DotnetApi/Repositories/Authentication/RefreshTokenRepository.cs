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
    public async Task UpdateTokenReplacedByAsync(Guid previousTokenId, string refreshTokenHash)
    {
        _logger.LogInformation("Updating `replacedByTokenHash` field on refrehs token {hash}", refreshTokenHash);
        
        // Find related token, ensure it's not revoked etc., and update
        try
        {
            // Inplace update as opposed to fetching, updating, and calling `await SaveChangesAsync()`
            // Required to prevent concurrent requests simultaneously updating the same row (possible during
            // the fetch, update, save scenario)
            
            // By checking if the replacement token hash field is still null, we know at this exact moment, the row has not
            // been updated by another request yet
            
            // The issue with the previous implementation was that the validation and modifications were handled separately
            var updatedToken = await _context.RefreshTokens
                .Where(rt => rt.Id == previousTokenId &&        // the refresh token in question
                             rt.ReplacedByTokenHash == null &&  // no other request has updated/replaced the token yet
                             rt.ExpiresAt >= DateTime.UtcNow && // token has not expired, therefore can be re-issued
                             rt.RevokedAt == null               // the token has not been revoked, therefore it can be re-issued
                )
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(token => token.ReplacedByTokenHash, refreshTokenHash)
                );
        }
        catch (Exception ex)
        {
            // if update fails, log and throw exception
            _logger.LogError("Failed to update refresh token `ReplacedBy` field: {message}", ex.Message);
            throw new Exception(ex.Message);
        }
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