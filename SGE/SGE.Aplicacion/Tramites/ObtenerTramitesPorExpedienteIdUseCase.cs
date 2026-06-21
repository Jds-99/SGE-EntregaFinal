namespace SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Expedientes;

public class ObtenerTramitesPorExpedienteIdUseCase(
    ITramiteRepository tramiteRepo, 
    IExpedienteRepository expedienteRepo)
{
    public ObtenerTramitesPorExpedienteIdResponse Ejecutar(ObtenerTramitesPorExpedienteIdRequest request)
    {
        // 1. Validación de Existencia del Expediente contenedor
        var expediente = expedienteRepo.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"El expediente con ID {request.ExpedienteId} no existe.");

        // 2. Usamos el método optimizado del repositorio que arreglamos hoy
        IEnumerable<Tramite> tramitesDelExpediente = tramiteRepo.ObtenerPorExpedienteId(request.ExpedienteId);

        // 3. Mapeamos la lista de entidades a una lista de DTOs limpios
        var listaDTO = tramitesDelExpediente.Select(t => new TramiteDTO(
            Id: t.Id,
            Detalle: t.contenido.ToString(), // Extraemos el string del Value Object
            Etiqueta: t.Etiqueta.ToString(),
            FechaCreacion: t.FechaCreacion
        )).ToList();

        return new ObtenerTramitesPorExpedienteIdResponse(listaDTO);
    }
}

// DTOs posicionales para la entrada y salida de datos
public record ObtenerTramitesPorExpedienteIdRequest(Guid ExpedienteId, Guid UsuarioId);
public record ObtenerTramitesPorExpedienteIdResponse(List<TramiteDTO> Tramites);

public record TramiteDTO(
    Guid Id, 
    string Detalle, 
    string Etiqueta, 
    DateTime FechaCreacion
);