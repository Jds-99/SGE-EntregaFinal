using System.Security.Claims;
using SGE.Aplicacion;
namespace SGE.WebApi.Endpoints;
public static class LoginEndpoint
{
public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
{
app.MapPost("/api/login", (LoginRequest request, LoginUseCase useCase) =>
{
var response = useCase.Ejecutar(request);
return Results.Ok(response);
}).WithTags("Autenticación");
}
}