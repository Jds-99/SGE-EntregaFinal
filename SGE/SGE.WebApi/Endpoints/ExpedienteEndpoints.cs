using System.Security.Claims;
using SGE.Aplicacion.Expedientes;
namespace SGE.WebApi.Endpoints;
public static class ExpedientesEndpoints
{
public static void MapExpedientesEndpoints(this IEndpointRouteBuilder app)
{
// Creamos un grupo base para todo lo relacionado a los Expedientes.
var ExpedientesApi = app.MapGroup("/api/expedientes").WithTags("Gestión de Expedientes");

// POST
ExpedientesApi.MapPost("/", (AgregarExpedienteRequest request, ClaimsPrincipal user, AgregarExpedienteUseCase useCase) =>
{
var userIdString = user.FindFirst("ID")?.Value;
var idUsuario = Guid.Parse(userIdString!);
var response = useCase.Ejecutar(request);
return Results.Created($"/api/expedientes/{response.IdExpediente}", response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

//PUT
ExpedientesApi.MapPut("/cambiar-estado", (CambiarEstadoExpedienteRequest request, CambiarEstadoExpedienteUseCase useCase) =>
{
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!


ExpedientesApi.MapPut("/modificar-caratula", (ModificarCaratulaRequest request, ModificarCaratulaUseCase useCase) =>
{
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!


// DELETE
ExpedientesApi.MapDelete("/{expedienteId:guid}", (Guid expedienteId,ClaimsPrincipal user,EliminarExpedienteUseCase useCase) =>
{
var userIdString = user.FindFirst("ID")?.Value;
var idUsuario = Guid.Parse(userIdString!);
var request = new EliminarExpedienteRequest(expedienteId.ToString(), idUsuario.ToString());
var response = useCase.Ejecutar(request);
// HTTP 200: OK (o podría ser 204 No Content)
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

// GET
ExpedientesApi.MapGet("/{expedienteId:guid}/tramites", (Guid expedienteId, ClaimsPrincipal user, ObtenerPorIdUseCase useCase) => // 2. Inyectado por el DI
{
var userIdString = user.FindFirst("ID")?.Value;
var idUsuario = Guid.Parse(userIdString!);
var request = new ObtenerPorIdRequest(expedienteId,idUsuario);
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!


ExpedientesApi.MapGet ("/Expedientes", (ObtenerTodosExpedientesUseCase useCase) =>
{
var request = new ObtenerTodosExpedientesRequest();
var response = useCase.Ejecutar();
return Results.Ok(response);
}).RequireAuthorization();

}
}