using SGE.Dominio;
using SGE.Aplicacion;

public class EliminarTramiteUseCase(ITramiteRepository tramiteRepo, ActualizacionEstadoExpedienteService estadoService, 
    IAutorizacionService autorizacion)
{
    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.TramiteBaja, request.UsuarioId))
            throw new AutorizacionException("No tenés permisos para eliminar trámites.");
        var tramite = tramiteRepo.ObtenerPorId(request.IdTramite)
            ?? throw new ValidacionException($"El trámite con ID {request.IdTramite} no existe.");
        int expedienteIdAsociado = tramite.ExpedienteId;
        tramiteRepo.Eliminar(request.IdTramite);

        estadoService.ActualizarEstado(expedienteIdAsociado);

        return new EliminarTramiteResponse(Exito: true, IdTramiteEliminado: request.IdTramite);
    }
}