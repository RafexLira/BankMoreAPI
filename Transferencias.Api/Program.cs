using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SQLitePCL;
using System.Data;
using System.Text;
using Transferencias.Application.Commands;
using Transferencias.Application.Security;
using Transferencias.Domain.Entities.Repositories;
using Transferencias.Infra.Persistence;
using Transferencias.Infra.Repositories;

Batteries.Init(); // NECESSÁRIO para testes de integração com SQLite

var builder = WebApplication.CreateBuilder(args);

// ---------------- INFRA E DATABASE ----------------

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");

    var connection = new SqliteConnection(connectionString);
    connection.Open();

    DatabaseInitializer.EnsureDatabaseCreated(connection);

    return connection;
});

// Repositórios
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

// ---------------- CAMADA DE APLICAÇÃO ----------------

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterTransferenciaCommand).Assembly);
});

// ---------------- APRESENTAÇÃO ----------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Transferencias.API", Version = "v1" });
});

// ---------------- JWT INTERNO (MICROSSERVIÇOS) ----------------

var jwtSection = builder.Configuration.GetSection("JwtInternal");
var secret = jwtSection["Secret"]!;
var issuer = jwtSection["Issuer"]!;
var audience = jwtSection["Audience"]!;

builder.Services.AddAuthentication("MicroserviceScheme")
    .AddJwtBearer("MicroserviceScheme", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
        };
    });

builder.Services.AddSingleton(new MicroserviceTokenService(secret, issuer, audience));

// ---------------- PIPELINE ----------------

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

public partial class Program { }
