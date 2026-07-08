namespace SoloLife.UnitTests.Features.Streak;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Streak.GetStreak;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Services;
using Xunit;

public class GetStreakQueryHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly GetStreakQueryHandler _sut;

    public GetStreakQueryHandlerTests()
        => _sut = new GetStreakQueryHandler(_users, new StreakService());

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaFailure()
    {
        _users.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new GetStreakQuery("id-x"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Usuário não encontrado.");
    }

    [Fact]
    public async Task Handle_ComAtividadeRecente_RetornaStreakArmazenado()
    {
        var user = new User
        {
            CurrentStreak = 5,
            LastActivityDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };
        _users.GetByIdAsync("id-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.Handle(new GetStreakQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.CurrentStreak.ShouldBe(5);
    }

    [Fact]
    public async Task Handle_ComStreakQuebrado_RetornaZero()
    {
        var user = new User
        {
            CurrentStreak = 5,
            LastActivityDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-5)
        };
        _users.GetByIdAsync("id-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.Handle(new GetStreakQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.CurrentStreak.ShouldBe(0);
    }
}
