using DotnetApi.Models.Users;
using DotnetApi.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Repositories.Users;

public class UserRepository(
    BudgetContext dbContext, 
    ILogger<UserRepository> logger
    ) : IUserRepository
{
    // Fields (populated by primary constructor) //
    
    private readonly BudgetContext _context = dbContext;
    private readonly ILogger<UserRepository> _logger = logger;
    
    // Methods //
    
    /// <summary>
    ///     Persists any outstanding changes in the context to the db
    /// </summary>
    public async Task SaveChangesAsync()
    {
        _logger.LogInformation($"Saving changes to {_context.Users.Count()} users");
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    ///     Finds a user based on their email field.
    /// </summary>
    /// <param name="email">The desired user's email</param>
    /// <returns>The user entity or null</returns>
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        // fetch by email
        // single or default will throw an exception if more than
        // one row satisfies the condition, ensuring only one email
        // is issued per user
        _logger.LogInformation($"Getting user {email}");
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        
        // TODO: consider how to handle the exception thrown when two of
        // the same email are in the db and this throws
        
        _logger.LogInformation($"Returning {user?.FirstName} {user?.LastName}");
        return user;
    }

    /// <summary>
    ///     Searches the db for the existence of a specified email.
    /// </summary>
    /// <param name="email">email address</param>
    /// <returns>true or false</returns>
    public async Task<bool> EmailExistsAsync(string email)
    {
        _logger.LogInformation($"Checking if email {email} exists.");
        var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
        
        _logger.LogInformation($"Returning email exists bool: {emailExists}");
        return emailExists;
    }

    /// <summary>
    ///     Adds the user to the db context. DOES NOT CALL SaveChangesAsync,
    ///     that is the responsibility of the caller.
    /// </summary>
    /// <param name="user">User entity</param>
    public async Task AddUserAsync(User user)
    {
        _logger.LogInformation($"Adding user {user.FirstName} {user.LastName}");
        await _context.Users.AddAsync(user);
    }

    /// <summary>
    ///     Find a user from an available ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>User entity</returns>
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        // fetch user based on ID
        _logger.LogInformation($"Retreiving user {userId}");
        var user = await _context.Users.FindAsync(userId);

        // Log result
        if (user == null)
        {
            _logger.LogInformation($"User {userId} not found.");
        }
        else
        {
            _logger.LogInformation($"Returning {user?.FirstName} {user?.LastName}");
        }
        
        return user;
    }
}
