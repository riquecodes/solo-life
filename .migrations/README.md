# .migrations

Migrations SQL manuais (fora da solution). Não usa EF migrations.

## Convenção

- Nome: `V{NNN}__{descricao}.sql` (ex.: `V002__add_xyz.sql`). Sequencial, zero-pad 3 dígitos.
- Identificadores em `snake_case` minúsculo (sem aspas) — casam com o mapeamento do EF Core via `UseSnakeCaseNamingConvention()` (pacote `EFCore.NamingConventions`, configurado no `AddInfrastructure`). Como ficam sem aspas, o PostgreSQL trata os nomes de forma case-insensitive (folding para minúsculo), evitando erros de `relation "Users" does not exist`.
- Tudo vive no schema `sololife` (não `public`). As migrations fazem `CREATE SCHEMA IF NOT EXISTS sololife; SET search_path TO sololife;`; o EF aponta para ele via `modelBuilder.HasDefaultSchema("sololife")`.
- Cada arquivo envolto em `BEGIN; ... COMMIT;`.
- Tipos: chaves como `varchar(50)` com default `(uuidv7())::text` (requer PostgreSQL 18+), `timestamptz`, enums como `integer`, arrays nativos (`text[]`).

## Aplicar

```bash
psql "Host=localhost;Port=5432;Database=sololife;Username=postgres;Password=postgres" -f .migrations/V001__initial_schema.sql
```

ou via Docker:

```bash
docker exec -i <container> psql -U postgres -d sololife < .migrations/V001__initial_schema.sql
```

## Histórico

| Versão | Descrição                          |
|--------|------------------------------------|
| V001   | Schema inicial                     |
| V002   | Refresh token no usuário (hash)    |
| V003   | LastActivityDate no usuário        |
