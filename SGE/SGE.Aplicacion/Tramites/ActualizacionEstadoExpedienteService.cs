namespace SGE.Aplicacion.Tramites;
using SGE.Aplicacion;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Expedientes;
public class ActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepository _repositorio;
    private readonly ITramiteRepository _tramiteRepo; 
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    public ActualizacionEstadoExpedienteService(IExpedienteRepository repositorio, ITramiteRepository repositorioTramite, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorio = repositorio;
        _tramiteRepo = repositorioTramite;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public bool Ejecutar(Expediente expediente, Guid IdUsuario)
    {
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente {expediente}");
        }
        
        // 1. Buscamos todos los trámites que pertenezcan a este expediente en el archivo TXT
        var tramitesDelExpediente = _tramiteRepo.ObtenerTodos()
            .Where(t => t.IdExpediente == expediente.Id).ToList();
        // 2. Determinamos cuál es el "último" basándonos en la FechaCreacion más reciente
        var ultimoTramite = tramitesDelExpediente
            .OrderByDescending(t => t.FechaCreacion)
            .FirstOrDefault();
        // 3. Extraemos la etiqueta. Si no tiene trámites (porque se eliminaron), queda en null
        EtiquetaTramite? ultimaEtiqueta = ultimoTramite?.Etiqueta;
        // 4. Le pedimos a la entidad rica que evalúe y aplique la regla de negocio
        bool huboCambio = expediente.ActualizarEstadoAutomatico(ultimaEtiqueta, IdUsuario);
        // 5. Solo si mutó el estado, guardamos el expediente modificado en el archivo, y persistimos en la base de datos
        if (huboCambio)
        {
            _repositorio.Modificar(expediente);
            _unidadDeTrabajo.Guardar();
        }
        
        return huboCambio;
    }
}

