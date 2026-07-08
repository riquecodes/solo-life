namespace SoloLife.Api.Controllers;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Common.Results;

// Sem [Route] na base: cada controller concreto declara o seu (auth, users, ...).
// Um [Route] aqui somaria um template extra ("/[controller]") e exporia rotas duplicadas.
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>Id do usuário autenticado, extraído do token JWT.</summary>
    // O gerador emite o claim "id" explicitamente (e Program.cs define NameClaimType = "id");
    // lê "id" primeiro para não depender do mapeamento de claims de entrada. NameIdentifier é fallback.
    protected string CurrentUserId =>
        User.FindFirstValue("id")
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? string.Empty;

    // Camada API é a única a retornar IActionResult (diretriz 9). Mapeia Result<T>.
    protected IActionResult ToActionResult<T>(Result<T> result)
        => result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });

    protected IActionResult ToActionResult(Result result)
        => result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
}
