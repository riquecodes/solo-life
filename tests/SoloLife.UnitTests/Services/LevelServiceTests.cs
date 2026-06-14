namespace SoloLife.UnitTests.Services;

using Shouldly;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Services;
using Xunit;

public class LevelServiceTests
{
    private readonly LevelService _sut = new();

    [Theory]
    [InlineData(1, 50)]
    [InlineData(2, 100)]
    [InlineData(3, 150)]
    [InlineData(10, 500)]
    public void XpForNextLevel_SegueCurvaLinear(int level, int expected)
        => _sut.XpForNextLevel(level).ShouldBe(expected);

    [Fact]
    public void RemainingXp_RetornaQuantoFaltaParaProximoNivel()
    {
        var user = new User { CurrentLevel = 2, CurrentXp = 30 };

        _sut.RemainingXp(user).ShouldBe(70); // 100 - 30
    }

    [Fact]
    public void AddXp_SemSubirNivel_AcumulaXp()
    {
        var user = new User { CurrentLevel = 1, CurrentXp = 10 };

        _sut.AddXp(user, 20);

        user.CurrentLevel.ShouldBe(1);
        user.CurrentXp.ShouldBe(30);
    }

    [Fact]
    public void AddXp_AoAtingirLimiar_SobeDeNivelECarregaExcedente()
    {
        var user = new User { CurrentLevel = 1, CurrentXp = 40 };

        _sut.AddXp(user, 20); // 60 >= 50 -> nível 2, sobra 10

        user.CurrentLevel.ShouldBe(2);
        user.CurrentXp.ShouldBe(10);
    }

    [Fact]
    public void AddXp_ComMuitoXp_SobeMultiplosNiveis()
    {
        var user = new User { CurrentLevel = 1, CurrentXp = 0 };

        // 50 (lv1) + 100 (lv2) = 150 para chegar ao nível 3; +20 de sobra
        _sut.AddXp(user, 170);

        user.CurrentLevel.ShouldBe(3);
        user.CurrentXp.ShouldBe(20);
    }

    [Fact]
    public void AddXp_ExatamenteNoLimiar_SobeNivelComXpZerado()
    {
        var user = new User { CurrentLevel = 1, CurrentXp = 0 };

        _sut.AddXp(user, 50);

        user.CurrentLevel.ShouldBe(2);
        user.CurrentXp.ShouldBe(0);
    }

    [Fact]
    public void AddXp_ComValorNaoPositivo_NaoAltera()
    {
        var user = new User { CurrentLevel = 2, CurrentXp = 30 };

        _sut.AddXp(user, 0);

        user.CurrentLevel.ShouldBe(2);
        user.CurrentXp.ShouldBe(30);
    }
}
