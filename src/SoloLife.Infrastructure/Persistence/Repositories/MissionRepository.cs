namespace SoloLife.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;

public class MissionRepository : IMissionRepository
{
    private readonly SoloLifeDbContext _context;

    public MissionRepository(SoloLifeDbContext context) => _context = context;

    public Task<Mission?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _context.Missions.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Mission>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        => await _context.Missions.Where(m => m.UserId == userId).ToListAsync(cancellationToken);

    public Task<int> CountCompletedByUserAsync(string userId, CancellationToken cancellationToken = default)
        => _context.Missions.CountAsync(
            m => m.UserId == userId && m.Status == MissionStatus.Completed, cancellationToken);

    public async Task AddAsync(Mission mission, CancellationToken cancellationToken = default)
        => await _context.Missions.AddAsync(mission, cancellationToken);

    public void Update(Mission mission) => _context.Missions.Update(mission);

    public void Remove(Mission mission) => _context.Missions.Remove(mission);
}
