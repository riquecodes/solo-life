namespace SoloLife.UnitTests.Features.Achievements;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Achievements.GetAchievements;
using SoloLife.Domain.Entities;
using Xunit;

public class GetAchievementsQueryHandlerTests
{
    private readonly IAchievementRepository _achievements = Substitute.For<IAchievementRepository>();
    private readonly GetAchievementsQueryHandler _sut;

    public GetAchievementsQueryHandlerTests()
        => _sut = new GetAchievementsQueryHandler(_achievements);

    [Fact]
    public async Task Handle_SemConquistas_RetornaListaVazia()
    {
        _achievements.GetByUserAsync("id-1", Arg.Any<CancellationToken>())
            .Returns(new List<Achievement>());

        var result = await _sut.Handle(new GetAchievementsQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_ComConquistas_MapeiaParaDto()
    {
        var unlockedAt = DateTime.UtcNow;
        var list = new List<Achievement>
        {
            new() { UserId = "id-1", Code = "FIRST_MISSION", Title = "Primeira Missão", Description = "...", UnlockedAt = unlockedAt }
        };
        _achievements.GetByUserAsync("id-1", Arg.Any<CancellationToken>()).Returns(list);

        var result = await _sut.Handle(new GetAchievementsQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        var dto = result.Value!.ShouldHaveSingleItem();
        dto.Code.ShouldBe("FIRST_MISSION");
        dto.Title.ShouldBe("Primeira Missão");
        dto.UnlockedAt.ShouldBe(unlockedAt);
    }
}
