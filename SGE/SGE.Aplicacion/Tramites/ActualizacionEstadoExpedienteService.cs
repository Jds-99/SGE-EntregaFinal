namespace SGE.Aplicacion.Expedientes;
using SGE.Aplicacion; 

public class ActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepository _repositorio;

    public ActualizacionEstadoExpedienteService(IExpedienteRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public ActualizacionEstadoExpedienteResponse Ejecutar(ActualizacionEstadoExpedienteRequest request)
    {
        var expediente = _repositorio.ObtenerPorId(request.ExpedienteId);
        
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente {request.ExpedienteId}");
        }

        expediente.CambiarEstado(request.NuevoEstado);
        _repositorio.Actualizar(expediente);
        return new ActualizacionEstadoExpedienteResponse("Estado actualizado con éxito");
    }
}

