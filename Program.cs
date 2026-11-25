using DSW_3P_PR03.Data;
using DSW_3P_PR03.Repositories;
using DSW_3P_PR03.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

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

// CORS
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

// Indica a Swagger qué operaciones requieren el token Bearer.
internal sealed class BearerSecuritySchemeTransformer
    : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes =
            new Dictionary<string, IOpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                }
            };

        foreach (var (path, pathItem) in document.Paths)
        {
            // El login debe poder usarse sin token.
            if (path.Equals(
                "/api/auth/login",
                StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (pathItem.Operations is null)
                continue;

            foreach (var operation in pathItem.Operations.Values)
            {
                operation.Security ??= [];

                operation.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(
                            "Bearer",
                            document)] = []
                    });
            }
        }

        return Task.CompletedTask;
    }
}