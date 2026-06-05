namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;

public class Achievement : Entity
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? UnlockedAt { get; set; }

    public User? User { get; set; }
}
