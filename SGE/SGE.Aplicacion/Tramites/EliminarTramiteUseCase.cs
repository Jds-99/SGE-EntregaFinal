using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;

public class EliminarTramiteUseCase(ITramiteRepository tramiteRepo, ActualizacionEstadoExpedienteService estadoService, 
    IAutorizacionService autorizacion)
{
    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.TramiteBaja, request.UsuarioId))
            throw new AutorizacionException("El usuario no tiene permisos para eliminar trámites.");

        var tramite = tramiteRepo.ObtenerPorId(request.IdTramite)
            ?? throw new EntidadNoEncontradaException($"El trámite con ID {request.IdTramite} no existe.");

        // Guardamos el ID del expediente asociado al trámite ANTES de eliminarlo, porque luego lo vamos a necesitar para actualizar su estado
        int expedienteIdAsociado = tramite.ExpedienteId;

        tramiteRepo.Eliminar(request.IdTramite);
        // Recalcular el estado del expediente, ya que al borrarse el trámite el "último" cambió
        estadoService.ActualizarEstado(expedienteIdAsociado);
        return new EliminarTramiteResponse(Exito: true, IdTramiteEliminado: request.IdTramite);
    }
}