# SoftMind API

Back-end em .NET 8 com autenticação JWT, persistência NoSQL (MongoDB via EF Core provider) e arquitetura em camadas (Controllers, Services, Repositories, Data).

Sumário
- Visão geral e arquitetura
- Stack técnica
- Executando o projeto
- Configuração (appsettings)
- Endpoints da API
- Modelo de dados (NoSQL)
- Autenticação e autorização


## Visão geral e arquitetura

Responsabilidades separadas por camadas:
- Controllers: expõem os endpoints HTTP, realizam validações básicas e delegam para serviços.
- Services: concentram a regra de negócio e orquestram repositórios.
- Repositories: acesso a dados (MongoDB) via `MongoDbContext` (EF Core provider).
- Data: `MongoDbContext` mapeia entidades para coleções.

Diagrama (alto nível)

```
Cliente HTTP
		|
		v
Controllers (API)
		|
		v
Services (regras de negócio)
		|
		v
Repositories (acesso a dados)
		|
		v
MongoDbContext  ->  MongoDB (coleções)
```

Principais componentes
- Autenticação via JWT (`TokenService`, `AuthService`), com claims como `android_id` e `user_type`.
- Identificação de dispositivo via header `x-device-id` em vários endpoints.
- Registros de humor, alertas, templates de alertas, mensagens de bem-estar e respostas de questionário por dispositivo.


## Stack técnica
- .NET 8 / ASP.NET Core Web API
- JWT (Microsoft.AspNetCore.Authentication.JwtBearer)
- MongoDB com EF Core provider (`MongoDB.EntityFrameworkCore`)
- Swagger/OpenAPI em Development


## Executando o projeto

Pré-requisitos: .NET 8 SDK, MongoDB acessível, appsettings configurado.

1) Restaurar e compilar
```
dotnet restore SoftMindApi/SoftMindApi.csproj
dotnet build   SoftMindApi/SoftMindApi.csproj -c Debug
```

2) Executar
```
dotnet run --project SoftMindApi/SoftMindApi.csproj
```

Ambiente Development expõe Swagger em `/swagger`.

## Como executar localmente com Docker

1. Copie o arquivo `.env.example` para `.env` e confirme os valores necessários. O template já aponta para o cluster MongoDB de testes (`Cluster0`) utilizado pela aplicação; ajuste apenas se for usar outro ambiente.
2. Construa e suba os serviços:
	```
	docker compose up -d --build
	```
3. A API ficará disponível em `http://localhost:8080` (Swagger em `/swagger`). Consulte os logs com `docker compose logs -f api`.
4. Para desligar os serviços:
	```
	docker compose down
	```
	Como o banco é remoto, não há volumes locais para limpar.

## Pipeline CI/CD

- Ferramenta: GitHub Actions (`.github/workflows/ci-cd.yml`).
- Gatilhos principais: `pull_request` (build + testes), `push` na branch `master` (build/test + imagem + deploy em staging), tags `v*` (promove para produção) e `workflow_dispatch` manual.
- Jobs:
	- **Build & Test**: restaura, compila e executa `dotnet test` com cobertura, publicando artefatos.
	- **Build & Push Docker Image**: monta a imagem com Docker Buildx e publica no GHCR (`ghcr.io/${owner}/${repo}`).
	- **Deploy to Staging**: acessa o host via SSH e executa `docker compose pull/up` usando os arquivos remotos.
	- **Deploy to Production**: fluxo semelhante, condicionado a tags `v*` e ao ambiente protegido `production`.
- Secrets necessários:
	- `REGISTRY_USERNAME` / `REGISTRY_PASSWORD`: credenciais para `docker login` (PAT com escopo `write:packages`).
	- `STAGING_SSH_HOST`, `STAGING_SSH_USER`, `STAGING_SSH_KEY`, `STAGING_WORKDIR`.
	- `PRODUCTION_SSH_HOST`, `PRODUCTION_SSH_USER`, `PRODUCTION_SSH_KEY`, `PRODUCTION_WORKDIR`.
	- `STAGING_SMOKE_URL`, `STAGING_SMOKE_USERNAME`, `STAGING_SMOKE_PASSWORD`, `STAGING_SMOKE_DEVICE_ID`.
	- `PRODUCTION_SMOKE_URL`, `PRODUCTION_SMOKE_USERNAME`, `PRODUCTION_SMOKE_PASSWORD`, `PRODUCTION_SMOKE_DEVICE_ID`.
	- `SMOKE_ANDROID_ID` (compartilhado entre ambientes ou configure versões específicas conforme necessário).
