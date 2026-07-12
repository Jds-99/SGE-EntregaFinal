using System;
using SGE.Dominio.Comun;

namespace SGE.Dominio.Expedientes;

public record class CaratulaExpendiente
{
    public  String Valor {get; private set;}
    public CaratulaExpendiente(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException();
        Valor = valor.Trim();
    }
    public override string ToString() => Valor;
}
