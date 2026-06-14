# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project status

SoloLife is a gamified personal-development app (mobile RN/Expo frontend + this .NET backend). **This repo contains only the backend, and it is an early scaffold.** The Clean Architecture skeleton, DI wiring, entities, DTOs, controllers, and the DB schema all exist — but **every MediatR handler is a stub** returning `Result.Failure("Não implementado.")`. Implementing those handlers (and the domain services they call) is the bulk of the remaining work. Treat `docs/` as product/architecture intent, not as a description of working code.

## Commands

Solution file is `SoloLife.slnx` (new XML solution format — needs a recent .NET 10 SDK / VS 2022 17.14+).

```powershell
dotnet build SoloLife.slnx                      # build all 4 projects
dotnet run --project src/SoloLife.Api           # run the API (Swagger at /swagger in Development)
dotnet format SoloLife.slnx                      # format
```

```powershell
dotnet test SoloLife.slnx                                    # run all tests
dotnet test --filter "FullyQualifiedName~RegisterCommandHandler"   # run a single test class
```

Tests live in `tests/SoloLife.UnitTests` (xUnit + NSubstitute for mocking + Shouldly for assertions).

### Database

EF Core is used for **runtime mapping only — there are no EF migrations**. Schema is managed by hand-written SQL files in `.migrations/`:

```powershell
psql "Host=localhost;Port=5432;Database=sololife;Username=postgres;Password=postgres" -f .migrations/V001__initial_schema.sql
```

New migrations: `V{NNN}__{descricao}.sql`, sequential zero-padded 3 digits, each wrapped in `BEGIN; ... COMMIT;`. **All identifiers are `snake_case` lowercase (unquoted)** so they match EF Core's mapping via `UseSnakeCaseNamingConvention()` (`EFCore.NamingConventions`, wired in `AddInfrastructure`). Because they are unquoted, PostgreSQL treats names case-insensitively (folds to lowercase) — this avoids `relation "Users" does not exist` errors. All objects live in the `sololife` schema (migrations do `CREATE SCHEMA IF NOT EXISTS sololife; SET search_path TO sololife;`; EF targets it via `modelBuilder.HasDefaultSchema("sololife")`). There are no `IEntityTypeConfiguration` classes, so the table/column names in SQL are the contract with the entity property names (translated to snake_case). Enums are stored as `integer`; `text[]` for arrays.

## Architecture

Four projects, dependencies point inward (Clean Architecture): `Api → Application → Domain`, `Api → Infrastructure → {Application, Domain}`.

- **Domain** (`SoloLife.Domain`) — entities (`Entity` base gives a `Guid Id`), enums. No dependencies.
- **Application** (`SoloLife.Application`) — CQRS via **MediatR**, organized as vertical slices: `Features/{Feature}/{Operation}/` each holding its Command/Query + Handler. DTOs under `Features/{Feature}/Dtos/`. Cross-cutting interfaces (`IUserRepository`, `IMissionRepository`, `IUnitOfWork`, `IJwtTokenGenerator`, `IPasswordHasher`) live in `Common/Interfaces/` and are implemented in Infrastructure. Uses FluentValidation + AutoMapper (registered by-assembly in `DependencyInjection.cs`).
- **Infrastructure** (`SoloLife.Infrastructure`) — EF Core `SoloLifeDbContext` (also implements `IUnitOfWork`; `SaveChangesAsync` is the commit point — repositories only stage changes, they don't save), repositories, JWT + password-hashing services. Npgsql/PostgreSQL.
- **Api** (`SoloLife.Api`) — thin controllers only. Program.cs wires Serilog, JWT bearer, Swagger.

### The layered result contract (important, enforced by the codebase)

Each layer has its own return type — do not leak one layer's type into another:

- **Application** handlers return `Result` / `Result<T>` (`Common/Results/Result.cs`).
- **Infrastructure** write operations return `InfraResult` (carries the affected `Guid Id`).
- **Api** is the *only* layer that returns `IActionResult`. Controllers extend `ApiControllerBase`, which:
  - exposes `Sender` (lazily-resolved MediatR `ISender`) — controllers just `Sender.Send(command)`,
  - exposes `CurrentUserId` (parsed from the JWT `NameIdentifier` claim) — pass this into commands; never trust a user id from the request body,
  - maps `Result`/`Result<T>` → `Ok`/`BadRequest` via `ToActionResult(...)`.

Controller pattern (keep them this thin): build the command from request + `CurrentUserId`, `await Sender.Send(...)`, wrap in `ToActionResult`.

### Design rules from `docs/backend-claude.md`

The backend is the single source of truth for all progression — the frontend never computes XP, levels, or streaks. All business logic stays in Domain/Application; controllers carry none. All endpoints require auth except register/login/refresh. Planned domain services (not yet written): `LevelService`, `StreakService`, `AchievementService`, `EnvironmentService` — `CompleteMissionCommandHandler` is the orchestration point that will call them.

## Workflow — TDD is mandatory

**Every feature/bugfix is test-driven: write the failing test first, then implement to green, then refactor.** No feature code lands without tests written first. Handlers are the primary unit under test — mock their injected interfaces (`IUserRepository`, `IPasswordHasher`, `IJwtTokenGenerator`, `IUnitOfWork`) with NSubstitute and assert the returned `Result`/`Result<T>` plus the side effects (repository calls, `SaveChangesAsync`). Pure logic (e.g. `PasswordHasher`) gets direct round-trip tests. This matches the Sil pipeline (`sil-superpowers:test-driven-development`, `sil-testing:unit-test-generator`).

## Conventions

- **Code is English; user-facing strings, comments, and error messages are pt-BR** (matches the Sil-ecosystem tooling active in this environment).
- C# 13 / .NET 10, nullable + implicit usings enabled across all projects.
- `using` directives go **inside** the namespace (file-scoped namespace first, then usings) — match the existing files.
- Config secrets (JWT `Secret`, connection string) currently sit in `appsettings.json` with dev defaults — override per environment, don't commit real secrets.
