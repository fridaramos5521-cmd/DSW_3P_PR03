using DSW_3P_PR03.Data;
using DSW_3P_PR03.Repositories;
using DSW_3P_PR03.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<PersonajesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection")
    )
);

builder.Services.AddScoped<IPersonajeRepository, PersonajeSqlRepository>();
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

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

app.MapControllers();

app.Run();