- Os jobs de deploy são executados apenas quando o secret `STAGING_SSH_HOST` (ou `PRODUCTION_SSH_HOST`) estiver definido; isso permite testar build/test e publicação mesmo sem infraestrutura provisionada.
- Pré-requisitos nos servidores: Docker + Docker Compose instalados, diretório (ex.: `/opt/softmind-api`) com `docker-compose.yml`, `.env.<ambiente>` e permissões para o usuário do deploy.
- Auxílio: use `scripts/deploy/sync-compose.sh <ambiente>` para sincronizar `docker-compose.yml` e o arquivo `.env` (copiado de `deploy/<env>/.env.<env>.example`) para os servidores remotos.
- Smoke tests: após cada deploy o workflow executa `scripts/smoke-tests.sh`, que valida login, verificação de token e consulta de alertas via HTTP. Execute localmente com:
	```
	SMOKE_BASE_URL=http://localhost:5000 \
		SMOKE_USERNAME=softmind_mobile \
		SMOKE_PASSWORD=SoftMind@2024!Secure#Pass$ \
	./scripts/smoke-tests.sh
	```


## Testes

Projeto de testes: `SoftMindApi.Tests`

Stack de testes
- xUnit (framework de testes)
- Moq (mocks)
- FluentAssertions (asserts fluentes)
- coverlet.collector (cobertura)

Executar testes com cobertura
```
dotnet test SoftMindApi.Tests --collect:"XPlat Code Coverage"
```

Onde encontrar a cobertura
- Um arquivo Cobertura XML é gerado em:
	- `SoftMindApi.Tests/TestResults/<GUID>/coverage.cobertura.xml`

Relatório HTML (opcional)
```
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator \
	-reports:SoftMindApi.Tests/TestResults/**/coverage.cobertura.xml \
	-targetdir:coveragereport \
	-reporttypes:Html

# Abra coveragereport/index.html no navegador
```

## Configuração (variáveis e appsettings)

