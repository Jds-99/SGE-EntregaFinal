using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public class CambiarEstadoExpedienteUseCase
{
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        var estadoNuevo= request.estado;

        return new CambiarEstadoExpedienteResponse(estadoNuevo);
    }
}
