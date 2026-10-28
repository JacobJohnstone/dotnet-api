using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Services.Authentication;

public interface IAuthService
{
    public Task<Result<TokenResponseDto>> Login(LoginDto credentials);
    public Task<Result<TokenResponseDto>> Register(RegisterDto user);
    public Task Logout();
    public Task<Result<TokenResponseDto>> Refresh();
    
    // Revoke ALL refresh tokens
    // public Task LogoutOfAllDevices(User user)

}