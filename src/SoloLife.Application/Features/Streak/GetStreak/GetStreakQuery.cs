namespace SoloLife.Application.Features.Streak.GetStreak;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Domain.Services;

public record StreakDto(int CurrentStreak);

public record GetStreakQuery(string UserId)
    : IRequest<Result<StreakDto>>;

public class GetStreakQueryHandler
    : IRequestHandler<GetStreakQuery, Result<StreakDto>>
{
    private readonly IUserRepository _users;
    private readonly StreakService _streakService;

    public GetStreakQueryHandler(IUserRepository users, StreakService streakService)
    {
        _users = users;
        _streakService = streakService;
    }

    public async Task<Result<StreakDto>> Handle(GetStreakQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<StreakDto>("Usuário não encontrado.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return Result.Success(new StreakDto(_streakService.CurrentStreak(user, today)));
    }
}