- `SoftMindApi/appsettings.json` mantém os valores de teste fornecidos pela equipe (cluster MongoDB hospedado no Atlas e credenciais de homologação). Para staging/produção, sobreponha os valores via variáveis de ambiente.
- Copie `.env.example` para `.env` e preencha:
	- `ConnectionStrings__ConnectionString` (string de conexão MongoDB)
	- `ConnectionStrings__DatabaseName`
	- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpiryDays`
	- `ApiCredentials__Username`, `ApiCredentials__Password`
- Durante o desenvolvimento, você pode exportar essas variáveis no shell ou usar o suporte nativo do `dotnet` (`dotnet user-secrets`) e do Docker Compose (`--env-file`).
- Os valores padrão do template apontam para o cluster MongoDB de testes (`Cluster0`) e para as credenciais de homologação; altere-os quando for promover para outro ambiente.

Exemplo (parcial) do `appsettings.json`:
```json
{
	"ConnectionStrings": {
		"ConnectionString": "mongodb+srv://rm556212_db_user:NCqRkBCo86QUBL59@cluster0.mryolxz.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0",
		"DatabaseName": "softMind"
	},
	"Jwt": {
		"Issuer": "SoftMindApi",
		"Audience": "SoftMindApp",
		"Key": "MinhaChaveJWT2024!Segura#Android$API@Secreta%2025&Forte*",
		"ExpiryDays": 365
	},
	"ApiCredentials": {
		"Username": "softmind_mobile",
		"Password": "SoftMind@2024!Secure#Pass$"
	}
}
```


## Endpoints da API

Notas gerais
- Base: `/api`
- Autenticação: Bearer JWT, exceto login.
- Cabeçalho de dispositivo: `x-device-id` quando indicado.

Auth
- POST `/api/Auth/login` (público)
	- Body: `{ "Username": string, "Password": string, "AndroidId"?: string }`
	- 200: `{ Token, AndroidId, ExpiresAt, Message }`
	- 401: `{ message: "Credenciais inválidas" }`
- POST `/api/Auth/refresh` ([Authorize])
	- 200: `{ Token, AndroidId, ExpiresAt, Message }`
- GET `/api/Auth/verify` ([Authorize])
	- 200: `{ valid: true, username, androidId, userType }`

Alert ([Authorize])
- GET `/api/Alert/GetRandomAlert`
	- Headers: `x-device-id: string`
	- 200: `AlertDTO { Id, Message, Category, CreatedAt, IsRead }` ou `{ Message: "Nenhum novo alerta disponível" }`
- GET `/api/Alert/GetRecentAlerts`
	- Headers: `x-device-id: string`
	- 200: `AlertDTO[]` ou `{ Message: "Nenhum alerta encontrado" }`
- POST `/api/Alert/MarkAsRead`
	- Headers: `x-device-id: string`
	- Body: `{ AlertId: string }`
	- 200: `{ Message, AlertId }` | 404 se não encontrado
- POST `/api/Alert/CreateAlert`
	- Headers: `x-device-id: string`
	- Body: `{ Message: string, Category: string }`
	- 200: `AlertDTO`

AlertTemplate ([Authorize])
- POST `/api/AlertTemplate/Create`
	- Body: `{ Message: string, Category: string }`
	- 200: `AlertTemplateDTO { Id, Message, Category }`
- GET `/api/AlertTemplate/List`
	- 200: `AlertTemplateDTO[]`
- DELETE `/api/AlertTemplate/Delete/{id}`
	- 200: `{ Message }` | 404 se não encontrado

Mood ([Authorize])
- GET `/api/Mood/GetMoodLastSevenDays`
	- Headers: `x-device-id: string`
	- 200: `Mood[]` (Name, DeviceId, Data) ou `{ Message: "Nenhum registro encontrado" }`
- POST `/api/Mood/AddMood`
	- Headers: `x-device-id: string`
	- Body: `string` (conteúdo puro do nome/emoji)
	- 200: `Mood` criado

WellnessMessages ([Authorize])
- GET `/api/WellnessMessages/GetRandom`
	- Headers: `x-device-id: string`
	- 200: `WellnessMessageDTO[] { Id, Name }` | 204 sem conteúdo

CategoryQuestionnaire ([Authorize])
- GET `/api/CategoryQuestionnaire/GetCategoryQuestionnaire`
	- 200: `CategoryQuestionnaire[]` ou `{ Message: "Nenhum questionario encontrado" }`
- POST `/api/CategoryQuestionnaire/AddResponseQuestionnaire`
	- Headers: `x-device-id: string`
	- Body: `ResponseQuestionnaireDTO[]` (cada `{ pergunta, resposta }`)
	- 200: `ResponseQuestionnaire[]` persistidos


## Modelo de dados (NoSQL)

Coleções e documentos (principais campos):

- CategoryQuestionnaire ("CategoryQuestionnaire")
	- `{ _id: ObjectId, Name?: string, Questions?: [{ QuestionText?: string, ResponseOptions?: string[] }] }`

- ResponseQuestionnaire ("ResponseQuestionnaire")
	- `{ _id: ObjectId, DeviceId: string, pergunta: string, resposta: string, Data: Date }`

- WellnessMessages ("WellnessMessages")
	- `{ _id: ObjectId, Name: string, Active: bool, CreatedAt: Date, ReadStats: [{ DeviceId: string, Count: number, LastReadAt: Date }] }`

- User ("User")
	- `{ _id: ObjectId, DeviceId: string }`

- Mood ("Mood")
	- `{ _id: ObjectId, Name: string, DeviceId: string, Data: Date }`

- Alert ("Alert")
	- `{ _id: ObjectId (string na API), DeviceId: string, Message: string, Category: string, CreatedAt: Date, IsRead: bool }`

- AlertTemplates ("AlertTemplates")
	- `{ _id: ObjectId (string na API), Message: string, Category: string }`

Mapeamentos no `MongoDbContext`
- Coleções nomeadas com `ToCollection(...)`.
- Tipos embedados com `OwnsMany(...)`: `CategoryQuestionnaire.Questions` e `WellnessMessage.ReadStats`.


## Autenticação e autorização

- JWT Bearer configurado em `Program.cs` (Issuer/Audience/Key de `appsettings`).
- Validações: emissor, audiência, tempo de vida, chave de assinatura; `ClockSkew = 0`.
- Emissão do token (`TokenService`): inclui claims `android_id` (quando enviado), `user_type`, `jti`, `iat` e `Name` fixo (`softmind_app`).
- Login (`AuthService`): valida `Username/Password` contra `ApiCredentials` (configuração) e emite token.
- Autorização: todos os controllers exigem `[Authorize]` exceto `Auth/login`.
- Identidade de dispositivo: vários endpoints requerem header `x-device-id`.

Boas práticas sugeridas
- Criar índices em coleções de alto uso (ex.: `Alert` por `{ DeviceId: 1, CreatedAt: -1 }`, `Mood` por `{ DeviceId: 1, Data: -1 }`, `WellnessMessages` por `{ Active: 1 }`).
- Padronizar objetos de erro e mensagens.
- Rate limiting nos endpoints com seleção aleatória.


## Estrutura de pastas (resumo)

```
SoftMindApi/
	Controllers/
	Services/
		Interface/
	Repositories/
		Interface/
	Data/
	DTO/
	Entities/
	Program.cs
```

---

Qualquer dúvida, abra uma issue ou PR.
