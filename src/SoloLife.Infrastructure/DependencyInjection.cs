namespace SoloLife.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Infrastructure.Persistence;
using SoloLife.Infrastructure.Persistence.Repositories;
using SoloLife.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SoloLifeDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("Default"))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SoloLifeDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMissionRepository, MissionRepository>();
        services.AddScoped<IAvatarRepository, AvatarRepository>();
        services.AddScoped<IAchievementRepository, AchievementRepository>();
        services.AddScoped<IProgressHistoryRepository, ProgressHistoryRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
