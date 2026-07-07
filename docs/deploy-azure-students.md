# Playbook — Deploy do SoloLife (Azure for Students: app no Container Apps + banco PostgreSQL 18 no Neon)

> **Objetivo:** subir o backend SoloLife usando **Azure for Students** ($100 / 12 meses, **sem cartão**) pra rodar a **API** em **Azure Container Apps** (grant grátis, HTTPS automático), com o **banco PostgreSQL 18 no Neon** (gerenciado, **permanente grátis**). Resultado: crédito Students quase intocado e banco grátis pra sempre.

> ⚠️ **Descoberta importante (por que o banco NÃO fica no Azure):** o Azure Database for PostgreSQL Flexible Server só existe em SKU **paid tier** (não tem free tier). Subscriptions **Azure for Students** bloqueiam a criação — o `az postgres flexible-server create` falha com **`The location is restricted from performing this operation`** em *todas* as regiões permitidas pela política (`eastus`, `eastus2`, `southcentralus`, `mexicocentral`, `chilecentral`), mesmo com PG18 GA e capabilities presentes. Não adianta trocar de região. **Solução:** banco gerenciado externo grátis (Neon) + app no Container Apps. Além de destravar, é melhor pro objetivo: Neon é **permanente grátis**, enquanto o crédito Azure é finito.

## Visão geral da arquitetura

```mermaid
flowchart LR
    subgraph PC["Seu PC (cliente — pode desligar)"]
        AZCLI["Azure CLI (az)"]
        DBEAVER["DBeaver / psql"]
        PORTAL["Portal Azure (monitor)"]
    end

    subgraph AZURE["Azure Students (rg-sololife)"]
        ACA["Container Apps: SoloLife.Api (HTTPS publico, scale-to-zero)"]
    end

    subgraph NEON["Neon (permanente gratis)"]
        PG["PostgreSQL 18 (SSL, db sololife)"]
    end

    ACA -->|SSL Require| PG
    USERS["Usuarios do app"] -->|HTTPS| ACA
    AZCLI -.provisiona/deploya.-> ACA
    DBEAVER -->|SSL| PG
    PORTAL -.metricas/logs.-> ACA
```

**Regra de ouro:** app no Azure (usa só o grant grátis), banco no Neon (grátis pra sempre). O crédito de $100 fica de reserva pra crescer depois.

---

## Pré-requisitos

- Conta **Azure for Students** ativa (`https://azure.microsoft.com/free/students` — e-mail institucional, **sem cartão**).
- Conta **Neon** (`https://neon.tech` — login GitHub/Google, **sem cartão**).
- **Azure CLI**: `winget install --exact --id Microsoft.AzureCLI` → reabrir terminal → `az login`.
- Extensão Container Apps: `az extension add --name containerapp --upgrade`.
- Providers registrados (uma vez):
  ```powershell
  az provider register -n Microsoft.App
  az provider register -n Microsoft.OperationalInsights
  ```
- No PC: **DBeaver** (ou `psql`) pra aplicar as migrations.
- `Dockerfile` na raiz do repo (já existe neste projeto).

> ✅ **Custo total esperado: ~$0.** Container Apps cabe no grant grátis mensal; Neon free não cobra. O crédito Students fica praticamente parado.

---

## Variáveis usadas (ajuste uma vez)

```powershell
$RG      = "rg-sololife"
$LOC     = "eastus2"          # regiao PERMITIDA no Students (nao use brazilsouth — barrado pela politica)
$APPNAME = "sololife-api"
```

> ⚠️ **Região no Students:** a política "Allowed resource deployment regions" só libera `eastus`, `eastus2`, `southcentralus`, `mexicocentral`, `chilecentral`. `brazilsouth` é **bloqueado**. Confirme a lista da sua sub com:
> ```powershell
> az policy assignment list --query "[].parameters.listOfAllowedLocations.value" -o json
> ```

---

## Parte A — Banco no Neon (PostgreSQL 18)

1. Em `neon.tech` → **Sign up** (GitHub/Google, sem cartão).
2. **Create Project:**
   - **Postgres version: 18** (é o default do Neon; confirme).
   - **Region:** a mais perto (ex.: `AWS US East (Ohio) — us-east-2`). *(Neon não tem São Paulo; se latência for crítica, veja a alternativa Aiven no fim.)*
3. Copie a **connection string** que o Neon mostra. Formato:
   `postgresql://<user>:<password>@ep-xxxx-yyyy.us-east-2.aws.neon.tech/neondb?sslmode=require`
   - Anote **host**, **user**, **password**.
   - O Neon dá dois endpoints: o **direto** (`ep-xxxx...`) e o **pooler** (`ep-xxxx-pooler...`). Use o **direto** pra rodar migrations (DDL); o **pooler** pra runtime da API (mais conexões).

### A.1 — Criar o database `sololife` e aplicar as migrations

