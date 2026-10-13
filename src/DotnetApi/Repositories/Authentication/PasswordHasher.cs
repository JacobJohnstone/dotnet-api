using DotnetApi.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace DotnetApi.Repositories.Authentication;

public class PasswordHasher
{
    public IPasswordHasher<User> passwordHasher { get; } = new PasswordHasher<User>();
}