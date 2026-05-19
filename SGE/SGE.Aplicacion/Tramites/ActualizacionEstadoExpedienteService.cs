using SGE.Aplicacion; 
using SGE.Dominio.Expedientes;   

namespace SGE.Aplicacion.Expedientes;

public class ActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepositorio _repositorio;

    public ActualizacionEstadoExpedienteService(IExpedienteRepositorio repositorio)
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
<<<<<<< HEAD
}
=======
}
>>>>>>> 908c6e86bbfadb1d1fdf97c8601d47dfc7b41981