Use o endpoint **direto** (não o `-pooler`) pra DDL:

```powershell
$NEONHOST = "ep-xxxx-yyyy.us-east-2.aws.neon.tech"   # endpoint DIRETO
$NEONUSER = "<user>"
$NEONPASS = "<password>"

# cria o database (o projeto novo do Neon vem com 'neondb')
psql "host=$NEONHOST dbname=neondb user=$NEONUSER password=$NEONPASS sslmode=require" -c "CREATE DATABASE sololife;"

# aplica o schema NA ORDEM (V001 -> V002 -> ...)
psql "host=$NEONHOST dbname=sololife user=$NEONUSER password=$NEONPASS sslmode=require" -f .migrations/V001__initial_schema.sql
```

> `sslmode=require` é obrigatório no Neon. Rode os `.migrations/V*.sql` em ordem crescente; eles já fazem `CREATE SCHEMA IF NOT EXISTS sololife; SET search_path TO sololife;`.
> Sem `psql`? Conecte no **DBeaver** em `$NEONHOST:5432` (SSL obrigatório) e execute cada arquivo.

### A.2 — Confirmar

```powershell
psql "host=$NEONHOST dbname=sololife user=$NEONUSER password=$NEONPASS sslmode=require" -c "SELECT version();"
```
Deve dizer **PostgreSQL 18.x**. Liste as tabelas: `\dt sololife.*`.

---

## Parte B — Resource group (Azure)

```powershell
az group create -n $RG -l $LOC
```

Confirme que ficou na região permitida (tem que imprimir `eastus2`):
```powershell
az group show -n $RG --query location -o tsv
```

> Se você criou o RG antes em `brazilsouth` e tomou `location is restricted`, apague e recrie: `az group delete -n $RG --yes` → o `az group create` acima.

---

## Parte C — Deploy da SoloLife.Api no Container Apps

`az containerapp up` builda o **Dockerfile na nuvem** e cria o Container App + ambiente. **Não precisa de Docker local.** A connection string aponta pro **pooler** do Neon.

```powershell
$NEONPOOLER = "ep-xxxx-yyyy-pooler.us-east-2.aws.neon.tech"   # endpoint POOLER
$CONNSTR = "Host=$NEONPOOLER;Database=sololife;Username=$NEONUSER;Password=$NEONPASS;SSL Mode=Require;Trust Server Certificate=true;Maximum Pool Size=20"
$JWTSECRET = "<gere-com: openssl rand -base64 48>"

az containerapp up `
  --resource-group $RG --name $APPNAME `
  --location $LOC `
  --source . `
  --ingress external --target-port 8080 `
  --env-vars `
    "ASPNETCORE_ENVIRONMENT=Production" `
    "ConnectionStrings__Default=$CONNSTR" `
    "Jwt__Secret=$JWTSECRET" `
    "Jwt__Issuer=SoloLife" `
    "Jwt__Audience=SoloLifeApp"
```

Ao terminar, imprime a **URL HTTPS pública** (`https://sololife-api.<hash>.eastus2.azurecontainerapps.io`).

> - `ConnectionStrings__Default` / `Jwt__Secret` (duplo underscore) sobrescrevem `ConnectionStrings:Default` / `Jwt:Secret` sem tocar no código — `DependencyInjection.cs` lê `GetConnectionString("Default")` e `Program.cs` exige `Jwt:Secret` (já sem fallback hardcoded).
> - `SSL Mode=Require;Trust Server Certificate=true` porque o Neon força TLS. (Endurecer depois: `VerifyFull` com o CA do Neon.)
> - `--target-port 8080` bate com `EXPOSE 8080` / `ASPNETCORE_URLS=http://+:8080` do Dockerfile.
> - **Pooler do Neon:** transaction-mode. Se aparecer erro de prepared statements, acrescente `No Reset On Close=true;Max Auto Prepare=0` na connection string.

### C.1 — Scale-to-zero (economia máxima)

```powershell
az containerapp update `
  --resource-group $RG --name $APPNAME `
  --min-replicas 0 --max-replicas 2
```

Sem tráfego, o app **dorme** (~$0). Primeiro request depois tem **cold start** de alguns segundos — ok pra MVP.

> 💡 O Neon free também **suspende** o banco após inatividade e acorda no primeiro acesso (alguns segundos). Combina bem com o scale-to-zero da API.

### C.2 — Segredos como secret (recomendado)

`--env-vars` deixa o valor visível na config. Pra tratar como secret:

```powershell
az containerapp secret set --resource-group $RG --name $APPNAME `
  --secrets jwt-secret=$JWTSECRET db-conn="$CONNSTR"

az containerapp update --resource-group $RG --name $APPNAME `
  --set-env-vars "Jwt__Secret=secretref:jwt-secret" "ConnectionStrings__Default=secretref:db-conn"
```

---

## Parte D — Validar

