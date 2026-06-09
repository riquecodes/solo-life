namespace SoloLife.Application.Features.Auth.Refresh;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IUserRepository users,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var hash = _tokenGenerator.HashRefreshToken(request.RefreshToken);
        var user = await _users.GetByRefreshTokenHashAsync(hash, cancellationToken);

        if (user is null || user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            return Result.Failure<AuthResponse>("Refresh token inválido ou expirado.");

        // Rotação: cada refresh invalida o token anterior.
        var refresh = _tokenGenerator.GenerateRefreshToken();
        user.RefreshTokenHash = refresh.Hash;
        user.RefreshTokenExpiresAt = refresh.ExpiresAt;

        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        return Result.Success(new AuthResponse(accessToken, refresh.Value));
    }
}
