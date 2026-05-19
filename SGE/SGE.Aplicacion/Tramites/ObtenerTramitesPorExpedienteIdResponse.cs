using System.Collections.Generic;

namespace SGE.Aplicacion.Tramites;

public record ObtenerTramitesPorExpedienteIdResponse(List<TramiteDTO> Tramites);