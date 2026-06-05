namespace SoloLife.Application.Features.Users.UpdateCurrentUser;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Users.Dtos;

public record UpdateCurrentUserCommand(Guid UserId, string Name)
    : IRequest<Result<UserDto>>;

public class UpdateCurrentUserCommandHandler
    : IRequestHandler<UpdateCurrentUserCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<UserDto>("Não implementado."));
}
