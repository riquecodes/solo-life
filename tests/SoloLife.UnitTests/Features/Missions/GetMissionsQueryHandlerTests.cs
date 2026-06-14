namespace SoloLife.UnitTests.Features.Missions;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Missions.GetMissions;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;
using Xunit;

public class GetMissionsQueryHandlerTests
{
    private readonly IMissionRepository _missions = Substitute.For<IMissionRepository>();
    private readonly GetMissionsQueryHandler _sut;

    public GetMissionsQueryHandlerTests()
        => _sut = new GetMissionsQueryHandler(_missions);

    [Fact]
    public async Task Handle_RetornaMissoesDoUsuarioMapeadas()
    {
        var userId = Guid.NewGuid().ToString();
        var missions = new List<Mission>
        {
            new() { UserId = userId, Title = "A", Category = MissionCategory.Health, XpReward = 10 },
            new() { UserId = userId, Title = "B", Category = MissionCategory.Study, XpReward = 20 }
        };
        _missions.GetByUserAsync(userId, Arg.Any<CancellationToken>()).Returns(missions);

        var result = await _sut.Handle(new GetMissionsQuery(userId), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
        result.Value[0].Title.ShouldBe("A");
        result.Value[1].XpReward.ShouldBe(20);
    }

    [Fact]
    public async Task Handle_QuandoSemMissoes_RetornaListaVazia()
    {
        var userId = Guid.NewGuid().ToString();
        _missions.GetByUserAsync(userId, Arg.Any<CancellationToken>()).Returns(new List<Mission>());

        var result = await _sut.Handle(new GetMissionsQuery(userId), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.ShouldBeEmpty();
    }
}
