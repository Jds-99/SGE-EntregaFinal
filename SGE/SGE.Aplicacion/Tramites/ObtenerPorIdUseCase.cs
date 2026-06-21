using System;
using System.Linq;
using SGE.Aplicacion;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public class ObtenerTramitePorIdUseCase(ITramiteRepository tramiteRepo)
{
    public ObtenerTramitePorIdResponse Ejecutar(ObtenerTramitePorIdRequest request)
    {
        // 1. Buscamos el trámite único en la lista que lee el archivo TXT
        var tramite = tramiteRepo.ObtenerTodos().FirstOrDefault(t => t.Id == request.TramiteId)
            ?? throw new EntidadNoEncontradaException($"El trámite con ID {request.TramiteId} no existe.");

        // 2. Retornamos el DTO mapeando correctamente las propiedades del Dominio
        return new ObtenerTramitePorIdResponse(
            Id: tramite.Id,
            ExpedienteId: tramite.IdExpediente, // Propiedad real en tu entidad
            Detalle: tramite.contenido.ToString(), // Extraemos el texto del Value Object
            Etiqueta: tramite.Etiqueta.ToString(),
            FechaCreacion: tramite.FechaCreacion
        );
    }
}

// DTOs auxiliares para la entrada y salida de datos
public record ObtenerTramitePorIdRequest(Guid TramiteId, Guid UsuarioId);
public record ObtenerTramitePorIdResponse(Guid Id, Guid ExpedienteId, string Detalle, string Etiqueta, DateTime FechaCreacion);

