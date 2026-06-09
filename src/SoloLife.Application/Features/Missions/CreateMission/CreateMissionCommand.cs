namespace SoloLife.Application.Features.Missions.CreateMission;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;
using SoloLife.Domain.Entities;
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
    private readonly IMissionRepository _missions;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMissionCommandHandler(IMissionRepository missions, IUnitOfWork unitOfWork)
    {
        _missions = missions;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MissionDto>> Handle(CreateMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = new Mission
        {
            UserId = request.UserId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Category = request.Category,
            XpReward = request.XpReward
        };

        await _missions.AddAsync(mission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MissionDto.From(mission));
    }
}
