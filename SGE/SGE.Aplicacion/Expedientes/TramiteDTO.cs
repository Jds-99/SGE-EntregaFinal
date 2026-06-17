using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion;

public record TramiteDTO(Guid IdTramite, string Detalle, string Etiqueta, DateTime FechaCreacion);