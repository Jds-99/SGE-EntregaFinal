using System.Security.Claims;
using SGE.Aplicacion.Tramites;
namespace SGE.WebApi;
public static class TramitesEndpoints
{
public static void MapTramitesEndpoints(this IEndpointRouteBuilder app)
{
var tramitesApi = app.MapGroup("/api/tramites").WithTags("Gestión de Trámites");

//POST
tramitesApi.MapPost("/", (AgregarTramiteRequest request, ClaimsPrincipal user, AgregarTramiteUseCase useCase) =>
{
var response = useCase.Ejecutar(request);
return Results.Created($"/api/tramites/{response.Id}", response);// HTTP 201: Creado exitosamente
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

//DELETE
tramitesApi.MapDelete("/{tramiteId:guid}", (Guid TramiteId,ClaimsPrincipal user,EliminarTramiteUseCase useCase) =>
{
var userIdString = user.FindFirst("ID")?.Value;
var idUsuario = Guid.Parse(userIdString!);
var request = new EliminarTramiteRequest(TramiteId, idUsuario);
var response = useCase.Ejecutar(request);
// HTTP 200: OK (o podría ser 204 No Content)
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

//PUT
tramitesApi.MapPut("/modificar-tramite", (ModificarTramiteRequest request, ModificarTramiteUseCase useCase) =>
{
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

//GET
tramitesApi.MapGet("/{tramiteId:guid}/Tramite", (Guid tramiteId, ClaimsPrincipal user, ObtenerTramitePorIdUseCase useCase) =>
{
var userIdString = user.FindFirst("ID")?.Value;
var idUsuario = Guid.Parse(userIdString!);
var request = new ObtenerTramitePorIdRequest(tramiteId,idUsuario);
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!


tramitesApi.MapGet ("/Tramites", (ObtenerTramitesPorExpedienteIdUseCase useCase, Guid expedienteId) =>
{
var request = new ObtenerTramitesPorExpedienteIdRequest(expedienteId);
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization();

}
}