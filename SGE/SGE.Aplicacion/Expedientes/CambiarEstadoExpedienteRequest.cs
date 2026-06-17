using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public record CambiarEstadoExpedienteRequest(Guid IdUsuario, Guid ExpedienteId, EstadoExpediente NuevoEstado);
