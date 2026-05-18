using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion;

public record TramiteDTO(int IdTramite, string Detalle, string Etiqueta, DateTime FechaCreacion);

public record ConsultaExpedienteResponse(int IdExpediente, string Caratula, string Estado, List<TramiteDTO> Tramites);