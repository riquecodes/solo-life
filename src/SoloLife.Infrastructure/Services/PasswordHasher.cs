namespace SoloLife.Infrastructure.Services;

using SoloLife.Application.Common.Interfaces;

/// <summary>Stub base — substituir por implementação real (ex.: BCrypt) na fase de implementação.</summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password) => throw new NotImplementedException();
    public bool Verify(string password, string hash) => throw new NotImplementedException();
}
