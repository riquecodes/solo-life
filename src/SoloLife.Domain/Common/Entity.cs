namespace SoloLife.Domain.Common;

/// <summary>Base para todas as entidades de domínio.</summary>
public abstract class Entity
{
    // Gerado pelo banco (default uuidv7()); o EF lê o valor de volta no INSERT via RETURNING.
    public string Id { get; protected set; } = null!;
}
