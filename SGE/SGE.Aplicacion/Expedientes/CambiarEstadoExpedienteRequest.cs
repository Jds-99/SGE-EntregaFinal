using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public record CambiarEstadoExpedienteRequest(EstadoExpediente estado)
{

}
