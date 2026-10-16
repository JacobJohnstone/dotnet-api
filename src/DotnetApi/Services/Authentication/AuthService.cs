using DotnetApi.Models.Common;
using DotnetApi.Models.Users;
using DotnetApi.Repositories.Users;
using Microsoft.AspNetCore.Identity;

namespace DotnetApi.Services.Authentication;

public class AuthService(
    ILogger<AuthService> logger,
    IUserRepository userRepository
) : IAuthService
{
    // Fields //
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    
    // Methods //

    /// <summary>
    ///     Main method for logging in a user. Checks the user exists using the
    ///     userRepository, that the credentials match, and in the future, that
    ///     they are not inactive/banned/locked, before returning a successful
    ///     login. 
    /// </summary>
    /// <param name="credentials">
    ///     The user's email and password, wrapped as a single loginDto
    /// </param>
    /// <returns>
    ///     The JWT
    /// </returns>
    public async Task<Result<bool>> Login(LoginDto credentials)
    {
        var email = credentials.email;
        var password = credentials.password;
        
        // log method start
        _logger.LogInformation($"Logging in user {email}");
        
        // check if a user already exists with this email
        User? user = await _userRepository.GetUserByEmailAsync(email);
        
        // validate email
        if (user == null)
        {
            _logger.LogInformation("User not found (invalid email).");
            return Result<bool>.Failure("Invalid email or password.");
        }
        
        // validate password
        var hasher = new PasswordHasher<User>();
        var comparisonResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        var isValidPassword = comparisonResult != PasswordVerificationResult.Failed;

        if (!isValidPassword)
        {
            _logger.LogInformation("Invalid password");
            return Result<bool>.Failure("Invalid email or password.");
        }
        
        // todo: generate jwt
        
        // update last login flag
        user.LastLogin = DateTime.UtcNow;
        try
        {
            await _userRepository.SaveChangesAsync();
        }
        catch
        {
            _logger.LogWarning("Failed to update last logged in timestamp.");
            // no early return, simply add a silent warning log.
        }
        
        // todo: return jwt
        return Result<bool>.Success(true);
    }


    /// <summary>
    ///     Creates a new user, handling sign-ups.
    /// </summary>
    /// <param name="user">A CreateUserDto, built from user inputs</param>
    /// <returns>The new user's JWT</returns>
    public async Task<Result<bool>> Register(CreateUserDto user)
    {
        _logger.LogInformation($"Registering user {user.FirstName} {user.LastName}");
        
        // Verify unique email
        var isEmailTaken = await _userRepository.EmailExistsAsync(user.Email);
        if (isEmailTaken)
        {
            _logger.LogInformation("Email already registered.");
            return Result<bool>.Failure("An account already exists with this email.");
        }
        
        // hash user password
        var hasher = new PasswordHasher<CreateUserDto>();
        var hash = hasher.HashPassword(user, user.Password);
        
        _logger.LogInformation($"Password hashed.");
        
        // build createUserDto
        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            PasswordHash = hash,
            LastLogin =  DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt =  DateTime.UtcNow,
        };
        
        // add user to db context
        await _userRepository.AddUserAsync(createdUser);
        
        _logger.LogInformation("New user added to db context.");
        
        // persist new user to db
        await _userRepository.SaveChangesAsync();
        
        _logger.LogInformation("New user changes saved.");
        
        // todo: generate jwt and return
        return Result<bool>.Success(true);
    }
}