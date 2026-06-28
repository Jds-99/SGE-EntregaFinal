namespace SGE.Aplicacion;

public class CredencialesInvalidadException : Exception
{
    // Constructor por defecto
    public CredencialesInvalidadException() : base("las credenciales no son validas") { }

    // Constructor que permite personalizar el mensaje (el que usamos en tu UseCase)
    public CredencialesInvalidadException(string mensaje) : base(mensaje) { }
}