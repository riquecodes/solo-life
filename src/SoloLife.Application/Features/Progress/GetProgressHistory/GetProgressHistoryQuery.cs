namespace SoloLife.Application.Features.Progress.GetProgressHistory;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Progress.Dtos;

public record GetProgressHistoryQuery(string UserId)
    : IRequest<Result<IReadOnlyList<ProgressHistoryDto>>>;

public class GetProgressHistoryQueryHandler
    : IRequestHandler<GetProgressHistoryQuery, Result<IReadOnlyList<ProgressHistoryDto>>>
{
    private readonly IProgressHistoryRepository _history;

    public GetProgressHistoryQueryHandler(IProgressHistoryRepository history) => _history = history;

    public async Task<Result<IReadOnlyList<ProgressHistoryDto>>> Handle(GetProgressHistoryQuery request, CancellationToken cancellationToken)
    {
        var entries = await _history.GetByUserAsync(request.UserId, cancellationToken);

        IReadOnlyList<ProgressHistoryDto> dtos = entries
            .Select(e => new ProgressHistoryDto(e.Id, e.MissionId, e.XpGained, e.CreatedAt))
            .ToList();

        return Result.Success(dtos);
    }
}
