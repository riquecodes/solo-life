namespace SoloLife.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class AvatarRepository : IAvatarRepository
{
    private readonly SoloLifeDbContext _context;

    public AvatarRepository(SoloLifeDbContext context) => _context = context;

    public Task<Avatar?> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
        => _context.Avatars.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);

    public void Update(Avatar avatar) => _context.Avatars.Update(avatar);
}
