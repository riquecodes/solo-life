namespace SoloLife.Application.Features.Auth.Login;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<AuthResponse>("Não implementado."));
}
