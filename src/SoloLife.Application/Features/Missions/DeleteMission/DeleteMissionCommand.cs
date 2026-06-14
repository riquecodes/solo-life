namespace SoloLife.Application.Features.Missions.DeleteMission;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;

public record DeleteMissionCommand(string Id, string UserId)
    : IRequest<Result>;

public class DeleteMissionCommandHandler
    : IRequestHandler<DeleteMissionCommand, Result>
{
    private readonly IMissionRepository _missions;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMissionCommandHandler(IMissionRepository missions, IUnitOfWork unitOfWork)
    {
        _missions = missions;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = await _missions.GetByIdAsync(request.Id, cancellationToken);

        // Mensagem genérica de propósito — não revela existência de missão de outro usuário.
        if (mission is null || mission.UserId != request.UserId)
            return Result.Failure("Missão não encontrada.");

        _missions.Remove(mission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
