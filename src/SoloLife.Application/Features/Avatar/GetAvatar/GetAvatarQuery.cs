namespace SoloLife.Application.Features.Avatar.GetAvatar;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Avatar.Dtos;

public record GetAvatarQuery(string UserId)
    : IRequest<Result<AvatarDto>>;

public class GetAvatarQueryHandler
    : IRequestHandler<GetAvatarQuery, Result<AvatarDto>>
{
    private readonly IAvatarRepository _avatars;

    public GetAvatarQueryHandler(IAvatarRepository avatars) => _avatars = avatars;

    public async Task<Result<AvatarDto>> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
    {
        var avatar = await _avatars.GetByUserAsync(request.UserId, cancellationToken);

        if (avatar is null)
            return Result.Failure<AvatarDto>("Avatar não encontrado.");

        return Result.Success(new AvatarDto(
            avatar.CurrentSkin,
            avatar.CurrentBackground,
            avatar.Accessories));
    }
}
