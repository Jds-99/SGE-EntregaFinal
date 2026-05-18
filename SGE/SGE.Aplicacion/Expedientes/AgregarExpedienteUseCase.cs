using SGE.Dominio.Expedientes;
namespace SGE.Aplicacion.Expedientes;
public class AgregarExpedienteUseCase (IExpedienteRepository repositorio){

    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request){
        var caratula = new Caratula(request.Caratula);
        var expediente = new Expediente(caratula, request.IdUsuario);
        repositorio.AgregarExpediente(expediente);
        return new AgregarExpedienteResponse(expediente.Id, expediente.Caratula, expediente.FechaCreacion);
    }

}