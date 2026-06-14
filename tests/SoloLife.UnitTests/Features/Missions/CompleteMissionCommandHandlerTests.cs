namespace SoloLife.UnitTests.Features.Missions;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Missions.CompleteMission;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;
using SoloLife.Domain.Services;
using Xunit;

public class CompleteMissionCommandHandlerTests
{
    private readonly IMissionRepository _missions = Substitute.For<IMissionRepository>();
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IProgressHistoryRepository _progressHistory = Substitute.For<IProgressHistoryRepository>();
    private readonly IAchievementRepository _achievements = Substitute.For<IAchievementRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CompleteMissionCommandHandler _sut;

    public CompleteMissionCommandHandlerTests()
        => _sut = new CompleteMissionCommandHandler(
            _missions,
            _users,
            _progressHistory,
            _achievements,
            new LevelService(),
            new StreakService(),
            new AchievementService(),
            _unitOfWork);

    [Fact]
    public async Task Handle_QuandoMissaoNaoExiste_RetornaFailure()
    {
        _missions.GetByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((Mission?)null);

        var result = await _sut.Handle(new CompleteMissionCommand("m-1", "u-1"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoMissaoDeOutroUsuario_RetornaFailure()
    {
        var mission = new Mission { UserId = "outro-user" };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, "u-1"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão não encontrada.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoJaConcluida_RetornaFailureSemPersistir()
    {
        var mission = new Mission { UserId = "u-1", Status = MissionStatus.Completed };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, "u-1"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Missão já concluída.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoUsuarioNaoExiste_RetornaFailureSemPersistir()
    {
        var mission = new Mission { UserId = "u-1", Status = MissionStatus.Pending };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);
        _users.GetByIdAsync("u-1", Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, "u-1"), default);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Usuário não encontrado.");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoPendenteEDono_ConcluiAplicaXpAtualizaStreakEGravaHistorico()
    {
        var mission = new Mission { UserId = "u-1", Status = MissionStatus.Pending, XpReward = 20 };
        var user = new User { CurrentLevel = 1, CurrentXp = 10, CurrentStreak = 0, LastActivityDate = null };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);
        _users.GetByIdAsync("u-1", Arg.Any<CancellationToken>()).Returns(user);
        _missions.CountCompletedByUserAsync("u-1", Arg.Any<CancellationToken>()).Returns(0);
        _achievements.GetByUserAsync("u-1", Arg.Any<CancellationToken>()).Returns(new List<Achievement>());

        var result = await _sut.Handle(new CompleteMissionCommand(mission.Id, "u-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Status.ShouldBe(MissionStatus.Completed);

        mission.Status.ShouldBe(MissionStatus.Completed);
        mission.CompletedAt.ShouldNotBeNull();

        user.CurrentXp.ShouldBe(30);     // 10 + 20, sem subir de nível
        user.CurrentLevel.ShouldBe(1);
        user.CurrentStreak.ShouldBe(1);  // primeira atividade
        user.LastActivityDate.ShouldNotBeNull();

        await _progressHistory.Received(1).AddAsync(
            Arg.Is<ProgressHistory>(p => p.UserId == "u-1" && p.MissionId == mission.Id && p.XpGained == 20),
            Arg.Any<CancellationToken>());
        _missions.Received(1).Update(mission);
        _users.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NaPrimeiraConclusao_DesbloqueiaConquistaPrimeiraMissao()
    {
        var mission = new Mission { UserId = "u-1", Status = MissionStatus.Pending, XpReward = 20 };
        var user = new User { CurrentLevel = 1, CurrentXp = 0, CurrentStreak = 0 };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);
        _users.GetByIdAsync("u-1", Arg.Any<CancellationToken>()).Returns(user);
        _missions.CountCompletedByUserAsync("u-1", Arg.Any<CancellationToken>()).Returns(0);
        _achievements.GetByUserAsync("u-1", Arg.Any<CancellationToken>()).Returns(new List<Achievement>());

        await _sut.Handle(new CompleteMissionCommand(mission.Id, "u-1"), default);

        await _achievements.Received(1).AddAsync(
            Arg.Is<Achievement>(a => a.Code == "FIRST_MISSION" && a.UserId == "u-1" && a.UnlockedAt != null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoConquistaJaDesbloqueada_NaoDuplica()
    {
        var mission = new Mission { UserId = "u-1", Status = MissionStatus.Pending, XpReward = 20 };
        var user = new User { CurrentLevel = 1, CurrentXp = 0, CurrentStreak = 0 };
        _missions.GetByIdAsync(mission.Id, Arg.Any<CancellationToken>()).Returns(mission);
        _users.GetByIdAsync("u-1", Arg.Any<CancellationToken>()).Returns(user);
        _missions.CountCompletedByUserAsync("u-1", Arg.Any<CancellationToken>()).Returns(4);
        _achievements.GetByUserAsync("u-1", Arg.Any<CancellationToken>()).Returns(
            new List<Achievement> { new() { UserId = "u-1", Code = "FIRST_MISSION" } });

        await _sut.Handle(new CompleteMissionCommand(mission.Id, "u-1"), default);

        await _achievements.DidNotReceive().AddAsync(
            Arg.Is<Achievement>(a => a.Code == "FIRST_MISSION"), Arg.Any<CancellationToken>());
    }
}
