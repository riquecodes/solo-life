namespace SoloLife.UnitTests.Features.Users;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Users.GetCurrentUser;
using SoloLife.Domain.Entities;
using Xunit;

public class GetCurrentUserQueryHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly GetCurrentUserQueryHandler _sut;

    public GetCurrentUserQueryHandlerTests()
        => _sut = new GetCurrentUserQueryHandler(_users);

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaFailure()
    {
        _users.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new GetCurrentUserQuery("id-x"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Usuário não encontrado.");
    }

    [Fact]
    public async Task Handle_QuandoUsuarioExiste_RetornaUserDto()
    {
        var user = new User
        {
            Name = "Ana",
            Email = "ana@test.com",
            CurrentLevel = 3,
            CurrentXp = 20,
            CurrentStreak = 4
        };
        _users.GetByIdAsync("id-1", Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.Handle(new GetCurrentUserQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Name.ShouldBe("Ana");
        result.Value.Email.ShouldBe("ana@test.com");
        result.Value.CurrentLevel.ShouldBe(3);
        result.Value.CurrentXp.ShouldBe(20);
        result.Value.CurrentStreak.ShouldBe(4);
    }
}
