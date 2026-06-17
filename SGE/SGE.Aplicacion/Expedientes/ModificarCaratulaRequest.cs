using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;
public record ModificarCaratulaRequest(string NuevaCaratulaTexto, Guid UsuarioId, Guid ExpedienteId);
