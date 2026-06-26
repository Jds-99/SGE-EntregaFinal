using System;
namespace SGE.Dominio.Comun;

public class DominioException : Exception
{
    public DominioException(){}
    public DominioException(string? messaje) : base(messaje) {}
    public DominioException(string? messaje, Exception? innerException) : base(messaje,innerException){}
}
