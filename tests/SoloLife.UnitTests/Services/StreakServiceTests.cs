namespace SoloLife.UnitTests.Services;

using Shouldly;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Services;
using Xunit;

public class StreakServiceTests
{
    private readonly StreakService _sut = new();
    private static readonly DateOnly Today = new(2026, 6, 13);

    [Fact]
    public void RegisterActivity_PrimeiraAtividade_IniciaStreakEm1()
    {
        var user = new User { CurrentStreak = 0, LastActivityDate = null };

        _sut.RegisterActivity(user, Today);

        user.CurrentStreak.ShouldBe(1);
        user.LastActivityDate.ShouldBe(Today);
    }

    [Fact]
    public void RegisterActivity_DiaConsecutivo_IncrementaStreak()
    {
        var user = new User { CurrentStreak = 3, LastActivityDate = Today.AddDays(-1) };

        _sut.RegisterActivity(user, Today);

        user.CurrentStreak.ShouldBe(4);
        user.LastActivityDate.ShouldBe(Today);
    }

    [Fact]
    public void RegisterActivity_MesmoDia_NaoAltera()
    {
        var user = new User { CurrentStreak = 5, LastActivityDate = Today };

        _sut.RegisterActivity(user, Today);

        user.CurrentStreak.ShouldBe(5);
        user.LastActivityDate.ShouldBe(Today);
    }

    [Fact]
    public void RegisterActivity_AposGap_ReiniciaStreakEm1()
    {
        var user = new User { CurrentStreak = 9, LastActivityDate = Today.AddDays(-3) };

        _sut.RegisterActivity(user, Today);

        user.CurrentStreak.ShouldBe(1);
        user.LastActivityDate.ShouldBe(Today);
    }

    [Fact]
    public void CurrentStreak_SemAtividade_RetornaZero()
    {
        var user = new User { CurrentStreak = 0, LastActivityDate = null };

        _sut.CurrentStreak(user, Today).ShouldBe(0);
    }

    [Fact]
    public void CurrentStreak_AtividadeHoje_RetornaStreakArmazenado()
    {
        var user = new User { CurrentStreak = 7, LastActivityDate = Today };

        _sut.CurrentStreak(user, Today).ShouldBe(7);
    }

    [Fact]
    public void CurrentStreak_AtividadeOntem_AindaValido()
    {
        var user = new User { CurrentStreak = 7, LastActivityDate = Today.AddDays(-1) };

        _sut.CurrentStreak(user, Today).ShouldBe(7);
    }

    [Fact]
    public void CurrentStreak_AtividadeAntesDeOntem_RetornaZero()
    {
        var user = new User { CurrentStreak = 7, LastActivityDate = Today.AddDays(-2) };

        _sut.CurrentStreak(user, Today).ShouldBe(0);
    }
}
