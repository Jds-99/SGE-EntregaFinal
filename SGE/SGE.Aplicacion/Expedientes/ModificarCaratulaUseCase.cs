using System;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion;

public class ModificarCaratulaUseCase
{
    public ModificarCaratulaResponse Ejecutar(ModificarCaratulaRequest request)
    {
        var caratula= new CaratulaExpendiente(request.caratula.ToString());
       
        return new ModificarCaratulaResponse(caratula);
    }
}
