namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Users.GetCurrentUser;
using SoloLife.Application.Features.Users.UpdateCurrentUser;

[Authorize]
[Route("users")]
public class UsersController : ApiControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
        => ToActionResult(await Sender.Send(new GetCurrentUserQuery(CurrentUserId)));

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateUserRequest request)
        => ToActionResult(await Sender.Send(new UpdateCurrentUserCommand(CurrentUserId, request.Name)));
}

public record UpdateUserRequest(string Name);
