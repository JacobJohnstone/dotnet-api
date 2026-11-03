using System.Security.Claims;
using DotnetApi.Repositories.Users;

namespace DotnetApi.Middleware;

public class UserContextMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;
    
    // TODO: implement caching to prevent lookup on EVERY request each time the same user makes a request
    
    // middleware logic (method called)
    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        // Ensure we have an authenticated request before fetching a user
        if (context.User.Identity?.IsAuthenticated == true)
        {
            // extract user ID claim
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ensure the id is a valid Guid
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                // TODO: Check cache first
                
                // fetch the user
                var user = await userRepository.GetUserByIdAsync(userId);

                // set the user object in the HttpContext
                if (user != null)
                {
                    context.Items["User"] = user;
                }
            }
        }
        
        // call next middleware
        await _next(context);
    }
}

/** Usage

[HttpGet("profile")]
public IActionResult GetProfile()
{
    var user = HttpContext.Items["CurrentUser"] as User;
    if (user == null) return Unauthorized();

    return Ok(user);
}
*/