namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Auth.Login;
using SoloLife.Application.Features.Auth.Refresh;
using SoloLife.Application.Features.Auth.Register;

[AllowAnonymous]
[Route("auth")]
public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
        => ToActionResult(await Sender.Send(command));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
        => ToActionResult(await Sender.Send(command));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
        => ToActionResult(await Sender.Send(command));
}
