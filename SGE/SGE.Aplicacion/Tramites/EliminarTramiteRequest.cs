using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;
public record EliminarTramiteRequest(int IdTramite, int UsuarioId);