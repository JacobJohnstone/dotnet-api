using DotnetApi.Models.Users;

namespace DotnetApi.Repositories.Authentication;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken> GetTokenByHashAsync(string refreshTokenHash);
    public Task<RefreshToken[]> GetTokensByUserIdAsync(string userId);
    public Task<RefreshToken> UpdateTokenRevocationDateAsync(DateTime revocationDate);
    public Task<RefreshToken> UpdateTokenReplacedByTokenHashAsync(RefreshToken refreshTokenHash);
    public Task AddRefreshTokenAsync(RefreshTokenDto refreshToken);
    public Task RemoveRefreshTokenAsync(string refreshToken);
}