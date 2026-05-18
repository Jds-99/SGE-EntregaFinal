using SGE.Dominio; 
using SGE.Aplicacion; 

namespace SGE.Aplicacion.Expedientes;

public class ObtenerPorIdUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion)
{
    public ObtenerPorIdResponse Ejecutar(ObtenerPorIdRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Permiso.Lectura, request.IdUsuario)) throw new AutorizacionException();
        var expediente = repositorio.ObtenerPorId(request.IdExpediente);
        if (expediente == null)
        {
            throw new Exception($"El expediente con ID {request.IdExpediente} no existe.");
        }

        //Mapear la lista de entidades de dominio 'Tramite' a 'TramiteDTO'
        var listaTramitesDTO = expediente.Tramites.Select(t => new TramiteDTO(
            t.Id, 
            t.Detalle, 
            t.Etiqueta.ToString(), 
            t.FechaCreacion
        )).ToList();

        return new ObtenerPorIdResponse(
            expediente.Id,
            expediente.Caratula,
            expediente.Estado.ToString(),
            listaTramitesDTO
        );
    }
}