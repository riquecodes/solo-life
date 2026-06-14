namespace SoloLife.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Domain.Common;
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

        // Id gerado pelo banco (default uuidv7()): o EF omite a coluna no INSERT e lê o valor de volta via RETURNING.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(Entity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType)
                .Property(nameof(Entity.Id))
                .HasDefaultValueSql("(uuidv7())::text");
        }

        base.OnModelCreating(modelBuilder);
    }
}
