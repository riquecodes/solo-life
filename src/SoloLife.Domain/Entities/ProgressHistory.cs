namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;

public class ProgressHistory : Entity
{
    public Guid UserId { get; set; }
    public Guid? MissionId { get; set; }
    public int XpGained { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Mission? Mission { get; set; }
}
