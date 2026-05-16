using System;
namespace SGE.Dominio.Comun;

public class DominioExcepcion : Exception
{
    public DominioExcepcion(){}
    public DominioExcepcion(string? messaje) : base(messaje) {}
    public DominioExcepcion(string? messaje, Exception? innerException) : base(messaje,innerException){}
}
