using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;

public class ModificarTramiteUseCase(
    ITramiteRepository tramiteRepo, 
    ActualizacionEstadoExpedienteService estadoService, 
    IAutorizacionService autorizacion)
{
    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.TramiteModificacion, request.UsuarioId))
            throw new AutorizacionException("El usuario no tiene permisos.");
        var tramite = tramiteRepo.ObtenerPorId(request.IdTramite)
            ?? throw new ValidacionException($"El trámite con ID {request.IdTramite} no existe.");

        if (string.IsNullOrWhiteSpace(request.NuevoDetalle))
            throw new ValidacionException("El detalle modificado no puede estar vacío.");

        if (!Enum.TryParse<EtiquetaTramite>(request.NuevaEtiqueta, true, out var nuevaEtiquetaEnum))
            throw new ValidacionException($"La etiqueta '{request.NuevaEtiqueta}' no es válida.");

        tramite.Detalle = request.NuevoDetalle;
        tramite.Etiqueta = nuevaEtiquetaEnum;
        tramite.FechaModificacion = DateTime.Now; 
        tramiteRepo.Modificar(tramite);
        estadoService.ActualizarEstado(tramite.ExpedienteId);

        return new ModificarTramiteResponse(tramite.Id, tramite.ExpedienteId, tramite.Etiqueta.ToString());
    }
}