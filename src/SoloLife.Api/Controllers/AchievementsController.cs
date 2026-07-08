namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Achievements.GetAchievements;

[Authorize]
[Route("achievements")]
public class AchievementsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
        => ToActionResult(await Sender.Send(new GetAchievementsQuery(CurrentUserId)));
}
