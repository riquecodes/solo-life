namespace SoloLife.Application.Features.Progress.GetProgress;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Progress.Dtos;
using SoloLife.Domain.Services;

public record GetProgressQuery(string UserId)
    : IRequest<Result<ProgressDto>>;

public class GetProgressQueryHandler
    : IRequestHandler<GetProgressQuery, Result<ProgressDto>>
{
    private readonly IUserRepository _users;
    private readonly LevelService _levelService;

    public GetProgressQueryHandler(IUserRepository users, LevelService levelService)
    {
        _users = users;
        _levelService = levelService;
    }

    public async Task<Result<ProgressDto>> Handle(GetProgressQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<ProgressDto>("Usuário não encontrado.");

        var xpForNextLevel = _levelService.XpForNextLevel(user.CurrentLevel);

        return Result.Success(new ProgressDto(
            user.CurrentLevel,
            user.CurrentXp,
            xpForNextLevel,
            _levelService.RemainingXp(user)));
    }
}
