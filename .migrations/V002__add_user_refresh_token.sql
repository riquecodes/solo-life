-- V002__add_user_refresh_token.sql
-- Persistência do refresh token no usuário (sessão única). Armazena apenas o hash.

BEGIN;

SET search_path TO sololife;

ALTER TABLE users
    ADD COLUMN refresh_token_hash       text         NULL,
    ADD COLUMN refresh_token_expires_at timestamptz  NULL;

-- Lookup por hash no fluxo de refresh.
CREATE INDEX ix_users_refresh_token_hash ON users (refresh_token_hash);

COMMIT;
