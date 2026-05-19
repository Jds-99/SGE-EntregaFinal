using System;

namespace SGE.Aplicacion.Tramites;

public record TramiteDTO(Guid Id, string Detalle, string Etiqueta, DateTime FechaCreacion);