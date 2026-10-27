using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Services.Authentication;

public interface IAuthService
{
    public Task<Result<string>> Login(LoginDto credentials);
    public Task<Result<string>> Register(RegisterDto user);
    public Task<Result<null>> Logout();
    public Task<Result<string>> Refresh();

}