namespace SoloLife.Application.Features.Auth.Refresh;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<AuthResponse>("Não implementado."));
}
