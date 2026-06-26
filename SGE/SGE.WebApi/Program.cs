using SGE.Infraestructura;
using SGE.Aplicacion;
using Scalar.AspNetCore;
using SGE.WebApi.ManejadorDeExcepciones;
using SGE.Aplicacion.Token;
using SGE.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SGE.WebApi.Endpoints;

Console.WriteLine("Iniciando el sistema...");

var builder = WebApplication.CreateBuilder(args);

// Registrar el proveedor de Tokens JWT
builder.Services.AddScoped<ITokenService, JwtTokenProvider>();

// Soporte para el formato estándar de errores (RFC 7807)
builder.Services.AddProblemDetails(); 

// Registramos el manejador personalizado de excepciones globales 🌟 (¡Vuelve a estar activo!)
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>(); 

// Construimos las dependencias de las capas del sistema
builder.Services.AddOpenApi().AddAplicacion().AddInfraestructura(builder.Configuration);

// Configuración del servicio de Autenticación por JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opciones =>
{
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, 
        ValidateAudience = true, 
        ValidateLifetime = true, 
        ValidateIssuerSigningKey = true, 
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// 🌟 BLOQUE DE INICIALIZACIÓN: Forzamos a EF Core a crear las tablas faltantes usando el Scope correcto
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
    // Si el archivo SGE.sqlite no existe o le faltan las tablas, las crea e inserta la Seed Data ahora
    context.Database.EnsureCreated(); 
}
Console.WriteLine("¡Base de datos y tablas verificadas/creadas con éxito!");

// Activamos el pipeline del manejador de errores global 🌟 (¡Vuelve a estar activo!)
app.UseExceptionHandler();

// "Descubre quién es" leyendo el token de la cabecera HTTP
app.UseAuthentication();

// "Decide si tiene permiso" para acceder a la ruta solicitada
app.UseAuthorization();

// Documentación de la API con OpenAPI y Scalar en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
    app.MapScalarApiReference(); 
}

// Endpoint de Prueba (Sanity Check)
app.MapGet("/", () => "¡La API del Sistema de Gestion de Expedientes está funcionando!");

// Mapeo de Endpoints de cada módulo
app.MapLoginEndpoint();
app.MapTramitesEndpoints();
app.MapExpedientesEndpoints();
app.MapUsuariosEndpoints();

app.Run(); // Arranca Kestrel
