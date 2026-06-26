using SGE.Infraestructura;
using SGE.Aplicacion;
using Scalar.AspNetCore;
using SGE.WebApi.ManejadorDeExcepciones;
using SGE.Aplicacion.Token;
using SGE.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SGE.WebApi.Endpoints; // Necesario para leer el claim "ID"

Console.WriteLine("Iniciando el sistema y verificando base de datos...");

// Instanciamos el contexto. Al ejecutarse el constructor, se va a crear el archivo SQLite solo.
using (var context = new SgeContext())
{
    Console.WriteLine("¡Base de datos verificada/creada con éxito!");
}

var builder = WebApplication.CreateBuilder(args);

// IMPORTANTE: En la sección de configuración registrar el provider
builder.Services.AddScoped<ITokenService, JwtTokenProvider>();

// Soporte para el formato estándar de errores
builder.Services.AddProblemDetails();
// Registramos nuestro manejador personalizado
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>();

// Construimos la aplicación y cerramos la fase de configuración
builder.Services.AddOpenApi().AddAplicacion().AddInfraestructura(builder.Configuration);


// En el bloque superior donde registramos los servicios)
// Le decimos a .NET que vamos a usar Autenticación por JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opciones =>
{
opciones.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true, // Validar quién lo emitió
ValidateAudience = true, // Validar para quién es
ValidateLifetime = true, // Validar que no esté vencido
ValidateIssuerSigningKey = true, // ¡Vital! Validar la firma criptográfica
ValidIssuer = builder.Configuration["Jwt:Issuer"],
ValidAudience = builder.Configuration["Jwt:Audience"],
IssuerSigningKey = new SymmetricSecurityKey(
Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
};
});
// Agregamos el servicio de Autorización (para manejar los roles luego)
builder.Services.AddAuthorization();


var app = builder.Build();

// Agregamos el Middleware al principio del Pipeline
app.UseExceptionHandler();

// "Descubre quién es" leyendo el token de la cabecera HTTP
app.UseAuthentication();

// "Decide si tiene permiso" para acceder a la ruta solicitada
app.UseAuthorization();

// Solo exponemos esto en modo Desarrollo (seguridad)
if (app.Environment.IsDevelopment())
{
app.MapOpenApi(); // Genera el archivo JSON interno
app.MapScalarApiReference(); // Levanta la interfaz gráfica en /scalar
}

// Bloque 3: Endpoint de Prueba (Sanity Check)
app.MapGet("/", () => "¡La API del Sistema de Gestion de Expedientes está funcionando!");

app.MapLoginEndpoint();
app.MapTramitesEndpoints();
app.MapExpedientesEndpoints();
app.MapUsuariosEndpoints();

app.Run(); // Arranca el servidor web (Kestrel)
