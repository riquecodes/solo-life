-- V001__initial_schema.sql
-- SoloLife - schema inicial
-- Identificadores em snake_case minúsculo (sem aspas) — casam com o mapeamento
-- do EF Core via UseSnakeCaseNamingConvention (case-insensitive no PostgreSQL).

BEGIN;

CREATE SCHEMA IF NOT EXISTS sololife;
SET search_path TO sololife;

CREATE TABLE users (
    id             varchar(50)  NOT NULL DEFAULT (uuidv7())::text,
    name           text         NOT NULL,
    email          text         NOT NULL,
    password_hash  text         NOT NULL,
    created_at     timestamptz  NOT NULL DEFAULT now(),
    last_login_at  timestamptz  NULL,
    current_level  integer      NOT NULL DEFAULT 1,
    current_xp     integer      NOT NULL DEFAULT 0,
    current_streak integer      NOT NULL DEFAULT 0,
    CONSTRAINT pk_users PRIMARY KEY (id)
);

CREATE UNIQUE INDEX ix_users_email ON users (email);

CREATE TABLE missions (
    id           varchar(50)  NOT NULL DEFAULT (uuidv7())::text,
    user_id      varchar(50)  NOT NULL,
    title        text         NOT NULL,
    description  text         NOT NULL,
    category     integer      NOT NULL,  -- enum MissionCategory: 0=Health 1=Study 2=Productivity 3=PersonalDevelopment
    xp_reward    integer      NOT NULL,
    status       integer      NOT NULL,  -- enum MissionStatus: 0=Pending 1=Completed 2=Expired
    created_at   timestamptz  NOT NULL DEFAULT now(),
    completed_at timestamptz  NULL,
    CONSTRAINT pk_missions PRIMARY KEY (id),
    CONSTRAINT fk_missions_users_user_id FOREIGN KEY (user_id)
        REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_missions_user_id ON missions (user_id);

CREATE TABLE avatars (
    id                 varchar(50)  NOT NULL DEFAULT (uuidv7())::text,
    user_id            varchar(50)  NOT NULL,
    current_skin       text    NOT NULL,
    current_background text    NOT NULL,
    accessories        text[]  NOT NULL DEFAULT '{}',
    CONSTRAINT pk_avatars PRIMARY KEY (id),
    CONSTRAINT fk_avatars_users_user_id FOREIGN KEY (user_id)
        REFERENCES users (id) ON DELETE CASCADE
);

-- 1:1 com user
CREATE UNIQUE INDEX ix_avatars_user_id ON avatars (user_id);

CREATE TABLE achievements (
    id          varchar(50)  NOT NULL DEFAULT (uuidv7())::text,
    user_id     varchar(50)  NOT NULL,
    code        text         NOT NULL,
    title       text         NOT NULL,
    description text         NOT NULL,
    unlocked_at timestamptz  NULL,
    CONSTRAINT pk_achievements PRIMARY KEY (id),
    CONSTRAINT fk_achievements_users_user_id FOREIGN KEY (user_id)
        REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_achievements_user_id ON achievements (user_id);

CREATE TABLE life_goals (
    id       varchar(50)  NOT NULL DEFAULT (uuidv7())::text,
    user_id  varchar(50)  NOT NULL,
    name     text  NOT NULL,
    category text  NOT NULL,
    CONSTRAINT pk_life_goals PRIMARY KEY (id),
    CONSTRAINT fk_life_goals_users_user_id FOREIGN KEY (user_id)
        REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX ix_life_goals_user_id ON life_goals (user_id);

CREATE TABLE progress_history (
    id         varchar(50)  NOT NULL DEFAULT (uuidv7())::text,
    user_id    varchar(50)  NOT NULL,
    mission_id varchar(50)  NULL,
    xp_gained  integer      NOT NULL,
    created_at timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT pk_progress_history PRIMARY KEY (id),
    CONSTRAINT fk_progress_history_users_user_id FOREIGN KEY (user_id)
        REFERENCES users (id) ON DELETE CASCADE,
    CONSTRAINT fk_progress_history_missions_mission_id FOREIGN KEY (mission_id)
        REFERENCES missions (id) ON DELETE SET NULL
);

CREATE INDEX ix_progress_history_user_id ON progress_history (user_id);
CREATE INDEX ix_progress_history_mission_id ON progress_history (mission_id);

COMMIT;
