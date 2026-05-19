using System;

namespace SGE.Aplicacion.Tramites;

public record ObtenerTramitePorIdRequest(Guid TramiteId, Guid UsuarioId);