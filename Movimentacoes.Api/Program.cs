using System.Data;
using System.Text;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movimentacoes.Application.Commands;
using Movimentacoes.Application.QueryHandlers;
using Movimentacoes.Domain.Entities.Repositories;
using Movimentacoes.Infra.Persistence;
using Movimentacoes.Infra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ------------------- DATABASE + INFRA -------------------

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config.GetConnectionString("DefaultConnection");

    var connection = new SqliteConnection(connStr);
    connection.Open();

    DatabaseInitializer.Initialize(connection);

    return connection;
});

builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

// ------------------- APPLICATION (MediatR) -------------------

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegistrarMovimentoCommand).Assembly);
});

// ------------------- CONTROLLERS -------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ------------------- SWAGGER -------------------

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movimentacoes.API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {token}"
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

// ------------------- INTERNAL JWT -------------------

var jwt = builder.Configuration.GetSection("JwtInternal");

var secret = jwt["Secret"]!;
var issuer = jwt["Issuer"]!;
var audience = jwt["Audience"]!;
var keyBytes = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

// ------------------- PIPELINE -------------------

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
