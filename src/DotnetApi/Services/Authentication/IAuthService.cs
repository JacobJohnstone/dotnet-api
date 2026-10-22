using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Services.Authentication;

public interface IAuthService
{
    // todo: login should return JWT I believe
    public Task<Result<bool>> Login(LoginDto credentials);
    public Task<Result<bool>> Register(RegisterDto user);
    
    // private Token CreateJWT(string email, string hashPassword);
}