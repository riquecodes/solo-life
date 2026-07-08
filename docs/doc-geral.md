# Documentação geral

# Documento de Introdução — MVP do SoloLife

# 🎯 Visão Geral

SoloLife é um aplicativo mobile de desenvolvimento pessoal gamificado que transforma hábitos, tarefas e objetivos da vida real em missões que geram progresso visual.

O objetivo é permitir que o usuário acompanhe sua evolução através de um avatar personalizado e de um ambiente que melhora conforme ele mantém consistência em seus hábitos.

Diferente de aplicativos tradicionais de produtividade, o SoloLife não foca apenas em listas de tarefas ou organização.

Seu principal diferencial é permitir que o usuário visualize a construção gradual da vida que deseja alcançar.

O aplicativo deve transmitir sensação constante de:

- evolução;
- progresso;
- recompensa;
- disciplina;
- conquista;
- crescimento pessoal.

---

# 🚀 Objetivo do MVP

O MVP (Minimum Viable Product) terá como objetivo validar:

- engajamento do usuário;
- sensação de progressão;
- retenção diária;
- potencial da gamificação;
- impacto da evolução visual na motivação.

Além da validação da experiência do usuário, o MVP também terá como objetivo validar a arquitetura completa da aplicação:

- Frontend React Native;
- API .NET;
- PostgreSQL;
- autenticação de usuários;
- sincronização em nuvem;
- estrutura preparada para IA futura.

O sistema deve ser simples, rápido e intuitivo.

Pergunta principal que o MVP pretende responder:

> Usuários permanecem consistentes por mais tempo quando conseguem visualizar sua evolução e sua vida ideal sendo construída diariamente?
> 

---

# 📱 Plataforma

Aplicativo mobile:

- iOS;
- Android.

---

## Frontend

Tecnologias:

- React Native
- Expo
- TypeScript

Responsável por:

- interface do usuário;
- animações;
- experiência visual;
- comunicação com API.

---

## Backend

Tecnologias:

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- JWT Authentication

Responsável por:

- autenticação;
- gerenciamento de usuários;
- cálculo de XP;
- cálculo de níveis;
- gerenciamento de missões;
- streaks;
- conquistas;
- evolução visual;
- persistência dos dados.

---

## Banco de Dados

Tecnologia:

- PostgreSQL

Responsável por:

- armazenamento permanente;
- sincronização entre dispositivos;
- histórico de progresso;
- objetivos do usuário;
- evolução do avatar.

---

# 🏗️ Arquitetura do Sistema

O sistema será dividido em três camadas principais:

## Mobile App

Responsável por:

- experiência do usuário;
- exibição de progresso;
- animações;
- consumo da API.

---

## API

Responsável por:

- regras de negócio;
- autenticação;
- progressão;
- gerenciamento das entidades.

---

## Banco de Dados

Responsável por:

- persistência;
- histórico;
- integridade dos dados.

---

# 💡 Conceito Principal

O usuário possui uma representação digital de si mesmo.

Essa representação evolui conforme missões e hábitos são concluídos.

A ideia central não é evoluir um personagem fictício.

A ideia é evoluir uma representação da própria vida.

Exemplos:

- Treinar → aumenta progresso físico;
- Estudar → aumenta progresso intelectual;
- Dormir bem → aumenta bem-estar;
- Manter consistência → fortalece evolução geral.

Conceito principal:

> Sua rotina cria sua realidade.
> 

---

# 🎯 Missões como Núcleo do Produto

O sistema de missões é o coração do aplicativo.

Toda evolução visual depende diretamente das missões concluídas.

Fluxo principal:

```
Missão → XP → Level → Evolução Visual
```

Cada missão possui:

- título;
- descrição;
- categoria;
- XP;
- status;
- frequência;
- recompensa.

---

## Categorias de Missão

### Saúde

- Academia
- Corrida
- Caminhada
- Dormir 8 horas
- Beber água

### Estudos

- Estudar
- Ler livro
- Fazer exercícios
- Assistir aula

### Produtividade

- Trabalhar focado
- Finalizar tarefa
- Planejamento diário

### Desenvolvimento Pessoal

- Meditação
- Organização
- Journaling

---

# 🏠 Estrutura do MVP

## 1. Tela Home

Tela principal do aplicativo.

Deve exibir:

- avatar;
- ambiente atual;
- nível;
- barra de XP;
- streak;
- resumo diário;
- missões pendentes;
- progresso do dia.

Objetivo:

Gerar sensação imediata de progresso.

---

## 2. Tela de Missões

Lista completa de missões.

Funcionalidades:

- criar missão;
- editar missão;
- concluir missão;
- visualizar histórico.

Cada missão mostra:

- XP recebido;
- categoria;
- status.

---

## 3. Tela de Evolução

Mostra toda a jornada visual do usuário.

Exemplo:

| Nível | Desbloqueio |
| --- | --- |
| 1 | Quarto simples |
| 10 | Ambiente organizado |
| 25 | Setup moderno |
| 50 | Apartamento premium |
| 75 | Escritório próprio |
| 100 | Vida ideal |

Objetivo:

Permitir que o usuário visualize seu futuro e mantenha motivação.

---

## 4. Tela de Conquistas

Exemplos:

