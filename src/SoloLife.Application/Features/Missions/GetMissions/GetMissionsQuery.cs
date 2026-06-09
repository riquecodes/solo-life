namespace SoloLife.Application.Features.Missions.GetMissions;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;

public record GetMissionsQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<MissionDto>>>;

public class GetMissionsQueryHandler
    : IRequestHandler<GetMissionsQuery, Result<IReadOnlyList<MissionDto>>>
{
    private readonly IMissionRepository _missions;

    public GetMissionsQueryHandler(IMissionRepository missions)
        => _missions = missions;

    public async Task<Result<IReadOnlyList<MissionDto>>> Handle(GetMissionsQuery request, CancellationToken cancellationToken)
    {
        var missions = await _missions.GetByUserAsync(request.UserId, cancellationToken);
        IReadOnlyList<MissionDto> dtos = missions.Select(MissionDto.From).ToList();
        return Result.Success(dtos);
    }
}
