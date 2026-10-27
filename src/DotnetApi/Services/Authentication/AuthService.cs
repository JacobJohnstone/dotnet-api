using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using DotnetApi.Models.Common;
using DotnetApi.Models.Users;
using DotnetApi.Repositories.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Services.Authentication;

public class AuthService(
    ILogger<AuthService> logger,
    IUserRepository userRepository,
    IConfiguration _configuration
) : IAuthService
{
    // Fields //
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;

    #region IAuthService Implementations


    

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
    public async Task<Result<string>> Login(LoginDto credentials)
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
            return Result<string>.Failure("Invalid email or password.", HttpStatusCode.BadRequest);
        }
        
        // validate password
        var hasher = new PasswordHasher<User>();
        var comparisonResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        var isValidPassword = comparisonResult != PasswordVerificationResult.Failed;

        if (!isValidPassword)
        {
            _logger.LogInformation("Invalid password");
            return Result<string>.Failure("Invalid email or password.", HttpStatusCode.BadRequest);
        }

        var jwt = GenerateJWT(user);
        
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
        
        return Result<string>.Success(jwt, HttpStatusCode.OK);
    }


    /// <summary>
    ///     Creates a new user, handling sign-ups.
    /// </summary>
    /// <param name="user">A CreateUserDto, built from user inputs</param>
    /// <returns>The new user's JWT</returns>
    public async Task<Result<string>> Register(RegisterDto user)
    {
        _logger.LogInformation($"Registering user {user.FirstName} {user.LastName}");
        
        // Verify unique email
        var isEmailTaken = await _userRepository.EmailExistsAsync(user.Email);
        if (isEmailTaken)
        {
            _logger.LogInformation("Email already registered.");
            return Result<string>.Failure("An account already exists with this email.", HttpStatusCode.Conflict);
        }
        
        // hash user password
        var hasher = new PasswordHasher<RegisterDto>();
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
        
        var jwt = GenerateJWT(createdUser);
        
        return Result<string>.Success(jwt, HttpStatusCode.OK);
    }
    
    public async Task<Result<null>> Logout()
    {
        
    }

    public async Task<Result<string>> Refresh(RefreshTokenDto payload)
    {
        // check refresh token
        // ensure valid (not expired)
    }
    
    #endregion

    #region Local Helpers
    
    /// <summary>
    ///     Responsible for creating the JWT on login/register
    /// </summary>
    /// <param name="user"></param>
    private string GenerateJWT(User user)
    {
        // access configuration settings for jwt secret signing key
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

        // Generate claims
        var claims = new[]
        {
            // Standard claims
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            // additional details
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            
            // Completely custom claims
            new Claim("onboarding_complete", user.OnboardingCompleted.ToString() ?? "false"),
            new Claim("tutorial_completed", user.TutorialCompleted.ToString() ?? "false"),
        };
        
        // configure signing credentials
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), 
            SecurityAlgorithms.HmacSha256Signature
        );

        // build the token blueprint
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = signingCredentials,
        };
        
        // generate/serialize token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    ///     Will be responsible for clearing/disabling a jwt during logout, deleted user, etc.
    /// </summary>
    /// <param name="user"></param>
    private void ClearJWT(User user)
    {
        
    }
    
    #endregion
}