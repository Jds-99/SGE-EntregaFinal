namespace SGE.Aplicacion.Tramites;
using System;
using SGE.Aplicacion;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

public class AgregarTramiteUseCase(
    ITramiteRepository tramiteRepo, 
    IExpedienteRepository expedienteRepo,
    IAutorizacionService autorizacion,
    ActualizacionEstadoExpedienteService estadoService) // Inyectamos el servicio interno
{
    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {
        // 1. Validación de Seguridad
        if (!autorizacion.PoseeElPermiso(request.UsuarioId, Permiso.TramiteAlta.ToString()))
            throw new AutorizacionException("El usuario no tiene permisos para dar de alta trámites.");

        // 2. Validación de Existencia del Expediente 
        var expediente = expedienteRepo.ObtenerPorId(request.ExpedienteId)
            ?? throw new EntidadNoEncontradaException($"No existe el expediente con ID {request.ExpedienteId}");

        // 3. Parsear la etiqueta string al Enum
        if (!Enum.TryParse<EtiquetaTramite>(request.Etiqueta, true, out var etiquetaEnum))
            throw new EntidadNoEncontradaException($"La etiqueta '{request.Etiqueta}' no es válida.");

        // 4. Intentamos crear el Contenido (Si request.Detalle está vacío, el Dominio va a tirar la excepción acá)
        var contenidoTramite = new Contenido(request.Detalle);

        // 5. Creamos el trámite respetando su constructor real del Dominio (Sin .Parse)
        var nuevoTramite = new Tramite(request.ExpedienteId, request.UsuarioId, contenidoTramite);
        
        // 6. Guardamos el trámite en su repositorio
        tramiteRepo.Agregar(nuevoTramite);

        // 7. Ejecutamos el servicio interno pasando los parámetros reales
        estadoService.Ejecutar(expediente, request.UsuarioId);

        return new AgregarTramiteResponse(nuevoTramite.Id.ToString(), nuevoTramite.IdExpediente.ToString(), nuevoTramite.Etiqueta.ToString());
    }
}

public record AgregarTramiteRequest(Guid ExpedienteId, string Detalle, string Etiqueta, Guid UsuarioId);
// Asumo que el Response espera strings para acoplarse con la API/Consola externa
public record AgregarTramiteResponse(string Id, string ExpedienteId, string Etiqueta);