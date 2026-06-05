namespace SoloLife.Domain.Common;

/// <summary>Base para todas as entidades de domínio.</summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
