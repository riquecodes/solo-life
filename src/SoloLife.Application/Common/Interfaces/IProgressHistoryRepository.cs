namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IProgressHistoryRepository
{
    Task<IReadOnlyList<ProgressHistory>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(ProgressHistory entry, CancellationToken cancellationToken = default);
}
