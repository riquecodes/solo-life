-- V002__add_user_refresh_token.sql
-- Persistência do refresh token no usuário (sessão única). Armazena apenas o hash.

BEGIN;

ALTER TABLE "Users"
    ADD COLUMN "RefreshTokenHash"      text         NULL,
    ADD COLUMN "RefreshTokenExpiresAt" timestamptz  NULL;

-- Lookup por hash no fluxo de refresh.
CREATE INDEX "IX_Users_RefreshTokenHash" ON "Users" ("RefreshTokenHash");

COMMIT;
