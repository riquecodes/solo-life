namespace SoloLife.Application.Features.Achievements.GetAchievements;

using MediatR;
using SoloLife.Application.Common.Results;

public record AchievementDto(
    Guid Id,
    string Code,
    string Title,
    string Description,
    DateTime? UnlockedAt);

public record GetAchievementsQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<AchievementDto>>>;

public class GetAchievementsQueryHandler
    : IRequestHandler<GetAchievementsQuery, Result<IReadOnlyList<AchievementDto>>>
{
    public Task<Result<IReadOnlyList<AchievementDto>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<IReadOnlyList<AchievementDto>>("Não implementado."));
}
