namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;

public class ProgressHistory : Entity
{
    public string UserId { get; set; } = string.Empty;
    public string? MissionId { get; set; }
    public int XpGained { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Mission? Mission { get; set; }
}
