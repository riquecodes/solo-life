namespace SoloLife.Application.Common.Interfaces;

using SoloLife.Domain.Entities;

public interface IMissionRepository
{
    Task<Mission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Mission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Mission mission, CancellationToken cancellationToken = default);
    void Update(Mission mission);
    void Remove(Mission mission);
}
