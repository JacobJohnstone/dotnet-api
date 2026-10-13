using DotnetApi.Models.Users;

namespace DotnetApi.Repositories;

public interface IAuthRepository
{
    public Task<User?> Login(string username, string password);
    public Task<User> Register(CreateUserDto user);
}