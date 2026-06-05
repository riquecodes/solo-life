namespace SoloLife.Application.Features.Missions.GetMissions;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;

public record GetMissionsQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<MissionDto>>>;

public class GetMissionsQueryHandler
    : IRequestHandler<GetMissionsQuery, Result<IReadOnlyList<MissionDto>>>
{
    public Task<Result<IReadOnlyList<MissionDto>>> Handle(GetMissionsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<IReadOnlyList<MissionDto>>("Não implementado."));
}
