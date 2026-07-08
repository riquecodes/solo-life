namespace SoloLife.Domain.Entities;

using SoloLife.Domain.Common;

public class Avatar : Entity
{
    public string UserId { get; set; } = string.Empty;
    public string CurrentSkin { get; set; } = string.Empty;
    public string CurrentBackground { get; set; } = string.Empty;
    public List<string> Accessories { get; set; } = new();

    public User? User { get; set; }
}
