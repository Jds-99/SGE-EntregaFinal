using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion;

public record ConsultaExpedienteResponse(int IdExpediente, string Caratula, string Estado, List<TramiteDTO> Tramites);