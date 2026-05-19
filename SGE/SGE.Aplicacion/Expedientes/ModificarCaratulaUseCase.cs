using System;
using SGE.Dominio.Expedientes;
using SGE.Aplicacion;
namespace SGE.Aplicacion;

namespace SGE.Aplicacion;

public class ModificarCaratulaUseCase
{
    private readonly IExpedienteRepository RepositorioExpedientes;
    private readonly IAutorizacionService AutorizacionService;

    public ModificarCaratulaUseCase(IExpedienteRepository RepositorioDeExpedientes, IAutorizacionService ServivioDeAutorizacion)
    {
        this.RepositorioExpedientes = RepositorioDeExpedientes ?? throw new ArgumentNullException(nameof(RepositorioDeExpedientes));
        this.AutorizacionService = ServivioDeAutorizacion ?? throw new ArgumentNullException(nameof(ServivioDeAutorizacion));
    }

    public ModificarCaratulaResponse Ejecutar(ModificarCaratulaRequest request)
    {
        if (!this.AutorizacionService.PoseePermiso(request.UsuarioId, Permiso.ModificarExpediente))
        {
            throw new AutorizacionException($"El usuario {request.UsuarioId} no tiene permisos para modificar carátulas.");
        }

        Expediente expediente = this.RepositorioExpedientes.ObtenerPorId(request.ExpedienteId) 
            ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");

        var nuevaCaratula = new CaratulaExpediente(request.NuevaCaratulaTexto);
        expediente.CambiarCaratula(nuevaCaratula, request.UsuarioId);

        this.RepositorioExpedientes.Actualizar(expediente);

        return new ModificarCaratulaResponse(expediente.Id, expediente.Caratula.Valor);
    }
}