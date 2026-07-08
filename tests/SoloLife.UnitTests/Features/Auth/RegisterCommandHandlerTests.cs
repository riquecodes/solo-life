namespace SoloLife.UnitTests.Features.Auth;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Auth.Register;
using SoloLife.Domain.Entities;
using Xunit;

public class RegisterCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterCommandHandler _sut;

    public RegisterCommandHandlerTests()
        => _sut = new RegisterCommandHandler(_users, _passwordHasher, _tokenGenerator, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoEmailJaCadastrado_RetornaFailureSemPersistir()
    {
        _users.GetByEmailAsync("ana@test.com", Arg.Any<CancellationToken>())
            .Returns(new User { Email = "ana@test.com" });

        var result = await _sut.Handle(new RegisterCommand("Ana", "ana@test.com", "senha1234"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("E-mail já cadastrado.");
        await _users.DidNotReceive().AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComDadosValidos_CriaUsuarioComEmailNormalizadoESenhaHasheada()
    {
        _users.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordHasher.Hash("senha1234").Returns("HASHED");
        _tokenGenerator.GenerateRefreshToken()
            .Returns(new RefreshTokenResult("refresh-bruto", "refresh-hash", DateTime.UtcNow.AddDays(7)));
        _tokenGenerator.GenerateAccessToken(Arg.Any<User>()).Returns("access-jwt");

        var result = await _sut.Handle(new RegisterCommand("  Ana  ", "Ana@Test.com", "senha1234"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.AccessToken.ShouldBe("access-jwt");
        result.Value.RefreshToken.ShouldBe("refresh-bruto");

        await _users.Received(1).AddAsync(
            Arg.Is<User>(u =>
                u.Name == "Ana" &&
                u.Email == "ana@test.com" &&
                u.PasswordHash == "HASHED" &&
                u.RefreshTokenHash == "refresh-hash"),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComDadosValidos_CriaAvatarPadraoParaUsuario()
    {
        _users.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);
        _passwordHasher.Hash(Arg.Any<string>()).Returns("HASHED");
        _tokenGenerator.GenerateRefreshToken()
            .Returns(new RefreshTokenResult("refresh-bruto", "refresh-hash", DateTime.UtcNow.AddDays(7)));
        _tokenGenerator.GenerateAccessToken(Arg.Any<User>()).Returns("access-jwt");

        await _sut.Handle(new RegisterCommand("Ana", "ana@test.com", "senha1234"), default);

        // Avatar default criado junto do usuário (via navegação) — EF persiste em cascata.
        await _users.Received(1).AddAsync(
            Arg.Is<User>(u => u.Avatar != null), Arg.Any<CancellationToken>());
    }
}
