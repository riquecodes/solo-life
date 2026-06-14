namespace SoloLife.Domain.Services;

/// <summary>Definição de uma conquista do catálogo (ainda não persistida).</summary>
public sealed record AchievementDefinition(string Code, string Title, string Description);

/// <summary>
/// Avalia o catálogo de conquistas do MVP e decide quais desbloquear (insert-on-unlock).
/// Conquistas são desbloqueadas automaticamente conforme contadores de missões e streak.
/// </summary>
public sealed class AchievementService
{
    /// <summary>
    /// Retorna as conquistas a desbloquear dado o estado atual, ignorando as já desbloqueadas
    /// (presentes em <paramref name="unlockedCodes"/>).
    /// </summary>
    public IReadOnlyList<AchievementDefinition> Evaluate(
        IReadOnlySet<string> unlockedCodes,
        int completedMissions,
        int currentStreak)
    {
        var unlocked = new List<AchievementDefinition>();

        void Check(string code, string title, string description, bool condition)
        {
            if (condition && !unlockedCodes.Contains(code))
                unlocked.Add(new AchievementDefinition(code, title, description));
        }

        Check("FIRST_MISSION", "Primeira Missão", "Concluiu sua primeira missão.", completedMissions >= 1);
        Check("MISSIONS_100", "Centurião", "Concluiu 100 missões.", completedMissions >= 100);
        Check("STREAK_3", "Trinca", "Manteve um streak de 3 dias.", currentStreak >= 3);
        Check("STREAK_7", "Semana de Fogo", "Manteve um streak de 7 dias.", currentStreak >= 7);
        Check("STREAK_30", "Mês Imparável", "Manteve um streak de 30 dias.", currentStreak >= 30);
        Check("STREAK_100", "Lenda", "Manteve um streak de 100 dias.", currentStreak >= 100);

        return unlocked;
    }
}
