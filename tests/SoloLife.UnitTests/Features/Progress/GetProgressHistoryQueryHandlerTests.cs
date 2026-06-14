namespace SoloLife.UnitTests.Features.Progress;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Progress.GetProgressHistory;
using SoloLife.Domain.Entities;
using Xunit;

public class GetProgressHistoryQueryHandlerTests
{
    private readonly IProgressHistoryRepository _history = Substitute.For<IProgressHistoryRepository>();
    private readonly GetProgressHistoryQueryHandler _sut;

    public GetProgressHistoryQueryHandlerTests()
        => _sut = new GetProgressHistoryQueryHandler(_history);

    [Fact]
    public async Task Handle_SemHistorico_RetornaListaVazia()
    {
        _history.GetByUserAsync("id-1", Arg.Any<CancellationToken>())
            .Returns(new List<ProgressHistory>());

        var result = await _sut.Handle(new GetProgressHistoryQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_ComHistorico_MapeiaParaDto()
    {
        var entries = new List<ProgressHistory>
        {
            new() { UserId = "id-1", MissionId = "m-1", XpGained = 20 },
            new() { UserId = "id-1", MissionId = null, XpGained = 10 }
        };
        _history.GetByUserAsync("id-1", Arg.Any<CancellationToken>()).Returns(entries);

        var result = await _sut.Handle(new GetProgressHistoryQuery("id-1"), default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(2);
        result.Value[0].MissionId.ShouldBe("m-1");
        result.Value[0].XpGained.ShouldBe(20);
        result.Value[1].MissionId.ShouldBeNull();
    }
}
