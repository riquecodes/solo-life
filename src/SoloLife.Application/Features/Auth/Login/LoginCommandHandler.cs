namespace SoloLife.Application.Features.Auth.Login;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByEmailAsync(email, cancellationToken);

        // Mensagem genérica de propósito — não revela se o e-mail existe.
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>("Credenciais inválidas.");

        user.LastLoginAt = DateTime.UtcNow;

        var refresh = _tokenGenerator.GenerateRefreshToken();
        user.RefreshTokenHash = refresh.Hash;
        user.RefreshTokenExpiresAt = refresh.ExpiresAt;

        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        return Result.Success(new AuthResponse(accessToken, refresh.Value));
    }
}
