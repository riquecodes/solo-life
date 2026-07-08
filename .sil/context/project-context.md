---
last_updated: "2026-06-13T00:00:00"
---

# SoloLife — Contexto do Projeto

## Stack
- .NET 10 / C# 13 (nullable + implicit usings, `using` dentro do namespace).
- ASP.NET Core Web API, EF Core (Npgsql/PostgreSQL), MediatR (CQRS), FluentValidation, AutoMapper, Serilog, JWT Bearer.
- Testes: xUnit + NSubstitute + Shouldly (`tests/SoloLife.UnitTests`).
- Solução: `SoloLife.slnx` (formato XML novo).

## Arquitetura (Clean Architecture)
`Api → Application → Domain`; `Api → Infrastructure → {Application, Domain}`.
- **Domain**: entidades (base `Entity` com `string Id` gerado pelo banco via `uuidv7()`), enums, **domain services** puros (`Services/`).
- **Application**: CQRS por slices verticais `Features/{Feature}/{Operation}/`. Interfaces em `Common/Interfaces/`. Retorna `Result`/`Result<T>`.
- **Infrastructure**: `SoloLifeDbContext` (também `IUnitOfWork`; commit em `SaveChangesAsync`, repos só staging). Repositórios, JWT, PasswordHasher. Write ops retornam `InfraResult`.
- **Api**: controllers finos extendendo `ApiControllerBase` (expõe `Sender`, `CurrentUserId`, `ToActionResult`). Única camada que retorna `IActionResult`.

## Contrato de resultado por camada (obrigatório)
Application=`Result<T>`, Infra=`InfraResult`, Api=`IActionResult`. Não vazar tipo entre camadas.

## Banco / Migrations
- Sem migrations EF. Schema manual em `.migrations/V{NNN}__{desc}.sql`, sequencial 3 dígitos, cada uma em `BEGIN; ... COMMIT;`.
- Identificadores **`snake_case` minúsculo (sem aspas)** — casam com o mapeamento do EF via `UseSnakeCaseNamingConvention()` (não há `IEntityTypeConfiguration`). Enums = `integer`; arrays = `text[]`.
- Aplicar: `psql "...sololife..." -f .migrations/VNNN__....sql`.

## Regras de gamificação (MVP, fonte: docs/backend-claude.md)
- **XP/Nível** (`LevelService`): curva linear, nível N→N+1 custa `N*50` XP (LV1=50, LV2=100...). `CurrentXp` é **relativo ao nível** (reseta ao subir, carrega excedente). `RemainingXp = N*50 - CurrentXp`.
- **Streak** (`StreakService`): +1 por dia consecutivo com missão concluída; zera após gap. Usa `User.LastActivityDate` (date). Streak efetivo na leitura é 0 se última atividade < ontem.
- **Conquistas** (`AchievementService`): desbloqueio automático insert-on-unlock. Catálogo: FIRST_MISSION, MISSIONS_100, STREAK_3/7/30/100.
- **Avatar**: criado no Register (default). Backend não guarda imagens, só skin/background/accessories.
- `CompleteMissionCommandHandler` é o ponto de orquestração: XP → nível → streak → histórico → conquistas.

## Convenções
- Código em inglês; strings/mensagens/erros ao usuário e comentários em **pt-BR**.
- TDD obrigatório (red-green-refactor). Mockar interfaces injetadas; domain services puros têm teste direto.
- Mensagem de erro genérica para recursos de outro usuário ("Missão não encontrada.").

## Comandos
- `dotnet build SoloLife.slnx`
- `dotnet test SoloLife.slnx` (filtro: `--filter "FullyQualifiedName~XHandler"`)
- `dotnet run --project src/SoloLife.Api` (Swagger em `/swagger` no Development)
- `dotnet format SoloLife.slnx`
