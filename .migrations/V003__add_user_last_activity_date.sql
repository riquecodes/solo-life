-- V003__add_user_last_activity_date.sql
-- Data (sem hora) da última missão concluída — referência do StreakService para manter/quebrar o streak.

BEGIN;

SET search_path TO sololife;

ALTER TABLE users
    ADD COLUMN last_activity_date date NULL;

COMMIT;
