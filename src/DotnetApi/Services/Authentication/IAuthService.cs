using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Services.Authentication;

public interface IAuthService
{
    public Task<Result<string>> Login(LoginDto credentials);
    public Task<Result<string>> Register(RegisterDto user);
    public Task Logout();
    public Task<Result<string>> Refresh();
    
    // Revoke ALL refresh tokens
    // public Task LogoutOfAllDevices(User user)

}