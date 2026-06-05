# backend-claude.md

# 🚀 SOLO LIFE - Backend Context

Você é um arquiteto de software e desenvolvedor sênior especializado em .NET, PostgreSQL, Clean Architecture, DDD e APIs REST.

Este documento representa a fonte oficial de verdade para todas as decisões relacionadas ao backend do projeto SOLO LIFE.

# 📌 Visão Geral

SOLO LIFE é um aplicativo de desenvolvimento pessoal gamificado.

O objetivo do backend é gerenciar:

- usuários;
- missões;
- progresso;
- XP;
- níveis;
- streaks;
- conquistas;
- avatar;
- evolução visual;
- objetivos de vida.

O backend deve ser projetado desde o início para suportar crescimento futuro sem necessidade de reescrita.

# 🏗 Arquitetura

Padrão adotado:

- Clean Architecture
- Domain Driven Design (DDD)
- SOLID
- CQRS (quando necessário)
- Repository Pattern

Estrutura:

src/

├── SoloLife.Api

├── SoloLife.Application

├── SoloLife.Domain

├── SoloLife.Infrastructure

└── SoloLife.Tests

# 💻 Stack

Backend:

- .NET 9
- [ASP.NET](http://asp.net/) Core Web API

Banco:

- PostgreSQL

ORM:

- Entity Framework Core

Autenticação:

- JWT Bearer

Documentação:

- Swagger/OpenAPI

Logs:

- Serilog

Validação:

- FluentValidation

Mapeamento:

- AutoMapper

# 👤 Usuários

Cada usuário possui:

- Id
- Nome
- Email
- SenhaHash
- DataCadastro
- ÚltimoLogin
- NívelAtual
- XPAtual
- StreakAtual

Relacionamentos:

Usuário possui:

- Missões
- Conquistas
- Avatar
- Objetivos
- Histórico de progresso

# 🎯 Objetivos de Vida

Representam o caminho desejado pelo usuário.

Exemplos:

- Programador
- Empresário
- Executivo
- Atleta
- Corredor
- Casa própria
- Viajar

Esses objetivos influenciam apenas o sistema visual.

Não alteram cálculos de XP.

# 📜 Missões

As missões são o núcleo do sistema.

Toda progressão depende delas.

## Entidade Mission

Campos:

- Id
- UserId
- Título
- Descrição
- Categoria
- XPReward
- Status
- DataCriação
- DataConclusão

## Categorias

Health
Study
Productivity
PersonalDevelopment

## Status

Pending
Completed
Expired

# ⭐ Sistema de XP

Toda missão concluída gera XP.

Regra:

XP total do usuário deve ser atualizado imediatamente após conclusão.

## Fórmula MVP

Level 1 → 50 XP

Level 2 → 100 XP

Level 3 → 150 XP

Progressão linear simples.

## Responsabilidade

Serviço:

LevelService

Métodos:

CalculateLevel()

CalculateRemainingXP()

AddXP()

# 🔥 Sistema de Streak

O usuário mantém streak ao concluir missões diariamente.

## Regras

1 dia sem completar missão:

Streak = 0

Missão concluída no dia:

Streak +1

## Marcos

3 dias
7 dias
30 dias
100 dias

## Serviço

StreakService

Métodos:

UpdateStreak()

BreakStreak()

GetCurrentStreak()

# 🏆 Sistema de Conquistas

Conquistas são desbloqueadas automaticamente.

## Exemplos

Primeira missão

Primeiros 7 dias

Primeiros 30 dias

100 missões concluídas

100 horas estudadas

## Serviço

AchievementService

Métodos:

EvaluateAchievements()

UnlockAchievement()

# 👤 Avatar

Representação visual do usuário.

Backend NÃO armazena imagens.

Armazena apenas:

- nível visual atual
- itens desbloqueados
- customizações

## Entidade Avatar

- UserId
- CurrentSkin
- CurrentBackground
- Accessories

# 🏠 Evolução do Ambiente

Representa progresso visual.

## Marcos

Level 1

Quarto simples

Level 10

Quarto organizado

Level 25

Setup moderno

Level 50

Apartamento premium

Level 75

Escritório próprio

Level 100

Vida ideal

## Responsabilidade

EnvironmentService

Métodos:

GetCurrentEnvironment()

GetNextUnlock()

# 📊 Histórico

O sistema deve manter histórico.

## Entidade ProgressHistory

Campos:

- Id
- UserId
- XPGained
- MissionId
- CreatedAt

# 🔐 Autenticação

JWT Bearer.

Endpoints:

POST /auth/register

POST /auth/login

POST /auth/refresh

# 📡 API MVP

## Usuário

GET /users/me

PUT /users/me

## Missões

GET /missions

POST /missions

PUT /missions/{id}

DELETE /missions/{id}

POST /missions/{id}/complete

## Progresso

GET /progress

GET /progress/history

## Streak

GET /streak

## Conquistas

GET /achievements

## Avatar

GET /avatar

PUT /avatar

# 🤖 IA (FASE FUTURA)

Tecnologias:

- Semantic Kernel
- [Microsoft.Extensions.AI](http://microsoft.extensions.ai/)

## Objetivos

Gerar:

- missões inteligentes;
- recomendações;
- relatórios semanais;
- notificações personalizadas.

## Restrições

IA nunca altera XP diretamente.

IA nunca altera streak diretamente.

IA apenas sugere ações.

Toda regra de negócio deve permanecer no backend.

# 📌 Diretrizes para Desenvolvimento

1. Toda regra de negócio deve ficar na camada Domain/Application.
2. Controllers devem ser finos.
3. Não colocar lógica de negócio em Controllers.
4. Toda funcionalidade deve ser preparada para crescimento futuro.
5. Sempre considerar persistência em PostgreSQL.
6. Todo endpoint deve ser autenticado, exceto login e cadastro.
7. O backend é responsável pela verdade absoluta do progresso do usuário.
8. O frontend nunca calcula XP, níveis ou streaks.
9. Camada de API deve ser a única a retornar IActionResult.
10. Camada de Application retorna tipo Result<T>.
11. Camada de Infra retorna Id/Guid por meio de InfraResult.