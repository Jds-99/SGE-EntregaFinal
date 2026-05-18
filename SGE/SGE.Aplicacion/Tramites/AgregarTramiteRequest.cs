using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;

public record AgregarTramiteRequest(int ExpedienteId, string Detalle, string Etiqueta, int UsuarioId);