namespace SoloLife.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class UserRepository : IUserRepository
{
    private readonly SoloLifeDbContext _context;

    public UserRepository(SoloLifeDbContext context) => _context = context;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<User?> GetByRefreshTokenHashAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
        => _context.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await _context.Users.AddAsync(user, cancellationToken);

    public void Update(User user) => _context.Users.Update(user);
}
