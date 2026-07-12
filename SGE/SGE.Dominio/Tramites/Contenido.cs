using System;
using System.Diagnostics.Contracts;
using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public record class Contenido
{
    public String Valor {get; private set;}
    public Contenido(String valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException();
        Valor = valor.Trim();
    }
    public override string ToString() => Valor;

}
