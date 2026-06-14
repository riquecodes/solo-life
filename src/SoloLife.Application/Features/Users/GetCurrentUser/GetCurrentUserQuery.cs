namespace SoloLife.Application.Features.Users.GetCurrentUser;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Users.Dtos;

public record GetCurrentUserQuery(string UserId)
    : IRequest<Result<UserDto>>;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _users;

    public GetCurrentUserQueryHandler(IUserRepository users) => _users = users;

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return Result.Failure<UserDto>("Usuário não encontrado.");

        return Result.Success(new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.CurrentLevel,
            user.CurrentXp,
            user.CurrentStreak,
            user.CreatedAt));
    }
}
