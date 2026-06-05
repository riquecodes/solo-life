namespace SoloLife.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Entities;

public class SoloLifeDbContext : DbContext, IUnitOfWork
{
    public SoloLifeDbContext(DbContextOptions<SoloLifeDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Mission> Missions => Set<Mission>();
    public DbSet<Avatar> Avatars => Set<Avatar>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<LifeGoal> LifeGoals => Set<LifeGoal>();
    public DbSet<ProgressHistory> ProgressHistory => Set<ProgressHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SoloLifeDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
