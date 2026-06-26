using System.Security.Claims;
using Microsoft.AspNetCore.Mvc; // 👈 VITAL: Para usar [FromBody], [FromServices] y [FromQuery]
using SGE.Aplicacion.Tramites;

namespace SGE.WebApi;

public static class TramitesEndpoints
{
    public static void MapTramitesEndpoints(this IEndpointRouteBuilder app)
    {
        var tramitesApi = app.MapGroup("/api/tramites").WithTags("Gestión de Trámites");

        // 🌟 1. POST (Arreglado)
        tramitesApi.MapPost("/", (
            [FromBody] AgregarTramiteRequest request, 
            ClaimsPrincipal user, 
            [FromServices] AgregarTramiteUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Created($"/api/tramites/{response.Id}", response);
        }).RequireAuthorization();

        // 🌟 2. DELETE (Arreglado)
        tramitesApi.MapDelete("/{tramiteId:guid}", (
            Guid tramiteId, // 👈 Ojo con las mayúsculas/minúsculas, que coincida con la ruta {tramiteId:guid}
            ClaimsPrincipal user, 
            [FromServices] EliminarTramiteUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            var request = new EliminarTramiteRequest(tramiteId, idUsuario);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 3. PUT Modificar Trámite (Arreglado)
        tramitesApi.MapPut("/modificar-tramite", (
            [FromBody] ModificarTramiteRequest request, 
            [FromServices] ModificarTramiteUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 4. GET Obtener por Id (Arreglado)
        tramitesApi.MapGet("/{tramiteId:guid}/Tramite", (
            Guid tramiteId, 
            ClaimsPrincipal user, 
            [FromServices] ObtenerTramitePorIdUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            var request = new ObtenerTramitePorIdRequest(tramiteId, idUsuario);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 5. GET Listar por Expediente Id (Arreglado especificando que el Guid viene por Query String)
        tramitesApi.MapGet("/Tramites", (
            [FromServices] ObtenerTramitesPorExpedienteIdUseCase useCase, 
            [FromQuery] Guid expedienteId // 👈 Le indicamos explícitamente de dónde sale
        ) =>
        {
            var request = new ObtenerTramitesPorExpedienteIdRequest(expedienteId);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();
    }
}