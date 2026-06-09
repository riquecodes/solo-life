namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IJwtTokenGenerator
{
    /// <summary>Gera o access token JWT assinado para o usuário.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Gera um refresh token opaco novo (valor bruto + hash a persistir + expiração).</summary>
    RefreshTokenResult GenerateRefreshToken();

    /// <summary>Calcula o hash de um refresh token bruto, para lookup/validação.</summary>
    string HashRefreshToken(string refreshToken);
}

/// <summary>Resultado da geração de refresh token. <see cref="Value"/> vai ao cliente; <see cref="Hash"/> é persistido.</summary>
public sealed record RefreshTokenResult(string Value, string Hash, DateTime ExpiresAt);
