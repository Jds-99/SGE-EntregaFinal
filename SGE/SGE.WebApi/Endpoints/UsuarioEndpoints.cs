using System.Security.Claims;
using Microsoft.AspNetCore.Mvc; // 👈 Clave para usar los atributos explícitos
using SGE.Aplicacion;

namespace SGE.WebApi.Endpoints;

public static class UsuariosEndpoints
{
    public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var usuariosApi = app.MapGroup("/api/usuarios").WithTags("Gestión de Usuarios"); 

        // 🌟 1. DELETE
        usuariosApi.MapDelete("/EliminarUsuario", (
            [FromServices] EliminarUsuarioUseCase useCase
        ) =>
        {
            var request = new EliminarUsuarioRequest();
            useCase.Ejecutar(request);
            return Results.Ok();
        }).RequireAuthorization();
           
        // 🌟 2. GET Listar Usuarios
        usuariosApi.MapGet("/Usuarios", (
            [FromQuery] Guid userId, // 👈 Le indicamos explícitamente que viaja por Query String (?userId=...)
            [FromServices] ListarUsuariosUseCase useCase
        ) =>
        {
            var request = new ListarUsuariosRequest(userId);
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 3. PUT Modificar Mis Datos
        usuariosApi.MapPut("/modificar-usuario", (
            [FromBody] ModificarMisDatosRequest request, 
            [FromServices] ModificarMisDatosUseCase useCase
        ) =>
        {
            useCase.Ejecutar(request);
            return Results.Ok();
        }).RequireAuthorization();

        // 🌟 4. PUT Cambiar Estado / Modificar Permisos
        usuariosApi.MapPut("/cambiar-estado", (
            [FromBody] ModificarPermisosRequest request, 
            [FromServices] ModificarPermisosUsuarioUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).RequireAuthorization();

        // 🌟 5. POST Registrar Usuario
        usuariosApi.MapPost("/", (
            [FromBody] RegistrarUsuarioRequest request, 
            ClaimsPrincipal user, 
            [FromServices] RegistrarUsuarioUseCase useCase
        ) =>
        {
            var userIdString = user.FindFirst("ID")?.Value;
            var idUsuario = Guid.Parse(userIdString!);
            
            var response = useCase.Ejecutar(request);
            return Results.Created($"/api/usuarios/{response.Id}", response);
        }).RequireAuthorization();
    }
}