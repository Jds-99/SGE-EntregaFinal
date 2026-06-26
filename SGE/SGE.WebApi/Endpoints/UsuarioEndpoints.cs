using System.Security.Claims;
using SGE.Aplicacion;
namespace SGE.WebApi.Endpoints;
public static class UsuariosEndpoints
{
public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
{
var usuariosApi = app.MapGroup("/api/usuarios").WithTags("Gestión de Usuarios"); 
// DELETE
usuariosApi.MapDelete("/EliminarUsuario", (EliminarUsuarioUseCase useCase) =>
{
var request = new EliminarUsuarioRequest();
useCase.Ejecutar(request);
return Results.Ok();
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!
   
//GET   
usuariosApi.MapGet ("/Usuarios", (Guid userId, ListarUsuariosUseCase useCase) =>
{
var request = new ListarUsuariosRequest(userId);
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization();


//PUT
usuariosApi.MapPut("/modificar-usuario", (ModificarMisDatosRequest request, ModificarMisDatosUseCase useCase) =>
{
useCase.Ejecutar(request);
return Results.Ok();
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!


usuariosApi.MapPut("/cambiar-estado", (ModificarPermisosRequest request, ModificarPermisosUsuarioUseCase useCase) =>
{
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

//PUT
usuariosApi.MapPost("/", (RegistrarUsuarioRequest request, ClaimsPrincipal user, RegistrarUsuarioUseCase useCase) =>
{
var userIdString = user.FindFirst("ID")?.Value;
var idUsuario = Guid.Parse(userIdString!);
var response = useCase.Ejecutar(request);
return Results.Created($"/api/usuarios/{response.Id}", response);
}).RequireAuthorization(); // <- ¡Esta línea bloquea el acceso a usuarios sin token!

}
}