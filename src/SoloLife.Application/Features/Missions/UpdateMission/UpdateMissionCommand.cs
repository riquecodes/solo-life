namespace SoloLife.Application.Features.Missions.UpdateMission;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;
using SoloLife.Domain.Enums;

public record UpdateMissionCommand(
    string Id,
    string UserId,
    string Title,
    string Description,
    MissionCategory Category,
    int XpReward)
    : IRequest<Result<MissionDto>>;

public class UpdateMissionCommandHandler
    : IRequestHandler<UpdateMissionCommand, Result<MissionDto>>
{
    private readonly IMissionRepository _missions;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMissionCommandHandler(IMissionRepository missions, IUnitOfWork unitOfWork)
    {
        _missions = missions;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MissionDto>> Handle(UpdateMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = await _missions.GetByIdAsync(request.Id, cancellationToken);

        // Mensagem genérica de propósito — não revela existência de missão de outro usuário.
        if (mission is null || mission.UserId != request.UserId)
            return Result.Failure<MissionDto>("Missão não encontrada.");

        mission.Title = request.Title.Trim();
        mission.Description = request.Description.Trim();
        mission.Category = request.Category;
        mission.XpReward = request.XpReward;

        _missions.Update(mission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MissionDto.From(mission));
    }
}
