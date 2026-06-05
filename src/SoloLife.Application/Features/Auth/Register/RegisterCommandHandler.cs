namespace SoloLife.Application.Features.Auth.Register;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    // TODO: injetar IUserRepository, IPasswordHasher, IJwtTokenGenerator, IUnitOfWork.
    public Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<AuthResponse>("Não implementado."));
}
