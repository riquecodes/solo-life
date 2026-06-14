---
last_updated: "2026-06-13T00:00:00"
---

# Histórico de Sessões

## 2026-06-13 — Implementação dos handlers restantes + fluxo de gamificação (TDD)

**Objetivo:** implementar os 8 handlers stub e os fluxos completos da aplicação seguindo os padrões existentes e TDD.

**Decisões (alinhadas com o usuário):**
- Escopo: fluxo completo de gamificação (não só CRUD).
- `User.CurrentXp` é relativo ao nível (reseta ao subir, carrega excedente).
- Streak rastreado por novo campo `User.LastActivityDate` (date) + migration V003.
- Avatar criado no Register; conquistas insert-on-unlock.

**Entregue:**
- Domain services puros em `Domain/Services/`: `LevelService` (XP/nível, curva N*50), `StreakService` (streak diário), `AchievementService` (catálogo FIRST_MISSION, MISSIONS_100, STREAK_3/7/30/100). Testes diretos.
- `User.LastActivityDate` + `.migrations/V003__add_user_last_activity_date.sql`.
- Novos repositórios/interfaces: `IAvatarRepository`, `IAchievementRepository`, `IProgressHistoryRepository` + impls; `IMissionRepository.CountCompletedByUserAsync`. Registrados na DI (Infra + domain services como singleton na Application).
- 8 handlers implementados: Users (Get/Update), Progress (Get/History), Avatar (Get/Update), Streak (Get), Achievements (Get). Validators novos: UpdateCurrentUser, UpdateAvatar.
- `CompleteMissionCommandHandler` religado: conclui missão → XP (LevelService) → streak (StreakService) → ProgressHistory → conquistas (AchievementService). Usa `request.UserId` (não `user.Id`, que é gerado pelo banco).
- `RegisterCommandHandler` cria Avatar default via navegação (cascade EF).

**Verificação:** `dotnet build` 0 erros/avisos; `dotnet test` 76/76 verde; `dotnet format --verify-no-changes` limpo.

**Próximos passos sugeridos:**
- Code review (`sil-quality:code-review`) e commit (`sil-git:conventional-commit`) — ainda não commitado.
- Aplicar V003 no banco de dev (`psql ... -f .migrations/V003__...sql`).
- Avaliar `EnvironmentService` (evolução visual por nível) — ainda não implementado (não estava no escopo do MVP de handlers).
- Considerar retornar feedback de XP/nível/conquistas no payload de CompleteMission (hoje retorna só `MissionDto`).
