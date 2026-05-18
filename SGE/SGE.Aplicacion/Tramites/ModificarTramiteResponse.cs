using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;
public record ModificarTramiteResponse(int IdTramite, int ExpedienteId, string NuevaEtiqueta);