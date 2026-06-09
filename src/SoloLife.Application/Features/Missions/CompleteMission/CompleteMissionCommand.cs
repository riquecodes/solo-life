namespace SoloLife.Application.Features.Missions.CompleteMission;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;
using SoloLife.Domain.Enums;

public record CompleteMissionCommand(Guid Id, Guid UserId)
    : IRequest<Result<MissionDto>>;

public class CompleteMissionCommandHandler
    : IRequestHandler<CompleteMissionCommand, Result<MissionDto>>
{
    private readonly IMissionRepository _missions;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteMissionCommandHandler(IMissionRepository missions, IUnitOfWork unitOfWork)
    {
        _missions = missions;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MissionDto>> Handle(CompleteMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = await _missions.GetByIdAsync(request.Id, cancellationToken);

        // Mensagem genérica de propósito — não revela existência de missão de outro usuário.
        if (mission is null || mission.UserId != request.UserId)
            return Result.Failure<MissionDto>("Missão não encontrada.");

        if (mission.Status == MissionStatus.Completed)
            return Result.Failure<MissionDto>("Missão já concluída.");

        mission.Status = MissionStatus.Completed;
        mission.CompletedAt = DateTime.UtcNow;

        // TODO: aplicar XP via LevelService + atualizar streak via StreakService + avaliar conquistas.
        _missions.Update(mission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MissionDto.From(mission));
    }
}
