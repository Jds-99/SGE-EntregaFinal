namespace SGE.Aplicacion.Expedientes;
using SGE.Aplicacion;

public class EliminarExpedienteUseCase(IExpedienteRepository expedienteRepo, IAutorizacionService autorizacion)
{
    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        if (!autorizacion.PoseeElPermiso(Guid.Parse(Permiso.ExpedienteBaja.ToString()),request.UsuarioId.ToString()))
            throw new AutorizacionException("No tenés permisos para eliminar expedientes.");

<<<<<<< HEAD
        var expediente = expedienteRepo.ObtenerPorId(request.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"El expediente con ID {request.IdExpediente} no existe.");
=======
<<<<<<< HEAD
        var expediente = expedienteRepo.ObtenerPorId(request.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"El expediente con ID {request.IdExpediente} no existe.");
=======
        var expediente = expedienteRepo.ObtenerPorId(Guid.Parse(request.IdExpediente))
            ?? throw new AutorizacionException($"El expediente con ID {request.IdExpediente} no existe.");
>>>>>>> 49aa4144266a39216aada933adb0b6dcaa674457
>>>>>>> 908c6e86bbfadb1d1fdf97c8601d47dfc7b41981

        expedienteRepo.Eliminar(Guid.Parse(request.IdExpediente));
        return new EliminarExpedienteResponse(Exito: true, IdExpedienteEliminado: request.IdExpediente);
    }
}