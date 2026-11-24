using System.Text;
using Contratos.Data;
using Contratos.Factories;
using Contratos.Interfaces;
using Contratos.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebApiWeb.Factories;
using WebApiWeb.Interfaces;
using WebApiWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
var db = builder.Configuration.GetConnectionString("ConexionDB");
builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(db));

builder.Services.AddControllers();
builder.Services.AddScoped<IJefeInterface, JefeService>();
builder.Services.AddScoped<IProyectoInterface, ProyectoService>();
builder.Services.AddScoped<IAuthInterface, AuthService>();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "clave-de-desarrollo-proyectos-2026-0123456789";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ProyectosApi",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ProyectosWeb",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

// OpenAPI con esquema Bearer y candados
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((documento, _, _) =>
    {
        documento.Components ??= new OpenApiComponents();
        documento.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        documento.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Pega el token JWT que devuelve /api/Auth/login"
        };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operacion, contexto, _) =>
    {
        var metadata = contexto.Description.ActionDescriptor.EndpointMetadata;
        var requiereToken = metadata.OfType<IAuthorizeData>().Any()
                            && !metadata.OfType<IAllowAnonymous>().Any();

        if (requiereToken)
        {
            operacion.Security ??= new List<OpenApiSecurityRequirement>();
            operacion.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", contexto.Document, null)] = new List<string>()
            });
        }

        return Task.CompletedTask;
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API Proyectos v1");
    });
}

// Evita el 307 http->https que hace perder el header Authorization
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Datos de ejemplo
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BaseDatos>();
    DatosFactories.Seed(context);
    UsuarioFactories.Seed(context);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
