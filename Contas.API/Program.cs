using Contas.Application.Commands;
using Contas.Application.Security;
using Contas.Domain.Entities.Repositories;
using Contas.Infra.Persistence;
using Contas.Infra.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;

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

// Repositórios (Dapper)
builder.Services.AddScoped<IContaCorrenteReadRepository, ContaCorrenteReadRepository>();
builder.Services.AddScoped<IContaCorrenteWriteRepository, ContaCorrenteWriteRepository>();

builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();



// ---------------- CAMADA DE APLICAÇÃO ----------------

// MediatR — Contas.Application
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Contas.Application.Commands.RegisterContaCorrenteCommand).Assembly);
});


// ---------------- APRESENTAÇÃO ----------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contas.API", Version = "v1" });
  
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer {token}'"
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

//------------------------JWT--------------------

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretBase64 = jwtSection["Secret"]!;
var key = Convert.FromBase64String(secretBase64);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key) 
        };
    });

builder.Services.AddSingleton<TokenService>(sp =>
{
    return new TokenService(
        secretBase64,
        int.Parse(jwtSection["ExpiryMinutes"] ?? "60"),
        jwtSection["Issuer"]!,
        jwtSection["Audience"]!
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

//app.Use(async (context, next) =>
//{
//   
//    if (context.Request.Path.StartsWithSegments("/api"))
//    {
//        if (context.Request.Headers.ContainsKey("Authorization"))
//        {
//            var authHeader = context.Request.Headers["Authorization"].ToString();
//            Console.WriteLine($" TOKEN RECEBIDO: {authHeader}");
//        }
//        else
//        {
//            Console.WriteLine($" SEM TOKEN - Header Authorization faltando");
//        }
//    }

//    await next();
//});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }