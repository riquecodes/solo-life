namespace SoloLife.UnitTests.Features.Avatar;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Avatar.UpdateAvatar;
using SoloLife.Domain.Entities;
using Xunit;

public class UpdateAvatarCommandHandlerTests
{
    private readonly IAvatarRepository _avatars = Substitute.For<IAvatarRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateAvatarCommandHandler _sut;

    public UpdateAvatarCommandHandlerTests()
        => _sut = new UpdateAvatarCommandHandler(_avatars, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoAvatarNaoExiste_RetornaFailureSemPersistir()
    {
        _avatars.GetByUserAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Avatar?)null);

        var result = await _sut.Handle(
            new UpdateAvatarCommand("id-x", "skin", "bg", new List<string>()), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Avatar não encontrado.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoAvatarExiste_AtualizaCamposEPersiste()
    {
        var avatar = new Avatar { UserId = "id-1", CurrentSkin = "old", CurrentBackground = "old-bg" };
        _avatars.GetByUserAsync("id-1", Arg.Any<CancellationToken>()).Returns(avatar);

        var result = await _sut.Handle(
            new UpdateAvatarCommand("id-1", "new-skin", "new-bg", new List<string> { "watch" }), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.CurrentSkin.ShouldBe("new-skin");
        result.Value.CurrentBackground.ShouldBe("new-bg");
        result.Value.Accessories.ShouldContain("watch");
        avatar.CurrentSkin.ShouldBe("new-skin");
        avatar.Accessories.ShouldContain("watch");
        _avatars.Received(1).Update(avatar);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
