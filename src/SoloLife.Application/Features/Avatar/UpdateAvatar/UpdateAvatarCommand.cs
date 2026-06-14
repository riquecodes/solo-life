namespace SoloLife.Application.Features.Avatar.UpdateAvatar;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Avatar.Dtos;

public record UpdateAvatarCommand(
    string UserId,
    string CurrentSkin,
    string CurrentBackground,
    List<string> Accessories)
    : IRequest<Result<AvatarDto>>;

public class UpdateAvatarCommandHandler
    : IRequestHandler<UpdateAvatarCommand, Result<AvatarDto>>
{
    private readonly IAvatarRepository _avatars;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAvatarCommandHandler(IAvatarRepository avatars, IUnitOfWork unitOfWork)
    {
        _avatars = avatars;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AvatarDto>> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var avatar = await _avatars.GetByUserAsync(request.UserId, cancellationToken);

        if (avatar is null)
            return Result.Failure<AvatarDto>("Avatar não encontrado.");

        avatar.CurrentSkin = request.CurrentSkin;
        avatar.CurrentBackground = request.CurrentBackground;
        avatar.Accessories = request.Accessories;

        _avatars.Update(avatar);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AvatarDto(
            avatar.CurrentSkin,
            avatar.CurrentBackground,
            avatar.Accessories));
    }
}
