namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IAchievementRepository
{
    Task<IReadOnlyList<Achievement>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Achievement achievement, CancellationToken cancellationToken = default);
}
