using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public record  ModificarCaratulaResponse(CaratulaExpendiente caratula, Guid id);
