using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;
public record ModificarTramiteRequest(int IdTramite, string NuevoDetalle, string NuevaEtiqueta, int UsuarioId);