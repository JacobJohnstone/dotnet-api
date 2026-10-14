using DotnetApi.Models;
using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Repositories.Authentication;

public interface IAuthRepository
{
    public Task<Result<User>> Login(string email, string password);
    public Task<Result<User>> Register(CreateUserDto user);
}