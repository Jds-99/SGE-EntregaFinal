namespace SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;
public class AgregarExpedienteUseCase
{
    private readonly IExpedienteRepository _repositorio;
    private readonly IAutorizacionService _ServicioAutorizacion;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public AgregarExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService servicioAutorizacion, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _repositorio = repositorio;
        _ServicioAutorizacion = servicioAutorizacion;
        _unidadDeTrabajo = unidadDeTrabajo;
    }
    
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        // 1. Control de autorización
       // if (!_ServicioAutorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta.ToString()))
       //     throw new AutorizacionException("No autorizado.");

        // 2. Creación del objeto de negocio
        var caratula = new CaratulaExpendiente(request.CaratulaTxt); 
        var expediente = new Expediente(caratula, request.IdUsuario);

        // 3. El repositorio local "registra" el expediente en la memoria del contexto de EF Core
        _repositorio.Agregar(expediente);
        
        // 4. Guardamos físicamente los cambios en la base de datos
        _unidadDeTrabajo.Guardar();

        // 4. Retornamos la respuesta
        return new AgregarExpedienteResponse(expediente.Id, expediente.Caratula.Valor , DateTime.Now);
    }
}

public record AgregarExpedienteRequest(string CaratulaTxt, Guid IdUsuario);
public record AgregarExpedienteResponse(Guid IdExpediente, string Caratula, DateTime FechaCreacion);