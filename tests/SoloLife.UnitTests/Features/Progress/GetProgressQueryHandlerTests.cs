namespace SoloLife.UnitTests.Features.Progress;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Progress.GetProgress;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Services;
using Xunit;

public class GetProgressQueryHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly GetProgressQueryHandler _sut;

    public GetProgressQueryHandlerTests()
        => _sut = new GetProgressQueryHandler(_users, new LevelService());

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaFailure()
    {
        _users.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new GetProgressQuery("id-x"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Usuário não encontrado.");
    }

    [Fact]
    public async Task Handle_QuandoUsuarioExiste_CalculaProgressoComCurvaLinear()
    {
        var user = new User { CurrentLevel = 2, CurrentXp = 30 };
        _users.GetByIdAsync("id-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.Handle(new GetProgressQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.CurrentLevel.ShouldBe(2);
        result.Value.CurrentXp.ShouldBe(30);
        result.Value.XpForNextLevel.ShouldBe(100); // 2 * 50
        result.Value.RemainingXp.ShouldBe(70);      // 100 - 30
    }
}
