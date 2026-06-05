namespace SoloLife.Application.Features.Users.Dtos;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    int CurrentLevel,
    int CurrentXp,
    int CurrentStreak,
    DateTime CreatedAt);
