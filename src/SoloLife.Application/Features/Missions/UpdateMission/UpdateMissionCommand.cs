namespace SoloLife.Application.Features.Missions.UpdateMission;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;
using SoloLife.Domain.Enums;

public record UpdateMissionCommand(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    MissionCategory Category,
    int XpReward)
    : IRequest<Result<MissionDto>>;

public class UpdateMissionCommandHandler
    : IRequestHandler<UpdateMissionCommand, Result<MissionDto>>
{
    public Task<Result<MissionDto>> Handle(UpdateMissionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<MissionDto>("Não implementado."));
}
