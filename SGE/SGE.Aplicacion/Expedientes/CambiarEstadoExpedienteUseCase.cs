using System;
using SGE.Dominio.Expedientes;
using SGE.Aplicacion.Autorizacion; // Ajusta según tus namespaces

namespace SGE.Aplicacion;

public class CambiarEstadoExpedienteUseCase(IExpedienteRepository expedienteRepository, IAutorizacionService autorizacionService)
{

    private readonly IExpedienteRepository RepositorioExpediente;
    private readonly IAutorizacionService autorizacionService;

    public CambiarEstadoExpedienteUseCase(IExpedienteRepository RepositorioExpediente, IAutorizacionService autorizacionService)
    {
        this.RepositorioExpediente = RepositorioExpediente;
        this.autorizacionService = autorizacionService;
    }

    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        if (!autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar el expediente.");
        }

        var expediente = RepositorioExpediente.ObtenerPorId(request.ExpedienteId);
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException("No se encontró el expediente con el ID provisto.");
        }

        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);
        RepositorioExpediente.Modificar(expediente);
        return new CambiarEstadoExpedienteResponse(expediente.Estado);
    }
}