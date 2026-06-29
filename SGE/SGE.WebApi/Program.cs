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
using SGE.Aplicacion.Tramites;
using Microsoft.AspNetCore.Mvc;
Console.WriteLine("Iniciando el sistema...");

var builder = WebApplication.CreateBuilder(args);

// Registrar el proveedor de Tokens JWT
builder.Services.AddScoped<ITokenService, JwtTokenProvider>();

// Soporte para el formato estándar de errores (RFC 7807)
builder.Services.AddProblemDetails(); 

// Registramos el manejador personalizado de excepciones globales  (¡Vuelve a estar activo!)
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

//  BLOQUE DE INICIALIZACIÓN: Forzamos a EF Core a crear las tablas faltantes usando el Scope correcto
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SgeContext>();
    var services = scope.ServiceProvider;
    // Si el archivo SGE.sqlite no existe o le faltan las tablas, las crea e inserta la Seed Data ahora
    context.Database.EnsureCreated(); 
    //herramientas del sistema
    var usuarioRepository = services.GetRequiredService<IUsuarioRepository>();
    var passwordHasher = services.GetRequiredService<IPasswordHasher>();
    // Función auxiliar local para asignarle el ID por atrás a la entidad de Dominio
    // 📌 Función auxiliar local corregida y sin errores de compilación
void AsignarIdFijo(Usuario u, Guid idFijo)
{
    var tipoActual = typeof(Usuario);
    
    // 1. Intentamos buscar la propiedad 'Id' (buscando en mayúscula y minúscula)
    System.Reflection.PropertyInfo? propiedadId = null;
    while (tipoActual != null && propiedadId == null)
    {
        propiedadId = tipoActual.GetProperty("Id") ?? tipoActual.GetProperty("id");
        tipoActual = tipoActual.BaseType!; // Si no está acá, sube a la clase base (ej: Entidad)
    }

    if (propiedadId != null && propiedadId.CanWrite)
    {
        propiedadId.SetValue(u, idFijo);
        return;
    }

    // 2. Si no pudimos por propiedad (porque es de solo lectura), modificamos el campo privado por atrás
    tipoActual = typeof(Usuario);
    System.Reflection.FieldInfo? campoId = null;
    while (tipoActual != null && campoId == null)
    {
        campoId = tipoActual.GetField("_id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                  ?? tipoActual.GetField("id", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        tipoActual = tipoActual.BaseType!;
    }

    campoId?.SetValue(u, idFijo);
}
    //verificamos si el admin existe en bd
    var adminExistente = usuarioRepository.ObtenerPorCorreo("admin@sge.com");
    if(adminExistente == null)
    {
        //  Instanciamos el objeto REAL de tu dominio (Acomodá los parámetros según tu constructor)
        var nuevoAdmin = new Usuario("Admin inicial", "admin@sge.com", passwordHasher.HashPassword("admin123"), true);
        
        // Le clavamos el ID fijo de prueba
        AsignarIdFijo(nuevoAdmin, Guid.Parse("00000000-0000-0000-0000-000000000001"));
        
        context.Set<Usuario>().Add(nuevoAdmin); // Ahora sí va una entidad real
        
    }
    // 2. VERIFICAMOS E INSERTAMOS EL OPERADOR 1
    var op1Existente = usuarioRepository.ObtenerPorCorreo("operador1@sge.com");
    if (op1Existente == null)
    {
        var nuevoOp1 = new Usuario("Operador Uno", "operador1@sge.com", passwordHasher.HashPassword("123operador"), false);
        AsignarIdFijo(nuevoOp1, Guid.Parse("00000000-0000-0000-0000-000000000002"));
        context.Set<Usuario>().Add(nuevoOp1);
    }

    // 3. VERIFICAMOS E INSERTAMOS EL OPERADOR 2
    var op2Existente = usuarioRepository.ObtenerPorCorreo("operador2@sge.com");
    if (op2Existente == null)
    {
        var nuevoOp2 = new Usuario("Operador Dos", "operador2@sge.com", passwordHasher.HashPassword("123operador"), false);
        AsignarIdFijo(nuevoOp2, Guid.Parse("00000000-0000-0000-0000-000000000003"));
        context.Set<Usuario>().Add(nuevoOp2);
    }
    context.SaveChanges();
}
Console.WriteLine("¡Base de datos y tablas verificadas/creadas con éxito!");

// Activamos el pipeline del manejador de errores global  (¡Vuelve a estar activo!)
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

