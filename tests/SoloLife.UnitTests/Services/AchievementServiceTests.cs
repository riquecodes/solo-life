namespace SoloLife.UnitTests.Services;

using Shouldly;
using SoloLife.Domain.Services;
using Xunit;

public class AchievementServiceTests
{
    private readonly AchievementService _sut = new();

    [Fact]
    public void Evaluate_PrimeiraMissao_DesbloqueiaFirstMission()
    {
        var unlocked = _sut.Evaluate(new HashSet<string>(), completedMissions: 1, currentStreak: 0);

        unlocked.ShouldContain(a => a.Code == "FIRST_MISSION");
    }

    [Fact]
    public void Evaluate_SemMissoesNemStreak_NaoDesbloqueiaNada()
    {
        var unlocked = _sut.Evaluate(new HashSet<string>(), completedMissions: 0, currentStreak: 0);

        unlocked.ShouldBeEmpty();
    }

    [Fact]
    public void Evaluate_JaDesbloqueada_NaoRepete()
    {
        var already = new HashSet<string> { "FIRST_MISSION" };

        var unlocked = _sut.Evaluate(already, completedMissions: 5, currentStreak: 0);

        unlocked.ShouldNotContain(a => a.Code == "FIRST_MISSION");
    }

    [Theory]
    [InlineData(3, "STREAK_3")]
    [InlineData(7, "STREAK_7")]
    [InlineData(30, "STREAK_30")]
    [InlineData(100, "STREAK_100")]
    public void Evaluate_AtingeMarcoDeStreak_DesbloqueiaConquistaCorrespondente(int streak, string code)
    {
        var unlocked = _sut.Evaluate(new HashSet<string>(), completedMissions: 1, currentStreak: streak);

        unlocked.ShouldContain(a => a.Code == code);
    }

    [Fact]
    public void Evaluate_Streak100_DesbloqueiaTodosOsMarcosDeStreakNaoObtidos()
    {
        var unlocked = _sut.Evaluate(new HashSet<string>(), completedMissions: 1, currentStreak: 100);

        unlocked.Select(a => a.Code).ShouldBe(
            new[] { "FIRST_MISSION", "STREAK_3", "STREAK_7", "STREAK_30", "STREAK_100" },
            ignoreOrder: true);
    }

    [Fact]
    public void Evaluate_100Missoes_DesbloqueiaCenturiao()
    {
        var unlocked = _sut.Evaluate(new HashSet<string>(), completedMissions: 100, currentStreak: 0);

        unlocked.ShouldContain(a => a.Code == "MISSIONS_100");
    }

    [Fact]
    public void Evaluate_DefinicaoTemTituloEDescricao()
    {
        var unlocked = _sut.Evaluate(new HashSet<string>(), completedMissions: 1, currentStreak: 0);

        var first = unlocked.ShouldHaveSingleItem();
        first.Title.ShouldNotBeNullOrWhiteSpace();
        first.Description.ShouldNotBeNullOrWhiteSpace();
    }
}
