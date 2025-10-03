using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SoftMindApi.Configuration;
using SoftMindApi.Data;
using SoftMindApi.Services;
using SoftMindApi.Services.Interfaces;
using SoftMindApi.Services.Interface;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionString")
    ?? throw new InvalidOperationException("ConnectionString não configurada no appsettings.json");
var databaseName = builder.Configuration.GetConnectionString("DatabaseName")
    ?? throw new InvalidOperationException("DatabaseName não configurado no appsettings.json");
builder.Services.AddDbContext<MongoDbContext>(options =>
{
    options.UseMongoDB(connectionString, databaseName);
});

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings n�o configurado no appsettings.json");

builder.Services.AddSingleton(jwtSettings);

var apiCredentials = builder.Configuration
    .GetSection("ApiCredentials")
    .Get<ApiCredentials>() ?? throw new InvalidOperationException("API credentials n�o configuradas no appsettings.json");

builder.Services.AddSingleton(apiCredentials);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertTemplateRepository, AlertTemplateRepository>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IAlertTemplateService, AlertTemplateService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMoodRepository, MoodRepository>();
builder.Services.AddScoped<IMoodService, MoodService>();
builder.Services.AddScoped<IWellnessMessageRepository, WellnessMessageRepository>();
builder.Services.AddScoped<IWellnessMessageService, WellnessMessageService>();
builder.Services.AddScoped<IResponseQuestionnaireRepository, ResponseQuestionnaireRepository>();
builder.Services.AddScoped<ICategoryQuestionnaireService, CategoryQuestionnaireService>();

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
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SoftMind API",
        Version = "v1",
        Description = "API do SoftMind com autentica��o JWT"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. \n\n" +
                      "Digite 'Bearer' [espa�o] e ent�o seu token.\n\n" +
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();