// ==========================================
// 👥 MÓDULO DE USUARIOS 
// ==========================================
var usuariosApi = app.MapGroup("/api/usuarios").WithTags("Gestión de Usuarios");

// 1. POST: Registrar un usuario nuevo (Alta)
usuariosApi.MapPost("/", (RegistrarUsuarioRequest request,[FromServices] RegistrarUsuarioUseCase useCase) =>
{
    useCase.Ejecutar(request);
    return Results.Created($"/api/usuarios", new { Mensaje = "Usuario registrado exitosamente." });
}).RequireAuthorization(); // Protegido: Solo usuarios logueados (o admins) deberían crear usuarios

// 2. POST: Login de Usuario (PÚBLICO)
// Nota: A este NO le ponemos .RequireAuthorization() porque cualquiera tiene que poder loguearse.
usuariosApi.MapPost("/login", (LoginRequest request,[FromServices] LoginUseCase useCase) =>
{
    var response = useCase.Ejecutar(request);
    return Results.Ok(response); // Devuelve el token, nombre, etc.
});

// 3. PUT: Modificar Mis Datos (PROTEGIDO)
usuariosApi.MapPut("/mis-datos", (System.Security.Claims.ClaimsPrincipal user, ModificarMisDatosBody body,[FromServices]ModificarMisDatosUseCase useCase) =>
{
    // Extraemos el ID del usuario desde el Token JWT
    var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst("id")?.Value;

    if (string.IsNullOrEmpty(userIdClaim))
    {
        return Results.Problem(detail: "Token inválido o ausente.", statusCode: 401);
    }

    var idUsuarioLogueado = Guid.Parse(userIdClaim);

    // Armamos el Request que el Caso de Uso exige, inyectando el ID del token en ambos campos de ID 
    var request = new ModificarMisDatosRequest(
        UserIdDesdeToken: idUsuarioLogueado,
        UserIdAModificar: idUsuarioLogueado, // Como modifica sus PROPIOS datos, pasamos el mismo ID
        NuevoNombre: body.NuevoNombre,
        NuevoCorreo: body.NuevoCorreo,
        NuevacontraseniaPura: body.NuevacontraseniaPura
    );

    //  Ejecutamos el Caso de Uso (que es void, no devuelve nada)
    useCase.Ejecutar(request);

    // Respondemos con éxito
    return Results.Ok(new { Mensaje = "Tus datos se actualizaron correctamente." });
}).RequireAuthorization();

