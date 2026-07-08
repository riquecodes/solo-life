namespace SoloLife.Application.Features.Auth.Register;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;
using SoloLife.Domain.Entities;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
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

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _users.GetByEmailAsync(email, cancellationToken) is not null)
            return Result.Failure<AuthResponse>("E-mail já cadastrado.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            // Avatar default criado junto do usuário — o EF persiste em cascata (FK resolvida após o INSERT).
            Avatar = new Avatar
            {
                CurrentSkin = "default",
                CurrentBackground = "default",
                Accessories = new List<string>()
            }
        };

        var refresh = _tokenGenerator.GenerateRefreshToken();
        user.RefreshTokenHash = refresh.Hash;
        user.RefreshTokenExpiresAt = refresh.ExpiresAt;

        await _users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenGenerator.GenerateAccessToken(user);
        return Result.Success(new AuthResponse(accessToken, refresh.Value));
    }
}
