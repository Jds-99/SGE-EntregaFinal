using System;
using System.Collections.Generic;
using System.Linq;
using SGE.Aplicacion;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;
public record ObtenerTodosExpedientesRequest();

public class ObtenerTodosExpedientesUseCase(IExpedienteRepository expedienteRepo)
{
    // Quitamos la validación de permisos si la lectura es pública para los usuarios
    public IEnumerable<ExpedienteDTO> Ejecutar()
    {
        // 1. Buscamos todas las entidades del repositorio directo
        IEnumerable<Expediente> expedientes = expedienteRepo.ObtenerTodos();

        // 2. Transformamos las entidades de Dominio a DTOs limpios
        return expedientes.Select(e => new ExpedienteDTO(
            Id: e.Id,
            Caratula: e.Caratula.ToString(),
            Estado: e.Estado.ToString(),
            FechaCreacion: e.FechaCreacion,
            UsuarioId: e.UsuarioUltimoCambio
        )).ToList();
    }
}

public record ExpedienteDTO(
    Guid Id, 
    string Caratula, 
    string Estado, 
    DateTime FechaCreacion, 
    Guid UsuarioId
);