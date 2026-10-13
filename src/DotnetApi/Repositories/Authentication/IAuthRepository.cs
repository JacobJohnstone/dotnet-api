using DotnetApi.Models;
using DotnetApi.Models.Users;

namespace DotnetApi.Repositories;

public interface IAuthRepository
{
    public Task<Result<User>> Login(string username, string password);
    public Task<Result<User>> Register(CreateUserDto user);
}