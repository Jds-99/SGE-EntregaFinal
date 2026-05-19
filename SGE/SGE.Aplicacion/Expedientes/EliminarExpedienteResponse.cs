using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record EliminarExpedienteResponse(bool Exito, string IdExpedienteEliminado);