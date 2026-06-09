namespace SoloLife.UnitTests.Features.Missions;

using NSubstitute;
using Shouldly;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Features.Missions.CreateMission;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;
using Xunit;

public class CreateMissionCommandHandlerTests
{
    private readonly IMissionRepository _missions = Substitute.For<IMissionRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateMissionCommandHandler _sut;

    public CreateMissionCommandHandlerTests()
        => _sut = new CreateMissionCommandHandler(_missions, _unitOfWork);

    [Fact]
    public async Task Handle_ComDadosValidos_CriaMissaoPendenteEPersiste()
    {
        var userId = Guid.NewGuid();

        var result = await _sut.Handle(
            new CreateMissionCommand(userId, "  Correr 5km  ", "  Treino matinal  ", MissionCategory.Health, 50),
            default);

        result.IsSuccess.ShouldBeTrue();
        result.Value!.Title.ShouldBe("Correr 5km");
        result.Value.Description.ShouldBe("Treino matinal");
        result.Value.Category.ShouldBe(MissionCategory.Health);
        result.Value.XpReward.ShouldBe(50);
        result.Value.Status.ShouldBe(MissionStatus.Pending);

        await _missions.Received(1).AddAsync(
            Arg.Is<Mission>(m =>
                m.UserId == userId &&
                m.Title == "Correr 5km" &&
                m.Description == "Treino matinal" &&
                m.Status == MissionStatus.Pending),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
