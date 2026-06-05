namespace SoloLife.Application.Features.Avatar.UpdateAvatar;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Avatar.Dtos;

public record UpdateAvatarCommand(
    Guid UserId,
    string CurrentSkin,
    string CurrentBackground,
    List<string> Accessories)
    : IRequest<Result<AvatarDto>>;

public class UpdateAvatarCommandHandler
    : IRequestHandler<UpdateAvatarCommand, Result<AvatarDto>>
{
    public Task<Result<AvatarDto>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<AvatarDto>("Não implementado."));
}
