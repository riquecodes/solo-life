namespace SoloLife.Infrastructure.Services;

using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

/// <summary>Stub base — substituir por geração real de JWT na fase de implementação.</summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    public string GenerateAccessToken(User user) => throw new NotImplementedException();
    public string GenerateRefreshToken() => throw new NotImplementedException();
}
