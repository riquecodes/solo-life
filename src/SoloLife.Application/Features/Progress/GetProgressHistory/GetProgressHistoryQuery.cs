namespace SoloLife.Application.Features.Progress.GetProgressHistory;

using MediatR;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Progress.Dtos;

public record GetProgressHistoryQuery(Guid UserId)
    : IRequest<Result<IReadOnlyList<ProgressHistoryDto>>>;

public class GetProgressHistoryQueryHandler
    : IRequestHandler<GetProgressHistoryQuery, Result<IReadOnlyList<ProgressHistoryDto>>>
{
    public Task<Result<IReadOnlyList<ProgressHistoryDto>>> Handle(GetProgressHistoryQuery request, CancellationToken cancellationToken)
        => Task.FromResult(Result.Failure<IReadOnlyList<ProgressHistoryDto>>("Não implementado."));
}
