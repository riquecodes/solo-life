namespace SoloLife.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class ProgressHistoryRepository : IProgressHistoryRepository
{
    private readonly SoloLifeDbContext _context;

    public ProgressHistoryRepository(SoloLifeDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProgressHistory>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        => await _context.ProgressHistory
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ProgressHistory entry, CancellationToken cancellationToken = default)
        => await _context.ProgressHistory.AddAsync(entry, cancellationToken);
}
