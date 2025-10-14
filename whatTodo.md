# Plano de Execução - SoftMind API

## Contexto atual
- Repositório .NET 8 com solução `SoftMindApi.sln`, projetos `SoftMindApi` (API) e `SoftMindApi.Tests`.
- Configurações sensíveis hoje estão em `SoftMindApi/appsettings*.json`; MongoDB como fonte de dados.
- Não existe pasta `src`; quaisquer automações devem trabalhar diretamente sobre as pastas `SoftMindApi` e `SoftMindApi.Tests`.

## 1. Diagnóstico e preparação
- [x] Garantir que o ambiente local tem .NET 8 SDK e Docker instalados. *(Docker disponível porém sem daemon ativo neste ambiente; validar em máquina final).*
- [x] Rodar `dotnet restore SoftMindApi.sln` para validar dependências.
- [x] Executar `dotnet test SoftMindApi.Tests/SoftMindApi.Tests.csproj` e anotar eventuais falhas que precisem ser tratadas antes da automação.
- [x] Revisar `Program.cs` e middlewares para identificar variáveis que precisam ser externalizadas (connection string, JWT, credenciais básicas).
- [x] Levantar endpoints críticos e fluxos que precisam de evidências para a documentação final.

## 2. Padronização de configurações
- [x] Criar `.env.example` na raiz com todas as chaves necessárias (Mongo, JWT, credenciais, portas).
- [x] Ajustar `appsettings.json` / `appsettings.Development.json` para ler variáveis de ambiente (mantendo valores default apenas para dev).
- [x] Atualizar `README.md` com instruções de configuração de variáveis e dependências externas (MongoDB, portas expostas).

## 3. Containerização
- [x] Escrever `Dockerfile` multi-stage na raiz (build com `mcr.microsoft.com/dotnet/sdk:8.0`, runtime com `aspnet:8.0`) apontando para a pasta `SoftMindApi`.
- [x] Validar build local com `docker build -t softmind-api:local .` e rodar `docker compose up -d --build` para garantir que o container sobe corretamente usando o cluster remoto. *(Executado manualmente em ambiente local com Docker Desktop).*
- [x] Criar `docker-compose.yml` na raiz orquestrando:
  - Serviço `api` usando a imagem gerada, carregando variáveis via `.env`.
  - Configuração direcionada ao cluster MongoDB remoto de testes (nenhum serviço de banco local é iniciado).
- [x] Documentar no README como subir e derrubar o compose (`docker compose up -d`, logs, migrações seed se aplicável).

## 4. Pipeline CI/CD
- [x] Adotar GitHub Actions (pasta `.github/workflows/ci-cd.yml`) como orquestrador.
- [x] Definir gatilhos:
  - `pull_request` para build + testes.
  - `push` na branch principal para build, testes, docker build/push, deploy staging.
  - Tag `v*` ou branch `release/*` para deploy produção.
- [x] Job de build/test:
  - `actions/checkout`.
  - `actions/setup-dotnet` (8.0.x).
  - `dotnet restore`, `dotnet build`, `dotnet test --collect:"XPlat Code Coverage"`.
  - Publicar resultados de testes/cobertura como artefatos.
- [x] Job de imagem:
  - Fazer login no registry (preferencial: GHCR).
  - `docker build` com tags `ghcr.io/<org>/softmind-api:${{ github.sha }}` e `:latest`.
  - `docker push` das tags.
- [x] Deploy staging:
  - Utilizar `environment: staging` com secrets (host, usuário SSH, chave, paths).
  - Passos: copiar `.env.staging`, baixar compose chamando `docker compose pull` + `docker compose up -d`.
  - Opcional: rodar smoke tests via `curl` ou script dotnet. *(Implementado com `scripts/smoke-tests.sh`.)*
- [x] Deploy produção:
  - Similar ao staging, porém com `environment: production`, aprovação manual e arquivo `.env.production`.
  - Incluir etapa de backup/snapshot (dump Mongo) antes de subir nova versão.
- [x] Registrar no README a descrição do pipeline e ambientes.

## 5. Infraestrutura de staging e produção
- [ ] Provisionar (ou validar) hosts Docker nos ambientes-alvo, garantindo acesso via SSH para o pipeline. *(Depende da infraestrutura real.)*
- [ ] Criar diretório remoto `/opt/softmind-api` contendo `docker-compose.yml`, `.env.<env>`, scripts auxiliares. *(Modelos criados em `deploy/`, aplicar em ambiente real.)*
- [ ] Configurar reverse proxy (Nginx/Traefik) caso necessário para expor API com HTTPS; registrar portas e certificados.
- [ ] Testar manualmente o `docker compose up -d` em staging antes de ligar o deploy automatizado.
- [ ] Documentar endpoints públicos de staging e produção para evidências.

## 6. Automação de verificações pós-deploy
- [x] Criar script `scripts/smoke-tests.sh` (ou job GitHub Actions reutilizável) validando rotas principais usando `curl`.
- [x] Adicionar etapa no pipeline para executar smoke tests após deploy em staging e produção, com política de rollback manual se falhar.
- [ ] Coletar logs (`docker logs`) e métricas básicas para anexar às evidências.

## 7. Documentação e evidências
- [x] Atualizar `README.md` com seções exigidas: execução via Docker, pipeline CI/CD, containerização, prints.
- [ ] Criar pasta `docs/` (se ainda não existir) com capturas de tela: pipeline rodando, compose ativo, API respondendo, ambientes staging/prod. *(Estrutura criada em `docs/`, capturas pendentes após execuções reais.)*
- [ ] Preparar materiais (prints, tabelas, checkpoints) que serão usados pelo time para montar manualmente o PDF/PPT final.
- [ ] Manter checklist de entrega no final do README ou do documento final, marcando cada item ao concluir.

## 8. Empacotamento final
- [ ] Gerar build final via pipeline (tag release confirmando sucesso).
- [ ] Baixar artefatos necessários (logs, coverage) e organizar em `docs/`.
- [ ] Compactar projeto em `.zip` contendo código, Dockerfile, compose, workflows, documentação e evidências.
- [ ] Conferir checklist oficial:
  - Projeto compactado organizado.
  - Dockerfile funcional.
  - docker-compose.yml ou manifests Kubernetes.
  - Pipeline com build, teste e deploy.
  - README com instruções e prints.
  - Documentação técnica (PDF/PPT) a ser compilada manualmente com os materiais preparados.
  - Deploy válido nos ambientes staging e produção.
