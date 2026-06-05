namespace SoloLife.Application.Features.Progress.GetProgress;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Progress.Dtos;

public record GetProgressQuery(Guid UserId)
    : IRequest<Result<ProgressDto>>;

public class GetProgressQueryHandler
    : IRequestHandler<GetProgressQuery, Result<ProgressDto>>
{
    public Task<Result<ProgressDto>> Handle(GetProgressQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<ProgressDto>("Não implementado."));
}
