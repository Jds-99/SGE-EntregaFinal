using System.Security.Claims;
using Microsoft.AspNetCore.Mvc; // 👈 VITAL: Para usar [FromBody] y [FromServices]
using SGE.Aplicacion.Expedientes;

namespace SGE.WebApi.Endpoints;

public static class ExpedientesEndpoints
{
    public static void MapExpedientesEndpoints(this IEndpointRouteBuilder app)
    {
        var ExpedientesApi = app.MapGroup("/api/expedientes").WithTags("Gestión de Expedientes");

        // 🌟 1. POST (Arreglado con atributos explícitos)
        ExpedientesApi.MapPost("/", (
            [FromBody] AgregarExpedienteRequest request, 
            ClaimsPrincipal user, 
            [FromServices] AgregarExpedienteUseCase useCase
        ) =>
        {
            // Extraemos el ID real del token de forma segura
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            
            // Opcional: Si tu caso de uso necesita el idUsuario del token, 
            // creás un nuevo request con ese ID para que no viaje hardcodeado en el JSON:
            var requestDefinitivo = new AgregarExpedienteRequest(request.CaratulaTxt, idUsuario);

            var response = useCase.Ejecutar(requestDefinitivo);
            return Results.Created($"/api/expedientes/{response.IdExpediente}", response);
        }).RequireAuthorization();

        // 🌟 2. PUT Cambiar Estado
        ExpedientesApi.MapPut("/cambiar-estado", (
            [FromBody] CambiarEstadoExpedienteRequest request, 
            [FromServices] CambiarEstadoExpedienteUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 3. PUT Modificar Carátula
        ExpedientesApi.MapPut("/modificar-caratula", (
            [FromBody] ModificarCaratulaRequest request, 
            [FromServices] ModificarCaratulaUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 4. DELETE
        ExpedientesApi.MapDelete("/{expedienteId:guid}", (
            Guid expedienteId, 
            ClaimsPrincipal user, 
            [FromServices] EliminarExpedienteUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            
            var request = new EliminarExpedienteRequest(expedienteId.ToString(), idUsuario.ToString());
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 5. GET Trámites por Expediente
        ExpedientesApi.MapGet("/{expedienteId:guid}/tramites", (
            Guid expedienteId, 
            ClaimsPrincipal user, 
            [FromServices] ObtenerPorIdUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            
            var request = new ObtenerPorIdRequest(expedienteId, idUsuario);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 6. GET Listar Todos
        ExpedientesApi.MapGet("/Expedientes", (
            [FromServices] ObtenerTodosExpedientesUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(); // Pasalo sin parámetros si tu método no los requiere
            return Results.Ok(response);
        }).RequireAuthorization();
    }
}