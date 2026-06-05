namespace SoloLife.Application.Features.Avatar.Dtos;

public record AvatarDto(
    string CurrentSkin,
    string CurrentBackground,
    IReadOnlyList<string> Accessories);
