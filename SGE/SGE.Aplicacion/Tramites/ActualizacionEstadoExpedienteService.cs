namespace SGE.Aplicacion.Expedientes;
using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

public class ActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepository _repositorio;

    public ActualizacionEstadoExpedienteService(IExpedienteRepository repositorio)
    {
        _repositorio = repositorio;
    }

    public bool Ejecutar(Expediente expediente)
    {
        bool ok= false;
        if (expediente == null)
        {
            return ok;
            throw new EntidadNoEncontradaException($"No se encontró el expediente {expediente}");
        }

        else (expediente.ActualizarEstadoAutomatico(expediente.Id,expediente.Caratula))
        {
            ok= true;
            _repositorio.Modificar(expediente);
            
        }
        
        return ok;
    }
}

