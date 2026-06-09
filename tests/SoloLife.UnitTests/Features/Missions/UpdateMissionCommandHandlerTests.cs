namespace SoloLife.UnitTests.Features.Missions;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Missions.UpdateMission;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;
using Xunit;

public class UpdateMissionCommandHandlerTests
{
    private readonly IMissionRepository _missions = Substitute.For<IMissionRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateMissionCommandHandler _sut;

    public UpdateMissionCommandHandlerTests()
        => _sut = new UpdateMissionCommandHandler(_missions, _unitOfWork);

    [Fact]
    public async Task Handle_QuandoMissaoNaoExiste_RetornaFailureSemPersistir()
    {
        _missions.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Mission?)null);

        var result = await _sut.Handle(
            new UpdateMissionCommand(Guid.NewGuid(), Guid.NewGuid(), "T", "D", MissionCategory.Study, 10),
            default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoMissaoDeOutroUsuario_RetornaFailureSemPersistir()
    {
        var mission = new Mission { UserId = Guid.NewGuid() };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(
            new UpdateMissionCommand(mission.Id, Guid.NewGuid(), "T", "D", MissionCategory.Study, 10),
            default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        _missions.DidNotReceive().Update(Arg.Any<Mission>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoDono_AtualizaCamposEPersiste()
    {
        var userId = Guid.NewGuid();
        var mission = new Mission
        {
            UserId = userId,
            Title = "Antigo",
            Description = "Antiga",
            Category = MissionCategory.Health,
            XpReward = 10
        };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(
            new UpdateMissionCommand(mission.Id, userId, "  Novo  ", "  Nova  ", MissionCategory.Study, 99),
            default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Title.ShouldBe("Novo");
        result.Value.Description.ShouldBe("Nova");
        result.Value.Category.ShouldBe(MissionCategory.Study);
        result.Value.XpReward.ShouldBe(99);

        mission.Title.ShouldBe("Novo");
        _missions.Received(1).Update(mission);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
