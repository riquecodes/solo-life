namespace SoloLife.Domain.Services;

using SoloLife.Domain.Entities;

/// <summary>
/// Regras de streak (sequência de dias com missão concluída).
/// Usa <see cref="User.LastActivityDate"/> como referência. +1 em dia consecutivo, reinicia após gap.
/// </summary>
public sealed class StreakService
{
    /// <summary>
    /// Registra uma atividade (missão concluída) em <paramref name="today"/>, atualizando o streak. Muta o usuário.
    /// Mesmo dia: sem alteração. Dia consecutivo: +1. Caso contrário (gap ou primeira atividade): reinicia em 1.
    /// </summary>
    public void RegisterActivity(User user, DateOnly today)
    {
        var last = user.LastActivityDate;

        if (last == today)
            return; // já contou hoje

        user.CurrentStreak = last == today.AddDays(-1)
            ? user.CurrentStreak + 1 // dia consecutivo
            : 1;                     // primeira atividade ou streak quebrado

        user.LastActivityDate = today;
    }

    /// <summary>
    /// Streak efetivo em <paramref name="today"/>: 0 se a última atividade foi antes de ontem (streak quebrado).
    /// </summary>
    public int CurrentStreak(User user, DateOnly today)
    {
        var last = user.LastActivityDate;

        if (last == today || last == today.AddDays(-1))
            return user.CurrentStreak;

        return 0;
    }
}
