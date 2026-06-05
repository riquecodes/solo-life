namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public int CurrentLevel { get; set; } = 1;
    public int CurrentXp { get; set; }
    public int CurrentStreak { get; set; }

    public Avatar? Avatar { get; set; }
    public ICollection<Mission> Missions { get; set; } = new List<Mission>();
    public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
    public ICollection<LifeGoal> LifeGoals { get; set; } = new List<LifeGoal>();
    public ICollection<ProgressHistory> ProgressHistory { get; set; } = new List<ProgressHistory>();
}
