namespace SoloLife.Infrastructure.Common;

/// <summary>Retorno da camada de infraestrutura (diretriz 11) — carrega Id/Guid afetado.</summary>
public sealed class InfraResult
{
    public bool Success { get; }
    public Guid Id { get; }
    public string? Error { get; }

    private InfraResult(bool success, Guid id, string? error)
    {
        Success = success;
        Id = id;
        Error = error;
    }

    public static InfraResult Ok(Guid id) => new(true, id, null);
    public static InfraResult Fail(string error) => new(false, Guid.Empty, error);
}
