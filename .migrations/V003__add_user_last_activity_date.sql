-- V003__add_user_last_activity_date.sql
-- Data (sem hora) da última missão concluída — referência do StreakService para manter/quebrar o streak.

BEGIN;

ALTER TABLE "Users"
    ADD COLUMN "LastActivityDate" date NULL;

COMMIT;
