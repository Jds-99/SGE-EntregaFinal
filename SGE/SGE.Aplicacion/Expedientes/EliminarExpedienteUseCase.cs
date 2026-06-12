namespace SGE.Aplicacion.Expedientes;
using System;

public class EliminarExpedienteUseCase(IExpedienteRepository expedienteRepo, IAutorizacionService autorizacion)
{
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        // 1. Validación de Seguridad (Capa de Aplicación)
        // Usamos el ID del usuario directamete si tu servicio acepta los tipos correspondientes
        if (!autorizacion.PoseeElPermiso(Guid.Parse(request.UsuarioId), Permiso.ExpedienteBaja.ToString()))
        {
            throw new AutorizacionException("No tenés permisos para eliminar expedientes.");
        }

        // 2. Validación de Existencia 
        // Buscamos el expediente una sola vez de forma limpia.
        var expediente = expedienteRepo.ObtenerPorId(Guid.Parse(request.IdExpediente))
            ?? throw new EntidadNoEncontradaException($"El expediente con ID {request.IdExpediente} no existe.");

        // 3. Ejecución de la Acción en la Infraestructura
        expedienteRepo.Eliminar(Guid.Parse(request.IdExpediente));

        // 4. Retorno del DTO Response con el formato posicional de tu Record
        return new EliminarExpedienteResponse(Exito: true, IdExpedienteEliminado: request.IdExpediente);
    }
}