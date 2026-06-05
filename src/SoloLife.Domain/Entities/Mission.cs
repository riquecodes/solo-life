namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;
using SoloLife.Domain.Enums;

public class Mission : Entity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MissionCategory Category { get; set; }
    public int XpReward { get; set; }
    public MissionStatus Status { get; set; } = MissionStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public User? User { get; set; }
}
