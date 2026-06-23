namespace SGE.Aplicacion.Expedientes;
using System;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Usuarios;
public class EliminarExpedienteUseCase(IExpedienteRepository expedienteRepo, IAutorizacionService autorizacion, ITramiteRepository tramiteRepo)
{
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        // 1. Validación de Seguridad (Capa de Aplicación)
        // Usamos el ID del usuario directamete si tu servicio acepta los tipos correspondientes
        if (!autorizacion.PoseeElPermiso(Guid.Parse(request.UsuarioId), Permiso.ExpedienteBaja.ToString()))
        {
            throw new AutorizacionException("No tenés permisos para eliminar expedientes.");
        }

        
        // 2. Validación de Existencia 
        // Buscamos el expediente una sola vez de forma limpia.
        var expediente = expedienteRepo.ObtenerPorId(Guid.Parse(request.IdExpediente))
            ?? throw new EntidadNoEncontradaException($"El expediente con ID {request.IdExpediente} no existe.");
        //Si existe el expediente, busco todos los tramites con el id Del expediente
        IEnumerable<Tramite> lista= tramiteRepo.ObtenerPorExpedienteId(Guid.Parse(request.IdExpediente)); 
        
        // 3. Elimino todos los tramites 
        foreach(Tramite t in lista)
        {
            tramiteRepo.Eliminar(t.Id);
        }
        // 4 Elimino el expediente
        expedienteRepo.Eliminar(Guid.Parse(request.IdExpediente));

        // 5. Retorno del DTO Response con el formato posicional de tu Record
        return new EliminarExpedienteResponse(Exito: true, IdExpedienteEliminado: request.IdExpediente);
    }
}

public record EliminarExpedienteRequest(string IdExpediente, string UsuarioId);
public record EliminarExpedienteResponse(bool Exito, string IdExpedienteEliminado);