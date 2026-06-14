namespace SoloLife.Application.Features.Missions.CompleteMission;

using MediatR;
using SoloLife.Application.Common.Interfaces;
using SoloLife.Application.Common.Results;
using SoloLife.Application.Features.Missions.Dtos;
using SoloLife.Domain.Entities;
using SoloLife.Domain.Enums;
using SoloLife.Domain.Services;

public record CompleteMissionCommand(string Id, string UserId)
    : IRequest<Result<MissionDto>>;

/// <summary>
/// Ponto de orquestração da progressão: ao concluir a missão, aplica XP (subindo de nível),
/// atualiza o streak, grava o histórico e desbloqueia conquistas automaticamente.
/// </summary>
public class CompleteMissionCommandHandler
    : IRequestHandler<CompleteMissionCommand, Result<MissionDto>>
{
    private readonly IMissionRepository _missions;
    private readonly IUserRepository _users;
    private readonly IProgressHistoryRepository _progressHistory;
    private readonly IAchievementRepository _achievements;
    private readonly LevelService _levelService;
    private readonly StreakService _streakService;
    private readonly AchievementService _achievementService;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteMissionCommandHandler(
        IMissionRepository missions,
        IUserRepository users,
        IProgressHistoryRepository progressHistory,
        IAchievementRepository achievements,
        LevelService levelService,
        StreakService streakService,
        AchievementService achievementService,
        IUnitOfWork unitOfWork)
    {
        _missions = missions;
        _users = users;
        _progressHistory = progressHistory;
        _achievements = achievements;
        _levelService = levelService;
        _streakService = streakService;
        _achievementService = achievementService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MissionDto>> Handle(CompleteMissionCommand request, CancellationToken cancellationToken)
    {
        var mission = await _missions.GetByIdAsync(request.Id, cancellationToken);

        // Mensagem genérica de propósito — não revela existência de missão de outro usuário.
        if (mission is null || mission.UserId != request.UserId)
            return Result.Failure<MissionDto>("Missão não encontrada.");

        if (mission.Status == MissionStatus.Completed)
            return Result.Failure<MissionDto>("Missão já concluída.");

        var user = await _users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure<MissionDto>("Usuário não encontrado.");

        var now = DateTime.UtcNow;

        mission.Status = MissionStatus.Completed;
        mission.CompletedAt = now;

        // Progressão: XP (com subida de nível) e streak diário.
        _levelService.AddXp(user, mission.XpReward);
        _streakService.RegisterActivity(user, DateOnly.FromDateTime(now));

        await _progressHistory.AddAsync(new ProgressHistory
        {
            UserId = request.UserId,
            MissionId = mission.Id,
            XpGained = mission.XpReward,
            CreatedAt = now
        }, cancellationToken);

        await UnlockAchievementsAsync(request.UserId, user.CurrentStreak, now, cancellationToken);

        _missions.Update(mission);
        _users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MissionDto.From(mission));
    }

    private async Task UnlockAchievementsAsync(
        string userId, int currentStreak, DateTime now, CancellationToken cancellationToken)
    {
        // +1 inclui a missão recém-concluída (ainda não persistida na contagem).
        var completedMissions = await _missions.CountCompletedByUserAsync(userId, cancellationToken) + 1;
        var existing = await _achievements.GetByUserAsync(userId, cancellationToken);
        var unlockedCodes = existing.Select(a => a.Code).ToHashSet();

        var toUnlock = _achievementService.Evaluate(unlockedCodes, completedMissions, currentStreak);

        foreach (var definition in toUnlock)
        {
            await _achievements.AddAsync(new Achievement
            {
                UserId = userId,
                Code = definition.Code,
                Title = definition.Title,
                Description = definition.Description,
                UnlockedAt = now
            }, cancellationToken);
        }
    }
}
