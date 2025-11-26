using MediatR;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Transferencias.Application.Commands;
using Transferencias.Application.Security;
using Transferencias.Infra.Persistence;
using Transferencias.Infra.Repositories;
using Transferencias.Domain.Entities.Repositories;
using Transferencias.Application.Interfaces;


var builder = WebApplication.CreateBuilder(args);


// ---------------- INFRA / DATABASE ----------------

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");

    var connection = new SqliteConnection(connectionString);
    connection.Open();

    DatabaseInitializer.Initialize(connection);

    return connection;
});

// Repositório Dapper
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();


// ---------------- APPLICATION ----------------

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegistrarTransferenciaCommand).Assembly);
});


// ---------------- PRESENTATION ----------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transferencias.API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer {token interno}'"
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


// ---------------- INTERNAL JWT ----------------

var jwt = builder.Configuration.GetSection("JwtInternal");

var secret = jwt["Secret"]!;
var key = Encoding.UTF8.GetBytes(secret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });


builder.Services.AddSingleton<TokenInternoService>(sp =>
{
    return new TokenInternoService(
        secret,
        issuer: jwt["Issuer"]!,
        audience: jwt["Audience"]!
    );
});


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
