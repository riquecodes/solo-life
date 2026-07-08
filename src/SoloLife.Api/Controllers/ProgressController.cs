namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Progress.GetProgress;
using SoloLife.Application.Features.Progress.GetProgressHistory;

[Authorize]
[Route("progress")]
public class ProgressController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
        => ToActionResult(await Sender.Send(new GetProgressQuery(CurrentUserId)));

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
        => ToActionResult(await Sender.Send(new GetProgressHistoryQuery(CurrentUserId)));
}
