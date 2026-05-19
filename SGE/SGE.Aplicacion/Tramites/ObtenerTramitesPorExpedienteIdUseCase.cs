using System;
using System.Linq;
using SGE.Aplicacion.Interfaces;
using SGE.Dominio;

namespace SGE.Aplicacion.Tramites;

public class ObtenerTramitesPorExpedienteIdUseCase(
    ITramiteRepository tramiteRepo, 
    IExpedienteRepository expedienteRepo, 
    IAutorizacionService autorizacion)
{
    public ObtenerTramitesPorExpedienteIdResponse Ejecutar(ObtenerTramitesPorExpedienteIdRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.Lectura, request.UsuarioId)) 
            throw new AutorizacionException("El usuario no tiene permisos de lectura.");
        var expediente = expedienteRepo.ObtenerPorId(request.ExpedienteId)
            ?? throw new ValidacionException($"El expediente con ID {request.ExpedienteId} no existe.");

        var tramitesDelExpediente = tramiteRepo.ObtenerTodos()
            .Where(t => t.ExpedienteId == request.ExpedienteId)
            .ToList();

        var listaDTO = tramitesDelExpediente.Select(t => new TramiteDTO(
            t.Id,
            t.Detalle,
            t.Etiqueta.ToString(),
            t.FechaCreacion
        )).ToList();

        return new ObtenerTramitesPorExpedienteIdResponse(listaDTO);
    }
}