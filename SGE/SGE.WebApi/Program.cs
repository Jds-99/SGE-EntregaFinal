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
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Usuarios;
using Microsoft.VisualBasic;

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
    var services = scope.ServiceProvider;
    // Si el archivo SGE.sqlite no existe o le faltan las tablas, las crea e inserta la Seed Data ahora
    context.Database.EnsureCreated(); 
    //herramientas del sistema
    var usuarioRepository = services.GetRequiredService<IUsuarioRepository>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher>();
    //verificamos si el admin existe en bd
    var adminExistente = usuarioRepository.ObtenerPorCorreo("admin@sge.com");
    if(adminExistente == null)
    {
        var nuevoAdmin = new {Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                              Nombre = "Admin inicial",
                              CorreoElectronico = "admin@sge.com",
                              contraseniaHash = passwordHasher.HashPassword("admin123"),
                              EsAdministrador = true
                              }; 
        //ef core agrega este objeto en el set de usuarios
        context.Set<Usuario>().Add((dynamic)nuevoAdmin);
        context.SaveChanges();
    }
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
//app.MapExpedientesEndpoints();
//app.MapUsuariosEndpoints();

// ==========================================
// 👥 MÓDULO DE USUARIOS (Formato de la Cátedra)
// ==========================================
var usuariosApi = app.MapGroup("/api/usuarios");

// 1. POST: Registrar un usuario nuevo (Alta)
usuariosApi.MapPost("/", (RegistrarUsuarioRequest request, RegistrarUsuarioUseCase useCase) =>
{
    useCase.Ejecutar(request);
    return Results.Created($"/api/usuarios", new { Mensaje = "Usuario registrado exitosamente." });
}).RequireAuthorization(); // Protegido: Solo usuarios logueados (o admins) deberían crear usuarios

/// 2. GET: Listar todos los usuarios pasándole el OperadorId desde el Token
usuariosApi.MapGet("/", (System.Security.Claims.ClaimsPrincipal user, ListarUsuariosUseCase useCase) =>
{
    // 1. Extraemos el ID del usuario que está navegando desde su Token JWT
    var userIdClaim = user.FindFirst(System.Security.Claims.ClaimsIdentity.DefaultNameClaimType)?.Value 
                      ?? user.FindFirst("id")?.Value;

    if (string.IsNullOrEmpty(userIdClaim))
    {
        return Results.Json(new { detail = "Token inválido o ausente." }, statusCode: 401);
    }

    var idUsuarioOperador = Guid.Parse(userIdClaim);

    // 2. Fabricamos la request pasándole el Guid que exige el constructor
    var req = new ListarUsuariosRequest(idUsuarioOperador);

    // 3. Se la pasamos al ejecutar
    var response = useCase.Ejecutar(req);
    
    return Results.Ok(response);
}).RequireAuthorization(); // Candado activo

// 3. DELETE: Eliminar un usuario por su ID usando tu clase Request real
usuariosApi.MapDelete("/{id:guid}", (Guid id, System.Security.Claims.ClaimsPrincipal user, EliminarUsuarioUseCase useCase) =>
{
    // 1. Extraemos el ID del administrador/operador desde el Token
    var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst("id")?.Value;
                      
    if (string.IsNullOrEmpty(userIdClaim))
    {
        return Results.Json(new { detail = "Token inválido o ausente." }, statusCode: 401);
    }

    var idUsuarioOperador = Guid.Parse(userIdClaim);

    // 2. Instanciamos tu clase asignando las propiedades por su nombre real 🎯
    var req = new EliminarUsuarioRequest 
    { 
        OperadorId = idUsuarioOperador,
        UsuarioAEliminar = id // El ID que viene desde la URL
    };

    // 3. Se lo mandamos al caso de uso
    useCase.Ejecutar(req);
    return Results.NoContent(); // 204 OK
}).RequireAuthorization();

// ==========================================
// 📂 MÓDULO DE EXPEDIENTES (Formato de la Cátedra)
// ==========================================
var expedientesApi = app.MapGroup("/api/expedientes");

// 1. GET: Listar todos (El que te dio la profe)
expedientesApi.MapGet("/", (ObtenerTodosExpedientesUseCase useCase) =>
{
    var res = useCase.Ejecutar(); 
    return Results.Ok(res);
});

// 2. POST: Crear un expediente 
expedientesApi.MapPost("/", ( AgregarExpedienteRequest request, AgregarExpedienteUseCase useCase) =>
{
    // Supongamos que tu Request en C# recibe un IdUsuario. 
    // Si es un GUID hardcodeado de tu usuario administrador semilla, lo creamos acá:
    var idUsuarioAdmin = Guid.Parse("00000000-0000-0000-0000-000000000001"); // 

    // Armamos una nueva request que SÍ tenga el usuario responsable
    var requestConUsuario = new AgregarExpedienteRequest(request.CaratulaTxt, idUsuarioAdmin);

    // Si tu AgregarExpedienteRequest es una clase común, se haría así:
    // request.UsuarioId = idUsuarioAdmin;
    // var requestConUsuario = request;

    var expedienteCreado = useCase.Ejecutar(requestConUsuario);
    return Results.Created($"/api/expedientes/{expedienteCreado.IdExpediente}", expedienteCreado);
});

// 3. DELETE: Eliminar un expediente recibiendo una Request
expedientesApi.MapDelete("/{id:guid}", (Guid id, EliminarExpedienteUseCase useCase) =>
{
    // Armamos el objeto Request que exige el método Ejecutar
    var req = new EliminarExpedienteRequest(id.ToString(),"Usuario"); 
    
    // Si tu objeto no es un record con constructor, sino una clase común, se armaría así:
    // var req = new EliminarExpedienteRequest { Id = id };

    useCase.Ejecutar(req);
    return Results.NoContent(); // HTTP 204: Todo salió bien, pero no hay contenido que devolver
});
app.Run(); // Arranca Kestrel


