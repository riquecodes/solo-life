namespace SoloLife.UnitTests.Features.Avatar;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Avatar.GetAvatar;
using SoloLife.Domain.Entities;
using Xunit;

public class GetAvatarQueryHandlerTests
{
    private readonly IAvatarRepository _avatars = Substitute.For<IAvatarRepository>();
    private readonly GetAvatarQueryHandler _sut;

    public GetAvatarQueryHandlerTests()
        => _sut = new GetAvatarQueryHandler(_avatars);

    [Fact]
    public async Task Handle_QuandoAvatarNaoExiste_RetornaFailure()
    {
        _avatars.GetByUserAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Avatar?)null);

        var result = await _sut.Handle(new GetAvatarQuery("id-x"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Avatar não encontrado.");
    }

    [Fact]
    public async Task Handle_QuandoAvatarExiste_RetornaAvatarDto()
    {
        var avatar = new Avatar
        {
            UserId = "id-1",
            CurrentSkin = "skin-a",
            CurrentBackground = "bg-a",
            Accessories = new List<string> { "hat" }
        };
        _avatars.GetByUserAsync("id-1", Arg.Any<CancellationToken>()).Returns(avatar);

        var result = await _sut.Handle(new GetAvatarQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.CurrentSkin.ShouldBe("skin-a");
        result.Value.CurrentBackground.ShouldBe("bg-a");
        result.Value.Accessories.ShouldContain("hat");
    }
}
