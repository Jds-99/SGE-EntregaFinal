using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record EliminarTramiteResponse(bool Exito, int IdTramiteEliminado);