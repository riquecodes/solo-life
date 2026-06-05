namespace SoloLife.Application.Features.Streak.GetStreak;

using MediatR;
using SoloLife.Application.Common.Results;

public record StreakDto(int CurrentStreak);

public record GetStreakQuery(Guid UserId)
    : IRequest<Result<StreakDto>>;

public class GetStreakQueryHandler
    : IRequestHandler<GetStreakQuery, Result<StreakDto>>
{
    public Task<Result<StreakDto>> Handle(GetStreakQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<StreakDto>("Não implementado."));
}
