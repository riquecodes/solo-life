namespace SoloLife.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class MissionRepository : IMissionRepository
{
    private readonly SoloLifeDbContext _context;

    public MissionRepository(SoloLifeDbContext context) => _context = context;

    public Task<Mission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Missions.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Mission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Missions.Where(m => m.UserId == userId).ToListAsync(cancellationToken);

    public async Task AddAsync(Mission mission, CancellationToken cancellationToken = default)
        => await _context.Missions.AddAsync(mission, cancellationToken);

    public void Update(Mission mission) => _context.Missions.Update(mission);

    public void Remove(Mission mission) => _context.Missions.Remove(mission);
}
