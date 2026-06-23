namespace SGE.Aplicacion.Tramites;
using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Usuarios;
public class EliminarTramiteUseCase(ITramiteRepository tramiteRepo, IExpedienteRepository expedienteRepo, ActualizacionEstadoExpedienteService estadoService, 
    IAutorizacionService autorizacion)
{
    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(request.UsuarioId,Permiso.TramiteBaja.ToString()))
            throw new AutorizacionException("El usuario no tiene permisos para eliminar trámites.");

        // 1. Buscamos el trámite único filtrando la lista completa por su ID propio
        var tramite = tramiteRepo.ObtenerTodos().FirstOrDefault(t => t.Id == request.IdTramite)
        ?? throw new EntidadNoEncontradaException($"El trámite con ID {request.IdTramite} no existe.");

        Guid expedienteIdAsociado = tramite.IdExpediente;
        //Buscamos el expediente completo
        var expediente = expedienteRepo.ObtenerPorId(expedienteIdAsociado)
            ?? throw new EntidadNoEncontradaException($"No existe el expediente asociado con ID {expedienteIdAsociado}");
        
        tramiteRepo.Eliminar(request.IdTramite);
        // Recalcular el estado del expediente, ya que al borrarse el trámite el "último" cambió
        estadoService.Ejecutar(expediente, expedienteIdAsociado);
        return new EliminarTramiteResponse(Exito: true, IdTramiteEliminado: request.IdTramite);
    }
}
public record EliminarTramiteRequest(Guid IdTramite, Guid UsuarioId);
public record EliminarTramiteResponse(bool Exito, Guid IdTramiteEliminado);