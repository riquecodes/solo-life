namespace SoloLife.Application.Features.Users.Dtos;

public record UserDto(
    string Id,
    string Name,
    string Email,
    int CurrentLevel,
    int CurrentXp,
    int CurrentStreak,
    DateTime CreatedAt);
