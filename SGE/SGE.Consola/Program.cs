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

        // inyectar casos de usos a sus dto de Expediente
        var GuidUsuario=Guid.NewGuid();
        var altaExpedienteUseCase= new AgregarExpedienteUseCase(expedienteRepository,autorizacion);
        var bajaExpedienteUseCase = new EliminarExpedienteUseCase(expedienteRepository, autorizacion);
        var modificarCaratulaExpedienteUseCase= new ModificarCaratulaUseCase(expedienteRepository,autorizacion);
        var cambiarEstadoManualUseCase= new CambiarEstadoExpedienteUseCase(expedienteRepository,autorizacion);
        var obtenerPorIdUseCase=new ObtenerPorIdUseCase(expedienteRepository,autorizacion);
        // inyectar casos de uso a dtos de tramite

        var agregarTramiteUseCase= new AgregarTramiteUseCase(tramiteRepository,autorizacion);
        var listarPorExpedienteIdCaseUse= new ListarPorExpedienteIdCaseUse(tramiteRepository,autorizacion);

        // id identificador del usuario fijo para la simulacion

        Guid idUsuarioOperador=Guid.NewGuid();

        Console.WriteLine("----------------- Sistema de Gestion De Expedientes (SGE) -----------------");
        // flujo feliz(alta de expediente,tramite y cambios de estado)
        try{
        Console.WriteLine("flujo feliz");
        var altaExpedienteReq= new AgregarExpedienteRequest("Prueba 1", idUsuarioOperador);
        AgregarExpedienteResponse altaExpedienteRes=altaExpedienteUseCase.Ejecutar(altaExpedienteReq);
        Guid expedienteId=altaExpedienteRes.Id;
        Console.WriteLine($"expediente creado. ID{expedienteId}");

        // agregar Tramite
        var tramite1Req= new AgregarTramiteRequest(expedienteId,EtiquetaTramite.PaseAEstudio,idUsuarioOperador);
        agregarTramiteUseCase.Ejecutar(tramite1Req);

        // verificar listado de expedientes
        Console.WriteLine("listado de expedientes: ");
        var listaExpedentes= listarExpedienteUseCase.Ejecutar();
        foreach(var e in listaExpedentes)
            {
                Console.WriteLine($"ID:{e.Id}, caratula: {e.Caratula},Estado : {e.Estado}, ultmo cambio: {e.fechaModificacion}")
            }
        


        // agregar segundo tramite
        var tramite2Req= new AgregarTramiteRequest(expedienteId,EtiquetaTramite.Resolucion,idUsuarioOperador);
        agregarTramiteUseCase.Ejecutar(tramite2Req);
        // verificar listado
        //metodo de listar tramites
        // cambio manual
        var cambioEstado = new CambiarEstadoExpedienteRequest(EstadoExpediente.EnNotificacion);
        CambiarEstadoExpedienteUseCase.Ejecutar(cambioEstado);
        //listar
        // metodo de listar tramites
        }
        catch (Exception ex)
        {
            Console.WriteLine("error en camino feliz; Corregir");
        }

        Console.WriteLine("camino de error");

        var reqInvalido= new AgregarExpedienteRequest("",idUsuarioOperador);// viola la regla del value object
        altaExpedienteUseCase.Ejecutar(reqInvalido);

    }
}