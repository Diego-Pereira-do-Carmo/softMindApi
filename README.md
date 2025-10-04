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


## Configuração (appsettings)

`SoftMindApi/appsettings.json`
- ConnectionStrings
	- ConnectionString: string de conexão MongoDB
	- DatabaseName: nome do banco
- Jwt
	- Issuer, Audience, Key, ExpiryDays
- ApiCredentials
	- Username, Password (credenciais para login básico via `AuthService`)

Exemplo (parcial):
```json
{
	"ConnectionStrings": {
		"ConnectionString": "mongodb://localhost:27017",
		"DatabaseName": "softmind"
	},
	"Jwt": {
		"Issuer": "softmind",
		"Audience": "softmind-clients",
		"Key": "CHANGE_ME_SUPER_SECRET",
		"ExpiryDays": 365
	},
	"ApiCredentials": {
		"Username": "admin",
		"Password": "admin123"
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