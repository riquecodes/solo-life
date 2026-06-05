namespace SoloLife.Application.Features.Missions.CompleteMission;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;

public record CompleteMissionCommand(Guid Id, Guid UserId)
    : IRequest<Result<MissionDto>>;

public class CompleteMissionCommandHandler
    : IRequestHandler<CompleteMissionCommand, Result<MissionDto>>
{
    // TODO: aplicar XP via LevelService + atualizar streak via StreakService + avaliar conquistas.
    public Task<Result<MissionDto>> Handle(CompleteMissionCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<MissionDto>("Não implementado."));
}
