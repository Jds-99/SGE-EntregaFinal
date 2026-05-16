using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public record class ModificarCaratulaRequest(CaratulaExpendiente caratula, Guid id)
{

}
