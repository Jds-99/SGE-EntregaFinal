using SGE.Infraestructura;
using SGE.Infraestructura.Repository;
using SGE.Aplicacion;
using SGE.Aplicacion.Tramites;
using Microsoft.EntityFrameworkCore;
using SGE.Aplicacion.Expedientes;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using SGE.WebApi.ManejadorDeExcepciones;
using SGE.Aplicacion.Token;
using SGE.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims; // Necesario para leer el claim "ID"

Console.WriteLine("Iniciando el sistema y verificando base de datos...");

// Instanciamos el contexto. Al ejecutarse el constructor, se va a crear el archivo SQLite solo.
using (var context = new SgeContext())
{
    Console.WriteLine("¡Base de datos verificada/creada con éxito!");
}

var builder = WebApplication.CreateBuilder(args);
// A. Base de Datos (Leemos appsettings.json)
var connString = builder.Configuration.GetConnectionString("SGEDb");
builder.Services.AddDbContext<SgeContext>(opt => opt.UseSqlite(connString));
// B y C. Infraestructura y Seguridad (Scoped)
builder.Services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
builder.Services.AddScoped<ITramiteRepository, TramiteTxtRepository>();
builder.Services.AddScoped<IExpedienteRepository, ExpedienteTxtRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IAutorizacionService, AutorizacionProvisionalService>();

// IMPORTANTE: En la sección de configuración registrar el provider
// y el caso de uso
builder.Services.AddScoped<ITokenService, JwtTokenProvider>();
builder.Services.AddScoped<RegistrarUsuarioUseCase>();

// D. Capa de Aplicación (Casos de Uso)
builder.Services.AddScoped<AgregarExpedienteUseCase>();
builder.Services.AddScoped<CambiarEstadoExpedienteUseCase>();
builder.Services.AddScoped<EliminarExpedienteUseCase>();
builder.Services.AddScoped<ModificarCaratulaUseCase>();
builder.Services.AddScoped<ObtenerPorIdUseCase>();
builder.Services.AddScoped<ObtenerTodosExpedientesUseCase>();

builder.Services.AddScoped<AgregarTramiteUseCase>();
builder.Services.AddScoped<EliminarTramiteUseCase>();
builder.Services.AddScoped<ModificarTramiteUseCase>();
builder.Services.AddScoped<ObtenerTramitePorIdUseCase>();
builder.Services.AddScoped<ObtenerTramitesPorExpedienteIdUseCase>();

builder.Services.AddScoped<RegistrarUsuarioUseCase>();

// Soporte para el formato estándar de errores
builder.Services.AddProblemDetails();
// Registramos nuestro manejador personalizado
builder.Services.AddExceptionHandler<ManejadorDeExcepcionesGlobales>();

// Construimos la aplicación y cerramos la fase de configuración
builder.Services.AddOpenApi();


// En el bloque superior donde registramos los servicios)
// Le decimos a .NET que vamos a usar Autenticación por JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opciones =>
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

// Bloque 2: Simulamos un Scope temporal para inicializar SQLite
// porque EscuelaContex fue registrada como “Scoped”
using (var scope = app.Services.CreateScope()) {
var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
Sgesqlite.Inicializar(context);
}
// Bloque 3: Endpoint de Prueba (Sanity Check)
app.MapGet("/", () => "¡La API del Sistema de Gestion de Expedientes está funcionando!");


// Grupo para usuarios (Falta hacer el usecase de la lista de usuarios)
var usuariosApi = app.MapGroup("/api/usuarios");

app.Run(); // Arranca el servidor web (Kestrel)
