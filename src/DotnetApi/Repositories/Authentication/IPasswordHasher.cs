using DotnetApi.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace DotnetApi.Repositories.Authentication;

public interface IPasswordHasher
{
    public PasswordHasher<User> PasswordHasher { get; }
}