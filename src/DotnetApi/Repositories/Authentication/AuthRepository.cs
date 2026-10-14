using System.Security.Claims;
using DotnetApi.Models;
using DotnetApi.Models.Common;
using DotnetApi.Models.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotnetApi.Repositories.Authentication;

public class AuthRepository(
    BudgetContext dbContext, 
    ILogger<AuthRepository> logger, 
    HttpContext httpContext
    ) : IAuthRepository
{
    // Fields (populated by primary constructor) //
    
    private readonly BudgetContext _context = dbContext;
    private readonly ILogger<AuthRepository> _logger = logger;
    private readonly HttpContext _httpContext = httpContext;
    
    // Methods //
    
    /// <summary>
    ///     Main method for logging in a user. Checks the user exists in
    ///     the DbContext, that the credentials match, and in the future, that
    ///     they are not inactive/banned, before returning a successful login. 
    /// </summary>
    /// <param name="email">The user's email</param>
    /// <param name="password">The user's secret</param>
    /// <returns></returns>
    public async Task<Result<User>> Login(string email, string password)
    {
        // Check if a user exists with this email (and only one instance exists)
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        
        // If a user does not exist return an error
        if (user == null)
        {
            _logger.LogError($"User {email} not found");
            throw new InvalidOperationException("Invalid email or password.");
        }
        
        // check if hashed password matches, if so login, else return error
        var hasher =  new PasswordHasher<User>();
        var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        // if the user provided the wrong password, throw an error
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogError($"User {user.Email} failed to login");
            throw new InvalidOperationException("Invalid email or password.");
        }
        
        // initialize base claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        // initialize claims principal
        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        // initialized
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            IssuedUtc = DateTimeOffset.UtcNow,
        };
        
       // login
       await _httpContext.SignInAsync(
                JwtBearerDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);
        
        // Update "last logged in" flag
        user.LastLogin = DateTime.UtcNow;

        // Persist login timestamp update
        try
        {
            _context.SaveChangesAsync();
        }
        catch
        {
            _logger.LogError($"Failed to update user's ({user.Email}) last logged in timeout.");
        }

        // future: check if user is banned, inactive, deleted etc.

        return Result<User>.Success(user);
    }

    /// <summary>
    ///     Accepts a user creation DTO and adds user to
    ///     the database. Returns the newly created user
    /// </summary>
    /// <param name="user">CreateUserDto payload</param>
    /// <returns>The created user entity</returns>
    public async Task<Result<User>> Register(CreateUserDto user)
    {
        /***********************************************************************************
            If api reaches this function coming from a controller,
            password should be validated as present and of valid length (due to attributes) 
        ***********************************************************************************/

        // check if provided email is already in use.
        // use asNoTracking as this is a readOnly operation
        var alreadyExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Email == user.Email);

        // if a user already exists with this email, return an error.
        if (alreadyExists)
        {
            _logger.LogError($"User {user.Email} already exists");
            throw new InvalidOperationException("This email is already in use.");
        }

        // hash password
        var hasher =  new PasswordHasher<CreateUserDto>();
        var passwordHash = hasher.HashPassword(user, user.Password);
        
        // instantiate new user object
        var newUser = new User
        {
            // Payload fields (user input)
            Id = Guid.NewGuid(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,
            
            // Newly hashed password
            PasswordHash = passwordHash,
        };

        // submit user to the database ONLY EF TRACKING
        await _context.Users.AddAsync(newUser);
        
        // authenticate user with IAuthenticationService
        // initialize base claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString())
        };
        
        // initialize claims principal
        var claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        
        // initialized
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
            IssuedUtc = DateTimeOffset.UtcNow,
        };
        
        // login
        await _httpContext.SignInAsync(
            JwtBearerDefaults.AuthenticationScheme,
            claimsPrincipal,
            authProperties);
        
        // REQUIRED: push your tracked changes to the actual DB
        await _context.SaveChangesAsync();

        // return new user object
        return Result<User>.Success(newUser);
    }
}
