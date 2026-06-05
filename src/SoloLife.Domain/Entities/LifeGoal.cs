namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;

public class LifeGoal : Entity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public User? User { get; set; }
}
