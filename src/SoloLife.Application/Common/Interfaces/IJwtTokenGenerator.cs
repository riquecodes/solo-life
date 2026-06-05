namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
