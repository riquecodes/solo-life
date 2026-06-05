namespace SoloLife.Application.Features.Avatar.GetAvatar;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Avatar.Dtos;

public record GetAvatarQuery(Guid UserId)
    : IRequest<Result<AvatarDto>>;

public class GetAvatarQueryHandler
    : IRequestHandler<GetAvatarQuery, Result<AvatarDto>>
{
    public Task<Result<AvatarDto>> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<AvatarDto>("Não implementado."));
}
