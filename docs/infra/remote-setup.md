# Infraestrutura para Staging e Produção

## Pré-requisitos dos hosts
- Servidores Linux com Docker Engine e Docker Compose V2 instalados.
- Usuário de deploy com permissão para executar Docker (membro do grupo `docker`) e acesso via chave SSH.
- Acesso de saída (egress) liberado para o cluster MongoDB Atlas utilizado pelo projeto (`cluster0.mryolxz.mongodb.net` na porta 27017 ou 443 conforme configuração SRV).
- Porta 8080 exposta internamente para receber as requisições da API (ou outra porta definida no compose) e, externamente, um reverse proxy com TLS (ex.: Nginx ou Traefik).

## Estrutura de diretórios sugerida
```
/opt/softmind-api/
├── docker-compose.yml
├── .env                # arquivo específico do ambiente
└── logs/               # logs adicionais da aplicação (opcional)
```

### Passo a passo inicial
1. Criar o diretório base:
   ```bash
   sudo mkdir -p /opt/softmind-api && sudo chown -R $USER:$USER /opt/softmind-api
   ```
2. Copiar os arquivos necessários usando o script local:
   ```bash
   # No repositório local
   export STAGING_SSH_HOST=host.example.com
   export STAGING_SSH_USER=deploy
   export STAGING_WORKDIR=/opt/softmind-api
   export SSH_KEY_PATH=~/.ssh/id_rsa
   ./scripts/deploy/sync-compose.sh staging
   ```
   Repita o processo para produção utilizando as variáveis `PRODUCTION_*`.
3. Ajustar o arquivo `.env` no servidor com as credenciais reais (connection string do Atlas, JWT, credenciais da API). Verifique também as regras de firewall/IP allowlist no MongoDB Atlas para autorizar o IP do servidor.

## Configuração de rede e proxy
- Configure um reverse proxy (Nginx ou Traefik) para encaminhar `https://staging.seudominio.com` → `http://localhost:8080`.
- Exemplos de trechos para Nginx:
  ```nginx
  server {
      listen 80;
      listen 443 ssl;
      server_name staging.seudominio.com;

      ssl_certificate     /etc/letsencrypt/live/staging.seudominio.com/fullchain.pem;
      ssl_certificate_key /etc/letsencrypt/live/staging.seudominio.com/privkey.pem;

      location / {
          proxy_pass         http://127.0.0.1:8080;
          proxy_set_header   Host $host;
          proxy_set_header   X-Real-IP $remote_addr;
          proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
          proxy_set_header   X-Forwarded-Proto $scheme;
      }
  }
  ```
- Certifique-se de que somente o host autorizado acesse o cluster MongoDB (configuração de IP allowlist no Atlas). No servidor, restrinja o acesso público à API via reverse proxy/Firewall.

## Tarefas pendentes
- Coordenar com o time de dados a estratégia de backup do cluster MongoDB Atlas (snapshots ou automação própria).
- Configurar monitoração (Prometheus/Grafana ou logs centralizados) conforme a política da equipe.
- Validar certificados TLS e renovar automaticamente (Certbot, Traefik Let’s Encrypt).
- Registrar os endpoints públicos de staging e produção para utilização nos smoke tests.
