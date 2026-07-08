namespace SoloLife.UnitTests.Features.Auth;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Auth.Refresh;
using SoloLife.Domain.Entities;
using Xunit;

public class RefreshTokenCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenCommandHandlerTests()
        => _sut = new RefreshTokenCommandHandler(_users, _tokenGenerator, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoTokenNaoEncontrado_RetornaFailure()
    {
        _tokenGenerator.HashRefreshToken("bruto").Returns("hash");
        _users.GetByRefreshTokenHashAsync("hash", Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new RefreshTokenCommand("bruto"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Refresh token inválido ou expirado.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoTokenExpirado_RetornaFailure()
    {
        _tokenGenerator.HashRefreshToken("bruto").Returns("hash");
        _users.GetByRefreshTokenHashAsync("hash", Arg.Any<CancellationToken>())
            .Returns(new User { RefreshTokenHash = "hash", RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1) });

        var result = await _sut.Handle(new RefreshTokenCommand("bruto"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Refresh token inválido ou expirado.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComTokenValido_RotacionaERetornaNovosTokens()
    {
        var user = new User
        {
            RefreshTokenHash = "hash-antigo",
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(3)
        };
        _tokenGenerator.HashRefreshToken("bruto").Returns("hash-antigo");
        _users.GetByRefreshTokenHashAsync("hash-antigo", Arg.Any<CancellationToken>()).Returns(user);
        _tokenGenerator.GenerateRefreshToken()
            .Returns(new RefreshTokenResult("novo-refresh", "novo-hash", DateTime.UtcNow.AddDays(7)));
        _tokenGenerator.GenerateAccessToken(user).Returns("access-jwt");

        var result = await _sut.Handle(new RefreshTokenCommand("bruto"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.AccessToken.ShouldBe("access-jwt");
        result.Value.RefreshToken.ShouldBe("novo-refresh");
        user.RefreshTokenHash.ShouldBe("novo-hash");
        _users.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
