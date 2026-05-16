using System;
using SGE.Dominio.Expedientes;
using SGE.Aplicacion;
namespace SGE.Aplicacion;

public class ModificarCaratulaUseCase
{
    public ModificarCaratulaResponse Ejecutar(ModificarCaratulaRequest request,Guid IdUsuario)
    {
        if (IdUsuario == Guid.Empty)
        {
            throw new AutorizacionException("Id invalido");
        }
        var caratula= new CaratulaExpendiente(request.caratula.ToString());

        return new ModificarCaratulaResponse(caratula,IdUsuario);
    }
}