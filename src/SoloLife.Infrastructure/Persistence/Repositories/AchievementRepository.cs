namespace SoloLife.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class AchievementRepository : IAchievementRepository
{
    private readonly SoloLifeDbContext _context;

    public AchievementRepository(SoloLifeDbContext context) => _context = context;

    public async Task<IReadOnlyList<Achievement>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        => await _context.Achievements
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Achievement achievement, CancellationToken cancellationToken = default)
        => await _context.Achievements.AddAsync(achievement, cancellationToken);
}
