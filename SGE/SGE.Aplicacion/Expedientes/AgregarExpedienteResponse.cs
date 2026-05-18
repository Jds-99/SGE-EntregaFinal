using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record AgregarExpedienteResponse(int IdExpediente, string Caratula, DateTime FechaCreacion);