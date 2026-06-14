namespace SoloLife.Application.Features.Missions.Dtos;

using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;

public record MissionDto(
    string Id,
    string Title,
    string Description,
    MissionCategory Category,
    int XpReward,
    MissionStatus Status,
    DateTime CreatedAt,
    DateTime? CompletedAt)
{
    public static MissionDto From(Mission mission) => new(
        mission.Id,
        mission.Title,
        mission.Description,
        mission.Category,
        mission.XpReward,
        mission.Status,
        mission.CreatedAt,
        mission.CompletedAt);
}
