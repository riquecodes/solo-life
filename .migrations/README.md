# .migrations

Migrations SQL manuais (fora da solution). Não usa EF migrations.

## Convenção

- Nome: `V{NNN}__{descricao}.sql` (ex.: `V002__add_xyz.sql`). Sequencial, zero-pad 3 dígitos.
- Identificadores em `"PascalCase"` (aspas) — casam com o mapeamento por convenção do EF Core (`SoloLifeDbContext`). Mudar isso exige config EF nas entidades.
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
