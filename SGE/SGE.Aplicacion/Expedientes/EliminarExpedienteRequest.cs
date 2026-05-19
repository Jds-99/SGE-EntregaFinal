using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record EliminarExpedienteRequest(string IdExpediente, string UsuarioId);