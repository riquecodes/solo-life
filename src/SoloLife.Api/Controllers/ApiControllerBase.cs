namespace SoloLife.Api.Controllers;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SoloLife.Application.Common.Results;

[ApiController]
[Route("[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>Id do usuário autenticado, extraído do token JWT.</summary>
    protected string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    // Camada API é a única a retornar IActionResult (diretriz 9). Mapeia Result<T>.
    protected IActionResult ToActionResult<T>(Result<T> result)
        => result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });

    protected IActionResult ToActionResult(Result result)
        => result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
}
