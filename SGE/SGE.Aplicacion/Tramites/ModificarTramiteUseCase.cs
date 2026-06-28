namespace SGE.Aplicacion.Tramites;
using System;
using System.Linq;
using SGE.Aplicacion;
using SGE.Aplicacion.Tramites; 
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Usuarios;

public class ModificarTramiteUseCase(
    ITramiteRepository tramiteRepo, 
    IExpedienteRepository expedienteRepo, // Inyectamos el repo para buscar el expediente
    ActualizacionEstadoExpedienteService estadoService, 
    IAutorizacionService autorizacion,
    IUnidadDeTrabajo unidadDeTrabajo)
{
    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        // 1. Validación de Seguridad
        if (!autorizacion.PoseeElPermiso(request.UsuarioId, Permiso.TramiteModificacion.ToString()))
            throw new AutorizacionException("El usuario no tiene permisos.");

        // 2. Buscamos el trámite único usando ObtenerTodos()
        var tramite = tramiteRepo.ObtenerTodos().FirstOrDefault(t => t.Id == request.IdTramite)
            ?? throw new EntidadNoEncontradaException($"El trámite con ID {request.IdTramite} no existe.");

        // 3. Parsear la nueva etiqueta string al Enum de Dominio
        if (!Enum.TryParse<EtiquetaTramite>(request.NuevaEtiqueta, true, out var nuevaEtiquetaEnum))
            throw new EntidadNoEncontradaException($"La etiqueta '{request.NuevaEtiqueta}' no es válida.");

        // 4. Intentamos crear el Value Object. Si viene vacío, el Dominio lanza la excepción solo.
        var nuevoContenido = new Contenido(request.NuevoDetalle);

        // 5. Le delegamos la mutación al Dominio cuidando las invariantes
        tramite.Modificar(nuevoContenido, nuevaEtiquetaEnum, request.UsuarioId);

        // 6. Buscamos el expediente asociado para poder actualizarlo
        var expediente = expedienteRepo.ObtenerPorId(tramite.IdExpediente)
            ?? throw new EntidadNoEncontradaException($"No existe el expediente asociado con ID {tramite.IdExpediente}");

        // 7. Persistimos los cambios del trámite modificado
        tramiteRepo.Modificar(tramite);
        unidadDeTrabajo.Guardar();
        // 8. Orquestamos la actualización automática de estado mediante el servicio interno
        estadoService.Ejecutar(expediente, request.UsuarioId);

        return new ModificarTramiteResponse(tramite.Id, tramite.IdExpediente, tramite.Etiqueta.ToString());
    }
}

public record ModificarTramiteRequest(Guid IdTramite, string NuevoDetalle, string NuevaEtiqueta, Guid UsuarioId);
public record ModificarTramiteResponse(Guid IdTramite, Guid ExpedienteId, string NuevaEtiqueta);