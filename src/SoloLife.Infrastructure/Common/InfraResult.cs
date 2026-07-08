namespace SoloLife.Infrastructure.Common;

/// <summary>Retorno da camada de infraestrutura (diretriz 11) — carrega o Id afetado.</summary>
public sealed class InfraResult
{
    public bool Success { get; }
    public string Id { get; }
    public string? Error { get; }

    private InfraResult(bool success, string id, string? error)
    {
        Success = success;
        Id = id;
        Error = error;
    }

    public static InfraResult Ok(string id) => new(true, id, null);
    public static InfraResult Fail(string error) => new(false, string.Empty, error);
}
