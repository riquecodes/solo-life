namespace SoloLife.Application.Features.Auth.Refresh;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public record RefreshTokenCommand(string RefreshToken)
    : IRequest<Result<AuthResponse>>;
