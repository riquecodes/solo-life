namespace SoloLife.Domain.Services;

using SoloLife.Domain.Entities;

/// <summary>
/// Regras de XP e nível (diretriz: backend é a verdade absoluta da progressão).
/// Curva linear do MVP: sair do nível N para N+1 custa N*50 XP.
/// <see cref="User.CurrentXp"/> é relativo ao nível atual — reseta ao subir, carregando o excedente.
/// </summary>
public sealed class LevelService
{
    public const int XpPerLevelStep = 50;

    /// <summary>XP necessário para sair do nível informado e ir ao próximo.</summary>
    public int XpForNextLevel(int currentLevel)
        => currentLevel * XpPerLevelStep;

    /// <summary>XP que ainda falta para o usuário subir de nível.</summary>
    public int RemainingXp(User user)
        => XpForNextLevel(user.CurrentLevel) - user.CurrentXp;

    /// <summary>Adiciona XP ao usuário, subindo de nível e carregando o excedente. Muta o usuário.</summary>
    public void AddXp(User user, int xp)
    {
        if (xp <= 0)
            return;

        user.CurrentXp += xp;

        while (user.CurrentXp >= XpForNextLevel(user.CurrentLevel))
        {
            user.CurrentXp -= XpForNextLevel(user.CurrentLevel);
            user.CurrentLevel++;
        }
    }
}
