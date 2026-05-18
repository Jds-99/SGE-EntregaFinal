using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;
public record EliminarTramiteResponse(bool Exito, int IdTramiteEliminado);