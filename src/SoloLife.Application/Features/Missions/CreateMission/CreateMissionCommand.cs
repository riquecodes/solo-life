namespace SoloLife.Application.Features.Missions.CreateMission;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;
using SoloLife.Domain.Enums;

public record CreateMissionCommand(
    Guid UserId,
    string Title,
    string Description,
    MissionCategory Category,
    int XpReward)
    : IRequest<Result<MissionDto>>;

public class CreateMissionCommandHandler
    : IRequestHandler<CreateMissionCommand, Result<MissionDto>>
{
    public Task<Result<MissionDto>> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<MissionDto>("Não implementado."));
}
