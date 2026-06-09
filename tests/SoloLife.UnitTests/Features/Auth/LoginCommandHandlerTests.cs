namespace SoloLife.UnitTests.Features.Auth;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Auth.Login;
using SoloLife.Domain.Entities;
using Xunit;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
        => _sut = new LoginCommandHandler(_users, _passwordHasher, _tokenGenerator, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaCredenciaisInvalidas()
    {
        _users.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new LoginCommand("ana@test.com", "senha1234"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Credenciais inválidas.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoSenhaInvalida_RetornaCredenciaisInvalidas()
    {
        var user = new User { Email = "ana@test.com", PasswordHash = "HASHED" };
        _users.GetByEmailAsync("ana@test.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("senha-errada", "HASHED").Returns(false);

        var result = await _sut.Handle(new LoginCommand("ana@test.com", "senha-errada"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Credenciais inválidas.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComCredenciaisValidas_AtualizaLastLoginERotacionaRefresh()
    {
        var user = new User { Email = "ana@test.com", PasswordHash = "HASHED" };
        _users.GetByEmailAsync("ana@test.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("senha1234", "HASHED").Returns(true);
        _tokenGenerator.GenerateRefreshToken()
            .Returns(new RefreshTokenResult("novo-refresh", "novo-hash", DateTime.UtcNow.AddDays(7)));
        _tokenGenerator.GenerateAccessToken(user).Returns("access-jwt");

        var result = await _sut.Handle(new LoginCommand("ana@test.com", "senha1234"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.AccessToken.ShouldBe("access-jwt");
        result.Value.RefreshToken.ShouldBe("novo-refresh");
        user.LastLoginAt.ShouldNotBeNull();
        user.RefreshTokenHash.ShouldBe("novo-hash");
        _users.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
