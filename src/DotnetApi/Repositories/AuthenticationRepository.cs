using DotnetApi.Models.Users;

namespace DotnetApi.Repositories;

public class AuthenticationRepository(BudgetContext dbContext, ILogger<AuthenticationRepository> logger)
{
    // Fields (populated by primary constructor) //
    
    private readonly BudgetContext _context = dbContext;
    private readonly ILogger<AuthenticationRepository> _logger = logger;
    
    // Methods //
    
    /// <summary>
    ///     Main method for logging in a user. Checks the user exists in
    ///     the DbContext, that the credentials match, and in future, that
    ///     they are not inactive/banned, before returning a successful login. 
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="password">The user's secret</param>
    /// <returns></returns>
    public Task<User> Login(string email, string password)
    {
        // Check if a user exists with this email
        var user = _context.Users.FirstOrDefault(u => u.Email == email);
        
        // If a user does not exist return an error
        if (user == null)
        {
            _logger.LogError($"User {email} not found");
            return Task.FromResult<User>(null);
        }
        
        // check if hashed password matches, if so login, else return error
        
        
        // future: check if user is banned, inactive, deleted etc.
    }

    /// <summary>
    ///     Accepts create user Dto and adds user to the database
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<User> Register(CreateUserDto user)
    {
        // If api reaches this function, password should be validated as present and of valid length (due to attributes)?
        
        var newUser = new User(
        {
            Id = Guid.NewGuid(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
        });

        return Task.FromResult<User>(newUser);
    }
    
}