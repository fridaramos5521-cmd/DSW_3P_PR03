using DSW_3P_PR03.Data;
using DSW_3P_PR03.Repositories;
using DSW_3P_PR03.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// SQL Server
builder.Services.AddDbContext<PersonajesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection")
    )
);

builder.Services.AddScoped<IPersonajeRepository, PersonajeSqlRepository>();
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");

byte[] key = Convert.FromBase64String(
    jwt["Key"] ?? throw new InvalidOperationException(
        "Falta configurar Jwt:Key"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwt["Audience"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
    });

builder.Services.AddAuthorization();

// CORS para el cliente abierto con Live Server en el puerto 5500
builder.Services.AddCors(options =>
{
    options.AddPolicy("CharacterHubFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "CharacterHub API v1"
        );
    });
}

app.UseCors("CharacterHubFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();