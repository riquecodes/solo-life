namespace SoloLife.Application.Features.Auth.Login;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public record LoginCommand(string Email, string Password)
    : IRequest<Result<AuthResponse>>;