```powershell
$URL = az containerapp show -g $RG -n $APPNAME --query "properties.configuration.ingress.fqdn" -o tsv
"https://$URL"
```

- Bata num endpoint real (ex.: `POST https://$URL/api/auth/register` ou `/login`). **Swagger só aparece em Development** — em Production teste endpoint de verdade.
- Logs do app: `az containerapp logs show -g $RG -n $APPNAME --follow`.
- No banco (DBeaver/psql no Neon): `SELECT * FROM sololife.users;` pra ver o registro criado.

---

## Parte E — Monitoramento (sem construir nada)

| O que ver | Ferramenta | Como |
|---|---|---|
| CPU/RAM/replicas/deploy, logs do app | **Portal Azure** → Container App → *Monitoring* / *Log stream* | ou `az containerapp logs show --follow` |
| Requisições HTTP (rota, status, tempo) | **Log Analytics** (logs do Serilog) | tabela `ContainerAppConsoleLogs_CL` |
| Métricas/uso do banco | **Neon Console** → *Monitoring* | conexões, compute, storage |
| Conexões/queries ativas | **DBeaver/psql** no Neon | `SELECT * FROM pg_stat_activity;` |
| Queries lentas | **Neon** (`pg_stat_statements` já disponível) | `SELECT * FROM pg_stat_statements ORDER BY total_exec_time DESC;` |

> O **Seq** do `appsettings.json` (aponta pra `localhost:5341`) é opcional — no Azure os logs já caem no Log Analytics. Pra MVP, Log stream + Neon Console bastam.

---

## Custos

| Item | Custo | Observação |
|---|---|---|
| Container Apps (API) | **~$0** | dentro do grant grátis mensal; scale-to-zero reforça |
| PostgreSQL 18 (Neon free) | **$0** | 0.5 GB, permanente grátis, sem cartão |
| Crédito Azure Students | **quase intocado** | fica de reserva pra escalar depois |

Sem surpresa de fatura. Ainda assim, crie um **alerta de budget** (*Portal → Cost Management → Budgets*, ~$5) só pra garantir que nada saiu do grant.

---

## Checklist de segurança

- [ ] Conexão sempre com **SSL** (`sslmode=require` / `SSL Mode=Require`). Endurecer pra `VerifyFull` com o CA do Neon quando der.
- [ ] `Jwt__Secret` gerado com `openssl rand -base64 48`, **só** via env/secret do Container App — nunca commitado. `appsettings.json` mantém placeholders.
- [ ] `ConnectionStrings__Default` só via env/secret (idealmente `secretref:` — Parte C.2). Senha do Neon nunca no repo.
- [ ] Credenciais do Neon guardadas fora do git; se vazar, **rotacione** no Neon Console (*Roles → Reset password*).
- [ ] Ingress do Container App é **external** (público, HTTPS) — é a única porta exposta; o banco (Neon) só aceita conexão com credencial + SSL.
- [ ] Alerta de **budget** no Azure ativo.

---

## Plano de saída / evolução

O deploy é portátil — **mesmo `Dockerfile`, mesmas `.migrations/`, mesmos env vars**:

- **Banco cresceu do free do Neon?** Suba de plano no Neon, ou migre pro **Aiven free** (1 GB, tem São Paulo) ou pro **Oracle Always Free + Coolify** (`docs/deploy-coolify-oracle.md`, self-host permanente). Só troca `ConnectionStrings__Default` e reroda as migrations.
- **App estourou o grant do Container Apps?** Aí sim vale gastar o crédito Students, ou mover a API pro Oracle (mesmo Dockerfile).

---

## Alternativa — banco no Aiven (se latência importar)

Neon não tem região no Brasil. Se a latência do banco pesar, **Aiven** tem **São Paulo** e também é PG18 free (1 GB, sem cartão):

1. `aiven.io` → **Create service → PostgreSQL → versão 18 → plano Free → região AWS São Paulo**.
2. Pegue a **Service URI** (`postgres://avnadmin:...@pg-xxx.aivencloud.com:PORTA/defaultdb?sslmode=require`) — Aiven usa **porta própria** (não 5432) e é **vanilla, sem pooler** (mais simples pra migrations feitas à mão).
3. Migrations e connection string iguais à Parte A/C, só trocando host/porta:
   ```
   ConnectionStrings__Default=Host=pg-xxx.aivencloud.com;Port=<porta>;Database=sololife;Username=avnadmin;Password=<pass>;SSL Mode=Require;Trust Server Certificate=true
   ```

---

## Trabalho futuro

- **Migrator real:** trocar a aplicação manual dos `.sql` por DbUp/Grate rodado no boot da API (idempotente, versionado).
- **CI/CD:** GitHub Actions → `az containerapp up` no push pra `main`.
- **Key Vault:** mover `Jwt__Secret`/connection string pra Azure Key Vault referenciado pelo Container App.
- **Painel desktop `.exe` (.NET):** mission control próprio — **adiado para depois do MVP**.
