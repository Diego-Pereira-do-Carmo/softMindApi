using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SoftMindApi.Configuration;
using SoftMindApi.Data;
using SoftMindApi.Services;
using SoftMindApi.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO MONGODB (EXISTENTE) =====
var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
var databaseName = builder.Configuration.GetConnectionString("DatabaseName");
builder.Services.AddDbContext<MongoDbContext>(options =>
{
    options.UseMongoDB(connectionString, databaseName);
});

// ===== CONFIGURAÇÃO JWT (NOVO) =====
var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings não configurado no appsettings.json");

builder.Services.AddSingleton(jwtSettings);

var apiCredentials = builder.Configuration
    .GetSection("ApiCredentials")
    .Get<ApiCredentials>() ?? throw new InvalidOperationException("API credentials não configuradas no appsettings.json");

builder.Services.AddSingleton(apiCredentials);

// Registrar serviço de token
builder.Services.AddScoped<ITokenService, TokenService>();

// ===== CONFIGURAR AUTENTICAÇÃO JWT (NOVO) =====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key)
        ),
        ClockSkew = TimeSpan.Zero // Remove atraso padrão de 5 minutos
    };
});

builder.Services.AddAuthorization();

// ===== CONFIGURAÇÕES EXISTENTES =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===== SWAGGER COM SUPORTE JWT (ATUALIZADO) =====
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SoftMind API",
        Version = "v1",
        Description = "API do SoftMind com autenticação JWT"
    });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. \n\n" +
                      "Digite 'Bearer' [espaço] e então seu token.\n\n" +
                      "Exemplo: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ===== PIPELINE HTTP =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ===== IMPORTANTE: ADICIONAR AUTENTICAÇÃO (NOVO) =====
app.UseAuthentication(); // Deve vir ANTES do UseAuthorization()
app.UseAuthorization();

app.MapControllers();

app.Run();