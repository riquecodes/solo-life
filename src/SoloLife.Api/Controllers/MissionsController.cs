namespace SoloLife.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Features.Missions.CompleteMission;
using SoloLife.Application.Features.Missions.CreateMission;
using SoloLife.Application.Features.Missions.DeleteMission;
using SoloLife.Application.Features.Missions.GetMissions;
using SoloLife.Application.Features.Missions.UpdateMission;
using SoloLife.Domain.Enums;

[Authorize]
[Route("missions")]
public class MissionsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => ToActionResult(await Sender.Send(new GetMissionsQuery(CurrentUserId)));

    [HttpPost]
    public async Task<IActionResult> Create(CreateMissionRequest request)
        => ToActionResult(await Sender.Send(new CreateMissionCommand(
            CurrentUserId, request.Title, request.Description, request.Category, request.XpReward)));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateMissionRequest request)
        => ToActionResult(await Sender.Send(new UpdateMissionCommand(
            id, CurrentUserId, request.Title, request.Description, request.Category, request.XpReward)));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
        => ToActionResult(await Sender.Send(new DeleteMissionCommand(id, CurrentUserId)));

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(string id)
        => ToActionResult(await Sender.Send(new CompleteMissionCommand(id, CurrentUserId)));
}

public record CreateMissionRequest(string Title, string Description, MissionCategory Category, int XpReward);
public record UpdateMissionRequest(string Title, string Description, MissionCategory Category, int XpReward);
