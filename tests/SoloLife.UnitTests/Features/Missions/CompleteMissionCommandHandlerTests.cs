namespace SoloLife.UnitTests.Features.Missions;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Missions.CompleteMission;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;
using Xunit;

public class CompleteMissionCommandHandlerTests
{
    private readonly IMissionRepository _missions = Substitute.For<IMissionRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CompleteMissionCommandHandler _sut;

    public CompleteMissionCommandHandlerTests()
        => _sut = new CompleteMissionCommandHandler(_missions, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoMissaoNaoExiste_RetornaFailure()
    {
        _missions.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Mission?)null);

        var result = await _sut.Handle(new CompleteMissionCommand(Guid.NewGuid(), Guid.NewGuid()), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoMissaoDeOutroUsuario_RetornaFailure()
    {
        var mission = new Mission { UserId = Guid.NewGuid() };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, Guid.NewGuid()), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoJaConcluida_RetornaFailureSemPersistir()
    {
        var userId = Guid.NewGuid();
        var mission = new Mission { UserId = userId, Status = MissionStatus.Completed };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, userId), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão já concluída.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoPendenteEDono_MarcaConcluidaEPersiste()
    {
        var userId = Guid.NewGuid();
        var mission = new Mission
        {
            UserId = userId,
            Status = MissionStatus.Pending
        };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, userId), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Status.ShouldBe(MissionStatus.Completed);
        result.Value.CompletedAt.ShouldNotBeNull();

        mission.Status.ShouldBe(MissionStatus.Completed);
        mission.CompletedAt.ShouldNotBeNull();
        _missions.Received(1).Update(mission);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
