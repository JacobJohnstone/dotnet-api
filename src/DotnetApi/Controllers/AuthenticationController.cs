using DotnetApi.Models.Users;
using DotnetApi.Services.Authentication;
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
public class AuthenticationController(
        IAuthService authService,
        ILogger<AuthenticationController> logger
    ) : ControllerBase
{
    // Fields instantiated by primary constructor //
    private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IAuthService _authenticationService = authService;

    #region AuthenticationRoutes
    
    [HttpPost("Register")]
    [Route("api/v1/[controller]/register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto payload
    )
    {
        try
        {
            // register through auth service
            var result = await _authenticationService.Register(payload);

            // If the email is taken, return a 409 conflict
            if (result.Exception == "This email is already in use.")
            {
                return Conflict("This email is already in use.");
            }
            
            // if the registration failed for another reason, return a bad request
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            // If no error was found, return a "created" status with the result containing the new user entity
            // TODO: update with a userDto or don't send result in response? (User entity contains password hash)
            return Created($"api/v1/[controller]/register", result);
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Register failed: " + ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
        }
    }

    [HttpPost("Login")]
    [Route("api/v1/[controller]/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto payload
    )
    {
        try
        {
            // perform login through auth service
            var result = await _authenticationService.Login(payload);

            // if the login failed, return a 400
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            
            return Ok();
        }
        catch
        {
            // return 500 and log critical error if unsuccessful
            _logger.LogCritical("Login failed.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("logout")]
    [Route("api/v1/[controller]/logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // perform logout
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out.");
        }
        catch
        {
            // return 500 and log critical error if unsuccessful
            _logger.LogCritical("Logout failed.");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        
        // return 200 ok if successfully logged out
        return Ok("Logged out successfully");
    }
    

    #endregion
}