namespace SoloLife.Application.Features.Auth.Register;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Auth.Dtos;

public record RegisterCommand(string Name, string Email, string Password)
    : IRequest<Result<AuthResponse>>;
