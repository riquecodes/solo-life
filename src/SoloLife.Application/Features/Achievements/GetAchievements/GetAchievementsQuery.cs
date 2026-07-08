namespace SoloLife.Application.Features.Achievements.GetAchievements;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;

public record AchievementDto(
    string Id,
    string Code,
    string Title,
    string Description,
    DateTime? UnlockedAt);

public record GetAchievementsQuery(string UserId)
    : IRequest<Result<IReadOnlyList<AchievementDto>>>;

public class GetAchievementsQueryHandler
    : IRequestHandler<GetAchievementsQuery, Result<IReadOnlyList<AchievementDto>>>
{
    private readonly IAchievementRepository _achievements;

    public GetAchievementsQueryHandler(IAchievementRepository achievements) => _achievements = achievements;

    public async Task<Result<IReadOnlyList<AchievementDto>>> Handle(GetAchievementsQuery request, CancellationToken cancellationToken)
    {
        var achievements = await _achievements.GetByUserAsync(request.UserId, cancellationToken);

        IReadOnlyList<AchievementDto> dtos = achievements
            .Select(a => new AchievementDto(a.Id, a.Code, a.Title, a.Description, a.UnlockedAt))
            .ToList();

        return Result.Success(dtos);
    }
}
