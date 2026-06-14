namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IMissionRepository
{
    Task<Mission?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Mission>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> CountCompletedByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Mission mission, CancellationToken cancellationToken = default);
    void Update(Mission mission);
    void Remove(Mission mission);
}
