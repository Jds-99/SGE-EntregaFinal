using SGE.Dominio.Expedientes;

public class ActualizacionEstadoExpedienteService () {

    public ActualizacionEstadoExpedienteResponse ejecutar(ActualizacionEstadoExpedienteRequest request)
    {
        var estado= request;
        return  new ActualizacionEstadoExpedienteResponse(estado);
    }

}
