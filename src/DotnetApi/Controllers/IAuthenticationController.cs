using DotnetApi.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

public interface IAuthenticationController
{
    public Task<IActionResult> Register(RegisterDto payload);
    public Task<IActionResult> Login(LoginDto payload);
    public Task<IActionResult> Logout();
    public Task<IActionResult> Refresh(RefreshTokenDto payload);
}