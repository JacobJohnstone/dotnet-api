using DotnetApi.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

/// <summary>
///     The controller for handling the authentication routes, passing logic off to the
///     services, and returning a response.
/// </summary>
/// <param name="context">The DB context, passed in through DI</param>
public class AuthenticationController(BudgetContext context, IAuthRepository authRepository, IAuthenticationService authenticationService) : ControllerBase
{
    // Controller fields instantiated by "primary controller"
    private readonly BudgetContext _context = context;
    // private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IAuthRepository _authenticationRepository = authRepository;

    #region AuthenticationRoutes

    [HttpPost("Login")]
    [Route("api/v1/[controller]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            var user = await _authenticationRepository.Login(email, password);
        }
        catch
        {
            return Unauthorized();
        }
        
        return Ok();
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        return Ok("Logged out successfully");
    }
    

    #endregion
}