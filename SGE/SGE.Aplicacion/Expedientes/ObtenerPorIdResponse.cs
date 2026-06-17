using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion;

public record ObtenerPorIdResponse(Guid IdExpediente, string Caratula, string Estado, List<TramiteDTO> Tramites);