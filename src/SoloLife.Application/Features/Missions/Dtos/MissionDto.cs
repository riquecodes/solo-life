namespace SoloLife.Application.Features.Missions.Dtos;

using SoloLife.Domain.Enums;

public record MissionDto(
    Guid Id,
    string Title,
    string Description,
    MissionCategory Category,
    int XpReward,
    MissionStatus Status,
    DateTime CreatedAt,
    DateTime? CompletedAt);
