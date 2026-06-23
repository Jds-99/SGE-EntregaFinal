namespace SGE.Aplicacion.Expedientes;
using System;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Usuarios;
// Usamos el constructor primario de C# 12 limpiando las cabeceras duplicadas
public class ModificarCaratulaUseCase(IExpedienteRepository expedienteRepository, IAutorizacionService autorizacionService)
{
    public ModificarCaratulaResponse Ejecutar(ModificarCaratulaRequest request)
    {
        // 1. Validar permisos (Usamos el parámetro del constructor primario)
        if (!autorizacionService.PoseeElPermiso(request.UsuarioId, Permiso.ExpedienteModificacion.ToString()))
        {
            throw new AutorizacionException($"El usuario {request.UsuarioId} no tiene permisos para modificar carátulas.");
        }

        // 2. Buscar la entidad en el archivo de texto
        var expediente = expedienteRepository.ObtenerPorId(request.ExpedienteId) 
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");

        // 3. Crear el Value Object e invocar el método del dominio rico
        var nuevaCaratula = new CaratulaExpendiente(request.NuevaCaratulaTexto);
        expediente.ModificarCaratula(nuevaCaratula, request.UsuarioId);

        // 4. Persistir los cambios usando tu método de infraestructura
        expedienteRepository.Modificar(expediente); // Asegurate si tu interfaz usa Modificar o Actualizar

        // 5. Retornar la respuesta estructurada
        return new ModificarCaratulaResponse(expediente.Id, expediente.Caratula.Valor);
    }
}
public record ModificarCaratulaResponse(Guid Id, string CaratulaTexto);
public record ModificarCaratulaRequest(string NuevaCaratulaTexto, Guid UsuarioId, Guid ExpedienteId);