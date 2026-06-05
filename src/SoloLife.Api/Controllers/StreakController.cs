namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Streak.GetStreak;

[Authorize]
[Route("streak")]
public class StreakController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
        => ToActionResult(await Sender.Send(new GetStreakQuery(CurrentUserId)));
}
