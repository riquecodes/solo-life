namespace SoloLife.Application.Features.Users.GetCurrentUser;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Users.Dtos;

public record GetCurrentUserQuery(Guid UserId)
    : IRequest<Result<UserDto>>;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<UserDto>("Não implementado."));
}
