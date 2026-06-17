using System;
using System.Linq; // Asegurate de tener este using para que funcione el .Select()
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ObtenerPorIdUseCase(IExpedienteRepository repositorio, IAutorizacionService autorizacion)
{
    public ObtenerPorIdResponse Ejecutar(ObtenerPorIdRequest request)
    {
        
        var expediente = repositorio.ObtenerPorId(request.IdExpediente);
        if (expediente == null)
        {
        throw new EntidadNoEncontradaException($"El expediente con ID {request.IdExpediente} no existe.");
        }

        // Mapear la lista de entidades de dominio 'Tramite' a 'TramiteDTO'
        var listaTramitesDTO = expediente.Tramites.Select(t => new TramiteDTO(
            t.Id, 
            t.contenido.ToString(), 
            t.Etiqueta.ToString(), 
            t.FechaCreacion
        )).ToList();

        return new ObtenerPorIdResponse(
            expediente.Id,
            expediente.Caratula.Valor, // 2. OJO: Pasamos el .Valor (string) si el Response espera un string plano
            expediente.Estado.ToString(),
            listaTramitesDTO
        );
    }
}