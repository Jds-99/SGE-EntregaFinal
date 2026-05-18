using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public record AgregarExpedienteRequest(string Caratula, int IdUsuario);