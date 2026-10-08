using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

/// <summary>
///     The controller for handling the authentication routes, passing logic off to the
///     services, and returning a response.
/// </summary>
/// <param name="context">The DB context, passed in through DI</param>
public class AuthenticationController(BudgetContext context) : ControllerBase
{
    // Controller fields instantiated by "primary controller"
    private readonly BudgetContext _context = context;
    // private readonly ILogger<AuthenticationController> _logger = logger;
    // private readonly IAuthenticationService _authenticationService = authenticationService;

    #region AuthenticationRoutes

    [HttpGet]
    [Route("api/v1/[controller]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers()
    {
        // manual return 401 on unauthenticated request, or implicitly handled?
        
        // log user fetching initiated
        
        // call and await _authenticationService getUsers method
        
        // if nothing is returned, send "NotFound" response
        
        // return 200 OK response with user list
        return Ok();
    }

    #endregion
}