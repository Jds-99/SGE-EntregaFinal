using System;

namespace SGE.Aplicacion.Tramites;

public record ObtenerTramitesPorExpedienteIdRequest(Guid ExpedienteId, Guid UsuarioId);