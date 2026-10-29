using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Models.Common;
using DotnetApi.Models.Users;
using DotnetApi.Models.Authentication;
using DotnetApi.Repositories.Authentication;
using DotnetApi.Repositories.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Services.Authentication;

public class AuthService(
    ILogger<AuthService> logger,
    IUserRepository userRepository,
    IConfiguration _configuration,
    IRefreshTokenRepository refreshTokenRepository
) : IAuthService
{
    // Fields //
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

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
    public async Task<Result<TokenResponseDto>> Login(LoginDto credentials)
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
            return Result<TokenResponseDto>.Failure("Invalid email or password.", HttpStatusCode.BadRequest);
        }
        
        // validate password
        var hasher = new PasswordHasher<User>();
        var comparisonResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        var isValidPassword = comparisonResult != PasswordVerificationResult.Failed;

        if (!isValidPassword)
        {
            _logger.LogInformation("Invalid password");
            return Result<TokenResponseDto>.Failure("Invalid email or password.", HttpStatusCode.BadRequest);
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
        
        // generate refresh token
        var tokenString = GenerateRefreshToken();
        var refreshHash = HashRefreshToken(tokenString);
        
        // Create token entity
        var refreshToken = new RefreshToken(refreshHash);
        
        // build return value
        var tokenResponse = new TokenResponseDto
        {
            RefreshToken = tokenString,
            AccessToken = jwt,
            RefreshExpiresAt = refreshToken.ExpiresAt,
        };
        
        return Result<TokenResponseDto>.Success(tokenResponse, HttpStatusCode.OK);
    }


    /// <summary>
    ///     Creates a new user, handling sign-ups.
    /// </summary>
    /// <param name="user">A CreateUserDto, built from user inputs</param>
    /// <returns>The new user's JWT</returns>
    public async Task<Result<TokenResponseDto>> Register(RegisterDto user)
    {
        _logger.LogInformation($"Registering user {user.FirstName} {user.LastName}");
        
        // Verify unique email
        var isEmailTaken = await _userRepository.EmailExistsAsync(user.Email);
        if (isEmailTaken)
        {
            _logger.LogInformation("Email already registered.");
            return Result<TokenResponseDto>.Failure("An account already exists with this email.", HttpStatusCode.Conflict);
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
        
        // generate new jwt
        var jwt = GenerateJWT(createdUser);
        
        // generate new refresh token
        var refresh = GenerateRefreshToken();
        var refreshHash = HashRefreshToken(refresh);
        
        // Create new refresh entity token to store
        var refreshToken = new RefreshToken(refreshHash);
        
        // build return type
        var data = new TokenResponseDto
        {
            AccessToken = jwt,
            RefreshToken = refresh,
            RefreshExpiresAt = refreshToken.ExpiresAt,
        };
        
        // committ changes to refresh token DbSet
        
        return Result<TokenResponseDto>.Success(data, HttpStatusCode.OK);
    }
    
    public async Task Logout()
    {
        // revoke refresh token
        _logger.LogInformation("Logged out.");

    }

    public async Task<Result<TokenResponseDto>> Refresh(string refreshToken, User user)
    {
        _logger.LogInformation("Refreshing token...");
        
        // fetch the existing token
        var tokenHash = HashRefreshToken(refreshToken);
        var token = await _refreshTokenRepository.GetTokenByHashAsync(tokenHash);

        _logger.LogInformation("Refresh token found.");
        
        // ensure valid (not expired)
        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            _logger.LogInformation("Refresh token expired at {ExpiresAt}", token.ExpiresAt);
            return Result<TokenResponseDto>.Failure("Token is expired", HttpStatusCode.Unauthorized);
        }
        
        // issue new jwt
        _logger.LogInformation("Refresh token valid, generating new jwt.");
        var jwt = GenerateJWT(user);
        
        // issue a new refresh token
        var newRefreshToken = GenerateRefreshToken();
        var newRefreshHash = HashRefreshToken(newRefreshToken);

        // handles new refresh token added to db and previous token validation/updates
        var generatedToken = await _refreshTokenRepository.TryRotateRefreshTokenAsync(token.Id, newRefreshHash);

        if (generatedToken == null)
        {
            _logger.LogError("Failed to rotate refresh token");
            return  Result<TokenResponseDto>.Failure("Failed to rotate refresh token", HttpStatusCode.Conflict);
        }
        else
        {
            _logger.LogInformation("Refresh token rotated");
            var tokenResp = new TokenResponseDto
            {
                AccessToken = jwt,
                RefreshToken = newRefreshToken,
                RefreshExpiresAt = generatedToken.ExpiresAt,

            };
            return Result<TokenResponseDto>.Success(tokenResp, HttpStatusCode.OK);
        }
        
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

    private string GenerateRefreshToken()
    {
        // generate token
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        
        string plainTextToken = Convert.ToBase64String(randomBytes);
        return plainTextToken;
    }

    private string HashRefreshToken(string refreshToken)
    {
        // compute SHA256 hash of refresh token
        byte[] inputBytes = Encoding.UTF8.GetBytes(refreshToken);
        byte[] hashBytes = SHA256.HashData(inputBytes);
        
        // convert to clean hexadecimal string
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
    
    #endregion
}