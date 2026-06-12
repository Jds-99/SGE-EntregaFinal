namespace SGE.Aplicacion;

public class EntidadNoEncontradaException : Exception
{
    // Constructor por defecto
    public EntidadNoEncontradaException() : base("La entidad solicitada no fue encontrada.") { }

    // Constructor que permite personalizar el mensaje (el que usamos en tu UseCase)
    public EntidadNoEncontradaException(string mensaje) : base(mensaje) { }
}