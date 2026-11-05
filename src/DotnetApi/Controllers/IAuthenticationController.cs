using DotnetApi.Models.Users;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

public interface IAuthenticationController
{
    public Task<IActionResult> Register(RegisterDto payload);
    public Task<IActionResult> Login(LoginDto payload);
    public Task<IActionResult> Logout(string refreshToken);
    public Task<IActionResult> Refresh(string refreshToken);
}