- 7 dias seguidos;
- 30 dias seguidos;
- 100 treinos;
- 100 horas estudadas;
- 100 missões concluídas.

---

## 5. Perfil

Informações do usuário:

- avatar;
- objetivos de vida;
- estatísticas gerais;
- histórico de progresso.

---

# ⭐ Sistema de XP e Level

Cada missão gera XP.

Exemplo:

| Missão | XP |
| --- | --- |
| Academia | +20 XP |
| Estudo | +15 XP |
| Sono | +10 XP |

Ao atingir determinada quantidade de XP:

- sobe de nível;
- desbloqueia novos elementos visuais;
- recebe recompensas.

Curva inicial:

- LV1 → 50 XP
- LV2 → 100 XP
- LV3 → 150 XP

A progressão segue crescimento linear no MVP.

---

# 🧠 Sistema de Classes (Opcional)

Durante o cadastro o usuário pode escolher uma classe principal.

A classe não altera a evolução visual.

Ela apenas concede bônus em determinadas categorias.

## Monge

Foco:

- estudo;
- leitura;
- meditação.

Benefício:

- +100% XP nessas categorias.

---

## Atleta

Foco:

- academia;
- corrida;
- saúde.

Benefício:

- +100% XP em atividades físicas.

---

## Construtor

Foco:

- produtividade;
- trabalho;
- metas profissionais.

Benefício:

- bônus em tarefas produtivas.

---

# 👤 Sistema de Avatar

O avatar representa o usuário.

Conforme sobe de nível:

- aparência melhora;
- postura muda;
- roupas evoluem;
- acessórios são desbloqueados.

Objetivo:

Criar apego emocional e senso de crescimento.

---

# 🏡 Sistema de Ambiente Evolutivo

O ambiente evolui junto com o usuário.

## Nível 1

- quarto bagunçado;
- móveis antigos;
- iluminação ruim.

## Nível 10

- ambiente organizado;
- móveis melhores.

## Nível 25

- setup moderno;
- decoração personalizada.

## Nível 50

- apartamento sofisticado.

## Nível 75

- escritório próprio;
- hobbies visíveis.

## Nível 100

- representação da vida ideal.

---

# 🎯 Objetivos de Vida

Durante o onboarding o usuário escolhe objetivos que representam sucesso para ele.

## Carreira

- Programador
- Empresário
- Executivo

## Saúde

- Shape atlético
- Corredor
- Atleta

## Estilo de Vida

- Viajar
- Casa própria
- Apartamento moderno

## Luxo

- Carro esportivo
- Relógios
- Coleções

## Família

- Casamento
- Filhos
- Lar estruturado

Essas escolhas influenciam os elementos visuais desbloqueados.

---

# 🔥 Sistema de Streak

O aplicativo acompanha sequência de dias ativos.

Marcos:

- 3 dias → bônus XP;
- 7 dias → recompensa;
- 30 dias → título especial;
- 100 dias → conquista rara.

Objetivo:

Aumentar retenção e consistência.

---

# 💾 Persistência de Dados

O MVP utilizará armazenamento em nuvem através da API .NET.

Dados armazenados:

- usuário;
- perfil;
- avatar;
- objetivos de vida;
- XP;
- nível;
- streak;
- missões;
- conquistas;
- histórico de progresso.

Benefícios:

- sincronização entre dispositivos;
- backup automático;
- recuperação de conta;
- expansão futura.

---

# 🔌 API (MVP)

## Autenticação

- Cadastro
- Login
- Refresh Token

## Usuário

- Perfil
- Avatar
- Objetivos de vida

## Missões

- Criar missão
- Editar missão
- Excluir missão
- Concluir missão
- Histórico

## Progressão

- XP
- Nível
- Streak

## Conquistas

- Consulta de conquistas
- Desbloqueio automático

---

# 🎨 Direção Visual

Estilo:

- moderno;
- minimalista;
- clean;
- aspiracional.

Referências:

- Memoji
- Apple Fitness
- Duolingo
- Finch
- Headspace

Paleta:

- Dark Mode predominante;
- Azul;
- Branco;
- Verde para progresso;
- Roxo para conquistas.

O aplicativo deve parecer:

> Um jogo premium disfarçado de um aplicativo de evolução pessoal.
> 

---

# 🛣️ Roadmap

## MVP

- API .NET
- PostgreSQL
- Cadastro e Login
- JWT
- Sistema de Missões
- XP e Level
- Streak
- Conquistas
- Avatar básico
- Ambiente evolutivo
- Objetivos de vida
- Sincronização em nuvem

---

## Pós-MVP

- IA para geração de missões
- IA para análise de progresso
- Estatísticas avançadas
- Ranking entre amigos
- Compartilhamento de conquistas
- Notificações inteligentes
- Sistema de metas avançadas
- Personalização expandida

---

# 🎯 Objetivo Final

Transformar evolução pessoal em uma experiência visual, divertida e motivadora.

Fazer com que o usuário sinta que cada pequena ação realizada hoje está construindo a vida que deseja viver amanhã.

[claude.md](https://app.notion.com/p/claude-md-36e8eea7441b802b88cec526d86a6a45?pvs=21)

[backend-claude.md](https://app.notion.com/p/backend-claude-md-3768eea7441b801f998ee7a4e96a32a2?pvs=21)