/// 4. GET: Listar todos los usuarios pasándole el OperadorId desde el Token
usuariosApi.MapGet("/", (System.Security.Claims.ClaimsPrincipal user, [FromServices]ListarUsuariosUseCase useCase) =>
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

// 4. DELETE: Eliminar un usuario por su ID usando tu clase Request real
usuariosApi.MapDelete("/{id:guid}", (Guid id, System.Security.Claims.ClaimsPrincipal user,[FromServices] EliminarUsuarioUseCase useCase) =>
{
    // Extraemos el ID del administrador/operador desde el Token
    var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst("id")?.Value;
                      
    if (string.IsNullOrEmpty(userIdClaim))
    {
        return Results.Json(new { detail = "Token inválido o ausente." }, statusCode: 401);
    }

    var idUsuarioOperador = Guid.Parse(userIdClaim);

    // Instanciamos tu clase asignando las propiedades por su nombre real 
    var req = new EliminarUsuarioRequest 
    { 
        OperadorId = idUsuarioOperador,
        UsuarioAEliminar = id // El ID que viene desde la URL
    };

    // Se lo mandamos al caso de uso
    useCase.Ejecutar(req);
    return Results.NoContent(); // 204 OK
}).RequireAuthorization();

// 5. PUT: Modificar Permisos de un Usuario (PROTEGIDO - Solo Administradores)
// Pasamos el ID del usuario a modificar por la URL (ej: /api/usuarios/5/permisos)
usuariosApi.MapPut("/{id:guid}/permisos", (Guid id, System.Security.Claims.ClaimsPrincipal user, ModificarPermisosBody body,[FromServices]ModificarPermisosUsuarioUseCase useCase) =>
{
    // 1. Extraemos el ID del Administrador (Operador) desde el Token JWT
    var adminIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst("id")?.Value;

    if (string.IsNullOrEmpty(adminIdClaim))
    {
        return Results.Problem(detail: "Token inválido o ausente.", statusCode: 401);
    }

    var idAdminOperador = Guid.Parse(adminIdClaim);

    // 2. Armamos el Request completo que exige tu Caso de Uso 
    var request = new ModificarPermisosRequest
    {
        UsuarioId = id,                         // Tomado del parámetro de la URL
        NuevosPermisos = body.NuevosPermisos,   // Tomado del Body enviado por Postman
        IdUsuarioOperador = idAdminOperador     // Tomado del Token JWT de forma segura
    };

    // 3. Ejecutamos el Caso de Uso y capturamos su Response
    var response = useCase.Ejecutar(request);

    // 4. Retornamos el resultado del caso de uso
    return Results.Ok(response);
}).RequireAuthorization();
// ==========================================
// 📂 MÓDULO DE EXPEDIENTES 
// ==========================================
var expedientesApi = app.MapGroup("/api/expedientes").WithTags("Gestión de Expedientes");

// 1. GET: Listar todos (El que te dio la profe)
expedientesApi.MapGet("/", ([FromServices]ObtenerTodosExpedientesUseCase useCase) =>
{
    var res = useCase.Ejecutar(); 
    return Results.Ok(res);
});

// 2. POST: Crear un expediente 
expedientesApi.MapPost("/", ( AgregarExpedienteRequest request,[FromServices] AgregarExpedienteUseCase useCase) =>
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
// 3. PUT Cambiar Estado
        expedientesApi.MapPut("/cambiar-estado", (
            CambiarEstadoExpedienteRequest request, 
            [FromServices]CambiarEstadoExpedienteUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();
// 4. DELETE: Eliminar un expediente recibiendo una Request
expedientesApi.MapDelete("/{id:guid}", (Guid id,[FromServices] EliminarExpedienteUseCase useCase) =>
{
    // Armamos el objeto Request que exige el método Ejecutar
    var req = new EliminarExpedienteRequest(id.ToString(),"Usuario"); 
    
    // Si tu objeto no es un record con constructor, sino una clase común, se armaría así:
    // var req = new EliminarExpedienteRequest { Id = id };

    useCase.Ejecutar(req);
    return Results.NoContent(); // HTTP 204: Todo salió bien, pero no hay contenido que devolver
});
 // 5. PUT Modificar Carátula
        expedientesApi.MapPut("/modificar-caratula", (
            ModificarCaratulaRequest request, 
             [FromServices]ModificarCaratulaUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();
// 5. GET Trámites por Expediente
        expedientesApi.MapGet("/{expedienteId:guid}/tramites", (
            Guid expedienteId, 
            System.Security.Claims.ClaimsPrincipal user, 
           [FromServices]ObtenerPorIdUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            
            var request = new ObtenerPorIdRequest(expedienteId, idUsuario);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();
// ==========================================
// MÓDULO DE TRAMITES 
// ==========================================
var tramitesApi = app.MapGroup("/api/tramites").WithTags("Gestión de Trámites");

        // 1. POST (Arreglado)
        tramitesApi.MapPost("/", (
             AgregarTramiteRequest request, 
            System.Security.Claims.ClaimsPrincipal user, 
             [FromServices]AgregarTramiteUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Created($"/api/tramites/{response.Id}", response);
        }).RequireAuthorization();

        // 2. DELETE (Arreglado)
        tramitesApi.MapDelete("/{tramiteId:guid}", (
            Guid tramiteId, 
            System.Security.Claims.ClaimsPrincipal user, 
           [FromServices] EliminarTramiteUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            var request = new EliminarTramiteRequest(tramiteId, idUsuario);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 3. PUT Modificar Trámite (Arreglado)
        tramitesApi.MapPut("/modificar-tramite", (
             ModificarTramiteRequest request, 
             [FromServices]ModificarTramiteUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 4. GET Obtener por Id 
        tramitesApi.MapGet("/{tramiteId:guid}/Tramite", (
            Guid tramiteId, 
            System.Security.Claims.ClaimsPrincipal user, 
            [FromServices] ObtenerTramitePorIdUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            var request = new ObtenerTramitePorIdRequest(tramiteId, idUsuario);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 5. GET Listar por Expediente Id 
        tramitesApi.MapGet("/Tramites", (
             [FromServices]ObtenerTramitesPorExpedienteIdUseCase useCase, 
             Guid expedienteId // Le indicamos explícitamente de dónde sale
        ) =>
        {
            var request = new ObtenerTramitesPorExpedienteIdRequest(expedienteId);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

app.Run(); // Arranca Kestrel

public record ModificarPermisosBody(List<SGE.Dominio.Usuarios.Permiso> NuevosPermisos);
public record ModificarMisDatosBody(string NuevoNombre, string NuevoCorreo, string NuevacontraseniaPura);