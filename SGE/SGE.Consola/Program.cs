using SGE.Consola;
using SGE.Aplicacion;
using SGE.Dominio;
using SGE.Infraestructura;

public class Program {
    static void Main(String[] arg)
    {
        // instanciamos repositorios
        var tramiteRepository= new TramiteTxtRepository();
        var expedienteRepository= new ExpeditenteTxtRepository();

        // servicio de aprobacio provisional y cambio automatico de estado

        var autorizacion= new AutorizacionProvisionalService();

        // inyectar casos de usos a sus dto
        var GuidUsuario=Guid.NewGuid();
        var altaExpedienteUseCase= new AgregarExpedienteUseCase(expedienteRepository,GuidUsuario);
        var bajaExpedienteUseCase = new EliminarExpedienteUseCase(expedienteRepository, GuidUsuario);
        var modificarCaratulaExpedienteUseCase= new ModificarCaratulaUseCase(expedienteRepository,GuidUsuario);
    }
}