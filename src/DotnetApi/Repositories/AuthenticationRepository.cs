using DotnetApi.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Repositories;

public class AuthenticationRepository(BudgetContext dbContext, ILogger<AuthenticationRepository> logger)
{
    // Fields (populated by primary constructor) //
    
    private readonly BudgetContext _context = dbContext;
    private readonly ILogger<AuthenticationRepository> _logger = logger;
    
    // Methods //
    
    /// <summary>
    ///     Main method for logging in a user. Checks the user exists in
    ///     the DbContext, that the credentials match, and in the future, that
    ///     they are not inactive/banned, before returning a successful login. 
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="password">The user's secret</param>
    /// <returns></returns>
    public async Task<bool> Login(string email, string password)
    {
        // Check if a user exists with this email (and only one instance exists)
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        
        // If a user does not exist return an error
        if (user == null)
        {
            _logger.LogError($"User {email} not found");
            throw new InvalidOperationException("User not found");
        }
        
        // check if hashed password matches, if so login, else return error
        
        
        // future: check if user is banned, inactive, deleted etc.

        return true;
    }

    /// <summary>
    ///     Accepts a user creation DTO and adds user to
    ///     the database. Returns the newly created user
    /// </summary>
    /// <param name="user">CreateUserDto payload</param>
    /// <returns>The created user entity</returns>
    public async Task<User> Register(CreateUserDto user)
    {
        // If api reaches this function coming from a controller,
        // password should be validated as present and of valid length (due to attributes)

        // check if provided email is already in use
        var alreadyExists = await _context.Users.AnyAsync(u => u.Email == user.Email);

        // if a user already exists with this email, return an error.
        if (alreadyExists)
        {
            _logger.LogError($"User {user.Email} already exists");
            throw new InvalidOperationException("User already exists");
        }

        // hash password

        // instantiate new user object
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
        };

        // submit user to the database ONLY EF TRACKING
        await _context.Users.AddAsync(newUser);
        
        // REQUIRED: push your tracked changes to the actual DB
        await _context.SaveChangesAsync();

        // return new user object
        // TODO: return userDto instead?
        return newUser;
    }
}
