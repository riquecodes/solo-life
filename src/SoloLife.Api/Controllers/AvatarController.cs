namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Avatar.GetAvatar;
using SoloLife.Application.Features.Avatar.UpdateAvatar;

[Authorize]
[Route("avatar")]
public class AvatarController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
        => ToActionResult(await Sender.Send(new GetAvatarQuery(CurrentUserId)));

    [HttpPut]
    public async Task<IActionResult> Update(UpdateAvatarRequest request)
        => ToActionResult(await Sender.Send(new UpdateAvatarCommand(
            CurrentUserId, request.CurrentSkin, request.CurrentBackground, request.Accessories)));
}

public record UpdateAvatarRequest(string CurrentSkin, string CurrentBackground, List<string> Accessories);
