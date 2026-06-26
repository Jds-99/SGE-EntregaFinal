using System.Security.Claims;
using Microsoft.AspNetCore.Mvc; // 👈 Acordate de sumar este using para los atributos
using SGE.Aplicacion;

namespace SGE.WebApi.Endpoints;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        // 🌟 Agregamos [FromBody] y [FromServices] para destrabar la inferencia de parámetros
        app.MapPost("/api/login", (
            [FromBody] LoginRequest request, 
            [FromServices] LoginUseCase useCase
        ) =>
        {
            var response = useCase.Ejecutar(request);
            return Results.Ok(response);
        }).WithTags("Autenticación");
    }
}