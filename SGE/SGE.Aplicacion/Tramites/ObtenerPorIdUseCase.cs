using System;
using SGE.Aplicacion;
using SGE.Dominio;

namespace SGE.Aplicacion.Tramites;

public class ObtenerTramitePorIdUseCase(ITramiteRepository tramiteRepo, IAutorizacionService autorizacion)
{
    public ObtenerTramitePorIdResponse Ejecutar(ObtenerTramitePorIdRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.Lectura, request.UsuarioId)) 
            throw new AutorizacionException("El usuario no tiene permisos de lectura.");

        var tramite = tramiteRepo.ObtenerPorId(request.TramiteId);
        if (tramite == null)
        {
            throw new EntidadNoEncontradaException($"El trámite con ID {request.TramiteId} no existe.");
        }

        return new ObtenerTramitePorIdResponse(
            tramite.Id,
            tramite.ExpedienteId,
            tramite.Detalle,
            tramite.Etiqueta.ToString(),
            tramite.FechaCreacion
        );
    }
}