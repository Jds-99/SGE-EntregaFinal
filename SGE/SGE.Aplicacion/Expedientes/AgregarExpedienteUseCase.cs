namespace SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;
public class AgregarExpedienteUseCase
{
    private readonly IExpedienteRepository repositorio;
    private readonly IAutorizacionService ServicioAutorizacion;

    public AgregarExpedienteUseCase(IExpedienteRepository repositorio, IAutorizacionService servicioAutorizacion)
    {
        this.repositorio = repositorio;
        this.ServicioAutorizacion = servicioAutorizacion;
    }
    
    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        if (!this.ServicioAutorizacion.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta.ToString()))
            throw new AutorizacionException("No autorizado.");

        var caratula = new CaratulaExpendiente(request.CaratulaTxt); 
        var expediente = new Expediente(caratula, request.IdUsuario);

        repositorio.Agregar(expediente);

        return new AgregarExpedienteResponse(expediente.Id, expediente.Caratula.Valor , DateTime.Now);
    }
}

public record AgregarExpedienteRequest(string CaratulaTxt, Guid IdUsuario);
public record AgregarExpedienteResponse(Guid IdExpediente, string Caratula, DateTime FechaCreacion);