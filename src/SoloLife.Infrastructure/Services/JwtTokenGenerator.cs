namespace SoloLife.Infrastructure.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;
    private readonly int _refreshTokenDays;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        var jwt = configuration.GetSection("Jwt");
        _secret = jwt["Secret"] ?? throw new InvalidOperationException("Jwt:Secret não configurado.");
        _issuer = jwt["Issuer"] ?? "SoloLife";
        _audience = jwt["Audience"] ?? "SoloLifeApp";
        _accessTokenMinutes = int.TryParse(jwt["AccessTokenMinutes"], out var m) ? m : 60;
        _refreshTokenDays = int.TryParse(jwt["RefreshTokenDays"], out var d) ? d : 7;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            // Emitido explicitamente para que ApiControllerBase.CurrentUserId resolva independente do mapeamento de claims.
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshTokenResult GenerateRefreshToken()
    {
        var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return new RefreshTokenResult(value, HashRefreshToken(value), DateTime.UtcNow.AddDays(_refreshTokenDays));
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(bytes);
    }
}
