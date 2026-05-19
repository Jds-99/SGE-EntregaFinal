using SGE.Dominio;
using SGE.Aplicacion;

public class EliminarExpedienteUseCase(IExpedienteRepository expedienteRepo, IAutorizacionService autorizacion)
{
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.ExpedienteBaja, request.UsuarioId))
            throw new AutorizacionException("No tenés permisos para eliminar expedientes.");

        var expediente = expedienteRepo.ObtenerPorId(request.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"El expediente con ID {request.IdExpediente} no existe.");

        expedienteRepo.Eliminar(request.IdExpediente);
        return new EliminarExpedienteResponse(Exito: true, IdExpedienteEliminado: request.IdExpediente);
    }
}