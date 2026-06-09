namespace SoloLife.UnitTests.Features.Missions;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Missions.DeleteMission;
using SoloLife.Domain.Entities;
using Xunit;

public class DeleteMissionCommandHandlerTests
{
    private readonly IMissionRepository _missions = Substitute.For<IMissionRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteMissionCommandHandler _sut;

    public DeleteMissionCommandHandlerTests()
        => _sut = new DeleteMissionCommandHandler(_missions, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoMissaoNaoExiste_RetornaFailureSemRemover()
    {
        _missions.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Mission?)null);

        var result = await _sut.Handle(new DeleteMissionCommand(Guid.NewGuid(), Guid.NewGuid()), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        _missions.DidNotReceive().Remove(Arg.Any<Mission>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoMissaoDeOutroUsuario_RetornaFailureSemRemover()
    {
        var mission = new Mission { UserId = Guid.NewGuid() };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new DeleteMissionCommand(mission.Id, Guid.NewGuid()), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        _missions.DidNotReceive().Remove(Arg.Any<Mission>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoDono_RemoveEPersiste()
    {
        var userId = Guid.NewGuid();
        var mission = new Mission { UserId = userId };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new DeleteMissionCommand(mission.Id, userId), default);

        result.IsSuccess.ShouldBeTrue();
        _missions.Received(1).Remove(mission);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
