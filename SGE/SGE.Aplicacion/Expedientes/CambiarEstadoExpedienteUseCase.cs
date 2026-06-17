using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public class CambiarEstadoExpedienteUseCase(IExpedienteRepository expedienteRepository, IAutorizacionService autorizacionService)
{
    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        // 2. Validar permisos usando el parámetro directo del constructor primario
        if (!autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion.ToString()))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar el expediente.");
        }

        // 3. Buscar la entidad usando el repositorio del constructor primario
        var expediente = expedienteRepository.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException("No se encontró el expediente con el ID provisto.");

        // 4. Ejecutar la lógica en tu Dominio Rico (Directiva C)
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        // 5. Persistir en la infraestructura de texto plano
        expedienteRepository.Modificar(expediente);
        
        return new CambiarEstadoExpedienteResponse(expediente.Estado);
    }
}