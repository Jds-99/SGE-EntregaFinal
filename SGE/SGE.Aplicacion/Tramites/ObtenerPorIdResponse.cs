using System;

namespace SGE.Aplicacion.Tramites;

public record ObtenerTramitePorIdResponse(
    Guid Id, 
    Guid ExpedienteId, 
    string Detalle, 
    string Etiqueta, 
    DateTime FechaCreacion
);