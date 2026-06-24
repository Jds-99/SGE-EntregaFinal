using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SGE.Aplicacion;
using SGE.Dominio;

namespace SGE.WebApi.ManejadorDeExcepciones;

public class ManejadorDeExcepcionesGlobales : IExceptionHandler
{
public async ValueTask<bool> TryHandleAsync(
HttpContext httpContext, Exception exception, CancellationToken
cancellationToken)
{
var problemDetails = new ProblemDetails { Instance = httpContext.Request.Path };
if (exception is DominioException) {
problemDetails.Title = "Error de validación del negocio";
problemDetails.Status = StatusCodes.Status400BadRequest;
problemDetails.Detail = exception.Message;
} else if (exception is EntidadNoEncontradaException) {
problemDetails.Title = "Recurso no encontrado";
problemDetails.Status = StatusCodes.Status404NotFound;
problemDetails.Detail = exception.Message;
} else if (exception is AutorizacionException) {
problemDetails.Title = "Acceso denegado";
problemDetails.Status = StatusCodes.Status403Forbidden;
problemDetails.Detail = exception.Message;
} else {
problemDetails.Title = "Error interno del servidor";
problemDetails.Status = StatusCodes.Status500InternalServerError;
problemDetails.Detail = "Ha ocurrido un error inesperado.";
}
// Escribimos la respuesta final
httpContext.Response.StatusCode = problemDetails.Status.Value;
await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
return true;
}
}