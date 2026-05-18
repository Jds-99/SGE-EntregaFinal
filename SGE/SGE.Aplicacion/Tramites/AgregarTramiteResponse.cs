using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Tramites;

public record AgregarTramiteResponse(int Id, int ExpedienteId, string Etiqueta);