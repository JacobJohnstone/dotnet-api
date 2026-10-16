using DotnetApi.Models;
using DotnetApi.Models.Common;
using DotnetApi.Models.Users;

namespace DotnetApi.Repositories.Users;

/// <summary>
///     The repositoy for handling user interactions
/// </summary>
public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task SaveChangesAsync();
    Task<bool> EmailExistsAsync(string email);
    Task AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
}