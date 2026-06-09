-- V001__initial_schema.sql
-- SoloLife - schema inicial
-- Identificadores em PascalCase para casar com o mapeamento por convenção do EF Core (SoloLifeDbContext).

BEGIN;

CREATE TABLE "Users" (
    "Id"            uuid         NOT NULL DEFAULT gen_random_uuid(),
    "Name"          text         NOT NULL,
    "Email"         text         NOT NULL,
    "PasswordHash"  text         NOT NULL,
    "CreatedAt"     timestamptz  NOT NULL DEFAULT now(),
    "LastLoginAt"   timestamptz  NULL,
    "CurrentLevel"  integer      NOT NULL DEFAULT 1,
    "CurrentXp"     integer      NOT NULL DEFAULT 0,
    "CurrentStreak" integer      NOT NULL DEFAULT 0,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE TABLE "Missions" (
    "Id"          uuid         NOT NULL DEFAULT gen_random_uuid(),
    "UserId"      uuid         NOT NULL,
    "Title"       text         NOT NULL,
    "Description" text         NOT NULL,
    "Category"    integer      NOT NULL,  -- enum MissionCategory: 0=Health 1=Study 2=Productivity 3=PersonalDevelopment
    "XpReward"    integer      NOT NULL,
    "Status"      integer      NOT NULL,  -- enum MissionStatus: 0=Pending 1=Completed 2=Expired
    "CreatedAt"   timestamptz  NOT NULL DEFAULT now(),
    "CompletedAt" timestamptz  NULL,
    CONSTRAINT "PK_Missions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Missions_Users_UserId" FOREIGN KEY ("UserId")
        REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Missions_UserId" ON "Missions" ("UserId");

CREATE TABLE "Avatars" (
    "Id"                uuid    NOT NULL DEFAULT gen_random_uuid(),
    "UserId"            uuid    NOT NULL,
    "CurrentSkin"       text    NOT NULL,
    "CurrentBackground" text    NOT NULL,
    "Accessories"       text[]  NOT NULL DEFAULT '{}',
    CONSTRAINT "PK_Avatars" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Avatars_Users_UserId" FOREIGN KEY ("UserId")
        REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- 1:1 com User
CREATE UNIQUE INDEX "IX_Avatars_UserId" ON "Avatars" ("UserId");

CREATE TABLE "Achievements" (
    "Id"          uuid         NOT NULL DEFAULT gen_random_uuid(),
    "UserId"      uuid         NOT NULL,
    "Code"        text         NOT NULL,
    "Title"       text         NOT NULL,
    "Description" text         NOT NULL,
    "UnlockedAt"  timestamptz  NULL,
    CONSTRAINT "PK_Achievements" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Achievements_Users_UserId" FOREIGN KEY ("UserId")
        REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Achievements_UserId" ON "Achievements" ("UserId");

CREATE TABLE "LifeGoals" (
    "Id"       uuid  NOT NULL DEFAULT gen_random_uuid(),
    "UserId"   uuid  NOT NULL,
    "Name"     text  NOT NULL,
    "Category" text  NOT NULL,
    CONSTRAINT "PK_LifeGoals" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_LifeGoals_Users_UserId" FOREIGN KEY ("UserId")
        REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_LifeGoals_UserId" ON "LifeGoals" ("UserId");

CREATE TABLE "ProgressHistory" (
    "Id"        uuid         NOT NULL DEFAULT gen_random_uuid(),
    "UserId"    uuid         NOT NULL,
    "MissionId" uuid         NULL,
    "XpGained"  integer      NOT NULL,
    "CreatedAt" timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT "PK_ProgressHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProgressHistory_Users_UserId" FOREIGN KEY ("UserId")
        REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProgressHistory_Missions_MissionId" FOREIGN KEY ("MissionId")
        REFERENCES "Missions" ("Id") ON DELETE SET NULL
);

CREATE INDEX "IX_ProgressHistory_UserId" ON "ProgressHistory" ("UserId");
CREATE INDEX "IX_ProgressHistory_MissionId" ON "ProgressHistory" ("MissionId");

COMMIT;
