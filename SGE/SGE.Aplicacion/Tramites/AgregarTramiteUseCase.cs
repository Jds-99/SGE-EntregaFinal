using SGE.Aplicacion;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;

public class AgregarTramiteUseCase(
    ITramiteRepository tramiteRepo, 
    IExpedienteRepository expedienteRepo,
    IAutorizacionService autorizacion)
{
    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.TramiteAlta, request.UsuarioId))
            throw new AutorizacionException("El usuario no tiene permisos para dar de alta trámites.");

        if (string.IsNullOrWhiteSpace(request.Detalle))
            throw new EntidadNoEncontradaException("El detalle del trámite no puede estar vacío.");

        var expediente = expedienteRepo.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"No existe el expediente con ID {request.ExpedienteId}");

        //Parsear la etiqueta string recibida al Enum correspondiente del Dominio
        if (!Enum.TryParse<EtiquetaTramite>(request.Etiqueta, true, out var etiquetaEnum))
            throw new EntidadNoEncontradaException($"La etiqueta '{request.Etiqueta}' no es válida.");

        var nuevoTramite = new Tramite(request.ExpedienteId, request.Detalle, etiquetaEnum);
        tramiteRepo.Agregar(nuevoTramite);
        ActualizacionEstadoExpedienteService.Ejecutar(request.Etiqueta,request.UsuarioId);
        return new AgregarTramiteResponse(nuevoTramite.Id, nuevoTramite.ExpedienteId, nuevoTramite.Etiqueta.ToString());
    }
}