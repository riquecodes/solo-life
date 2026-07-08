# Checklist — Deploy SoloLife (Oracle Always Free + Coolify + PostgreSQL 18)

> Marque `[x]` conforme avança. **(Código)** = alteração no repositório, não é clique no painel.
> Caminho crítico: **VM → hardening → Coolify → banco → migrations → API**. Fases 7–8 podem vir depois.

---

## Fase 0 — Contas e preparação

- [x] Criar conta na Oracle Cloud (cartão só pra verificação — não cobra no Always Free)
  - Credenciais guardadas fora do repo (gerenciador de senhas). **Nunca** commitar login/senha aqui.
- [x] Gerar par de chaves SSH no PC: `ssh-keygen -t ed25519`
- [ ] Ter um domínio com acesso ao painel de DNS (registros A vêm depois, quando souber o IP)
- [ ] Instalar no PC: cliente SSH + DBeaver (ou pgAdmin/TablePlus)
- [ ] (Opcional, pode deixar pra Fase 4) Criar bucket S3-compatível pros backups — Cloudflare R2 ou Backblaze B2 (tier grátis)

---

## Fase 1 — Provisionar a VM

- [x] Escolher região próxima: `sa-saopaulo-1` ou `sa-vinhedo-1`
- [ ] Compute → Instances → Create Instance:
  - [ ] Image: **Ubuntu 24.04**
  - [ ] Shape: **VM.Standard.A1.Flex → 2 OCPU / 12 GB**
  - [ ] Colar a chave pública SSH
  - [ ] Boot volume: 50–100 GB
  - [ ] Networking: VCN com subnet pública + IP público
- [ ] ⚠️ Se der **"out of capacity"** no A1: trocar de região/AD ou tentar mais tarde
- [ ] ⚠️ Confirmar a cota A1 vigente na hora (o limite 2/12 está datado no playbook — pode ter mudado)
- [ ] Security List → Ingress (Source `0.0.0.0/0`):
  - [ ] Abrir **TCP 80** e **TCP 443**
  - [ ] **TCP 22** só pro seu IP (`SEU.IP/32`)
  - [ ] **NÃO** abrir 5432
- [ ] Conectar: `ssh ubuntu@SEU_IP_PUBLICO`

---

## Fase 2 — Hardening (ANTES do Coolify)

- [ ] Atualizar o SO: `sudo apt update && sudo apt upgrade -y`
- [ ] ⚠️ **Gotcha Oracle** — liberar 80/443 antes da regra REJECT:
  ```bash
  sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 80 -j ACCEPT
  sudo iptables -I INPUT 6 -m state --state NEW -p tcp --dport 443 -j ACCEPT
  sudo netfilter-persistent save
  ```
- [ ] Criar swap de 4G:
  ```bash
  sudo fallocate -l 4G /swapfile && sudo chmod 600 /swapfile
  sudo mkswap /swapfile && sudo swapon /swapfile
  echo '/swapfile none swap sw 0 0' | sudo tee -a /etc/fstab
  ```
- [ ] Instalar fail2ban: `sudo apt install -y fail2ban`
- [ ] Endurecer SSH em `/etc/ssh/sshd_config`: `PasswordAuthentication no`, `PermitRootLogin no`
- [ ] `sudo systemctl restart ssh`
- [ ] ⚠️ **Testar login por chave em OUTRA aba ANTES de fechar a sessão atual**

---

## Fase 3 — Instalar Coolify

- [ ] Instalar: `curl -fsSL https://cdn.coollabs.io/coolify/install.sh | sudo bash`
- [ ] Acessar `http://SEU_IP:8000` → criar conta admin (senha forte)
- [ ] Ativar 2FA em Profile → Two-Factor
- [ ] (Recomendado) Criar registro A `painel.seudominio.com` → IP, depois Settings → Instance Domain → `https://painel.seudominio.com`

---

## Fase 4 — PostgreSQL 18

- [ ] + New → Database → PostgreSQL
- [ ] Trocar a Image pra `postgres:18`
- [ ] ⚠️ **Workaround PG18 (issue #7279):** Database → Configuration → Persistent Storage → trocar destino de `/var/lib/postgresql/data` pra `/var/lib/postgresql` → **Restart**
- [ ] **NÃO** habilitar "Public Port"
- [ ] Anotar a **Internal Connection URL** que o Coolify mostra
- [ ] Database → Backups → agendar (diário) com retention + destino S3

---

## Fase 5 — Aplicar as migrations

- [ ] Abrir túnel SSH: `ssh -L 5432:localhost:<porta-interna> ubuntu@SEU_IP`
- [ ] No DBeaver, conectar em `localhost:5432`
- [ ] Rodar **em ordem**: `V001__initial_schema.sql` → `V002__...` → `V003__...`
- [ ] (Alternativa) Terminal web do Coolify no container: `psql -U postgres -d sololife -f ...`

---

## Fase 6 — Deploy da API

- [ ] **(Código)** Criar o `Dockerfile` na raiz do repo e commitar (bloco .NET 10 SDK/aspnet)
- [ ] **(Código, segurança)** Remover o fallback hardcoded do `Jwt:Secret` em `Program.cs:38` — em Production o secret vem **só** da env; se faltar, falhar explícito
- [ ] No Coolify: + New → Application → repo → Build Pack: Dockerfile
- [ ] Porta exposta **8080**
- [ ] Criar registro A `api.seudominio.com` → IP, definir domínio `https://api.seudominio.com`
- [ ] Setar Environment Variables como **secret**:
  - [ ] `ASPNETCORE_ENVIRONMENT=Production`
  - [ ] `ConnectionStrings__Default` (Host = nome interno do db)
  - [ ] `Jwt__Secret` (gerar com `openssl rand -base64 48`)
  - [ ] `Jwt__Issuer`
  - [ ] `Jwt__Audience`
- [ ] Deploy

---

## Fase 7 — Validar

- [ ] Testar endpoint real (registro/login) em `https://api.seudominio.com` (Swagger só aparece em Dev)
- [ ] Coolify: container **healthy**, logs sem erro de conexão
- [ ] DBeaver (via túnel): `SELECT version();` → deve dizer **PostgreSQL 18.x**
- [ ] Ver as tabelas no schema `sololife`

---

## Fase 8 — Monitoramento (Seq)

- [ ] Subir Seq no Coolify: imagem `datalust/seq`, porta 80, env `ACCEPT_EULA=Y`
- [ ] **(Código)** `dotnet add src/SoloLife.Api package Serilog.Sinks.Seq`
- [ ] **(Código)** Em `appsettings.json`, acrescentar o sink Seq em `Serilog.WriteTo` (URL interna do Seq) → redeploy
- [ ] (Opcional) `CREATE EXTENSION pg_stat_statements;` + ajuste no `postgresql.conf` pra queries lentas

---

## Antes de considerar "pronto"

- [ ] Backup testado com **restore** de verdade (backup não testado não é backup)
- [ ] SSH só por chave, porta 22 restrita, fail2ban ativo, **5432 fechado** ao mundo
- [ ] Painel Coolify com senha forte + 2FA + HTTPS
- [ ] Secrets só em env var, nada commitado; `appsettings.json` com placeholders
- [ ] Auto-update do Coolify ligado
- [ ] Coolify + SO atualizados
