namespace SoloLife.UnitTests.Features.Users;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Users.UpdateCurrentUser;
using SoloLife.Domain.Entities;
using Xunit;

public class UpdateCurrentUserCommandHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCurrentUserCommandHandler _sut;

    public UpdateCurrentUserCommandHandlerTests()
        => _sut = new UpdateCurrentUserCommandHandler(_users, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaFailureSemPersistir()
    {
        _users.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new UpdateCurrentUserCommand("id-x", "Novo Nome"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Usuário não encontrado.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoUsuarioExiste_AtualizaNomeComTrimEPersiste()
    {
        var user = new User { Name = "Ana", Email = "ana@test.com" };
        _users.GetByIdAsync("id-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.Handle(new UpdateCurrentUserCommand("id-1", "  Ana Maria  "), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Name.ShouldBe("Ana Maria");
        user.Name.ShouldBe("Ana Maria");
        _users.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
