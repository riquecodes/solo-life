namespace SoloLife.Application.Features.Progress.Dtos;

public record ProgressDto(
    int CurrentLevel,
    int CurrentXp,
    int XpForNextLevel,
    int RemainingXp);

public record ProgressHistoryDto(
    Guid Id,
    Guid? MissionId,
    int XpGained,
    DateTime CreatedAt);
