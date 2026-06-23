using System.Text.RegularExpressions;
using SGE.Dominio.Comun; // Asegurate de usar el namespace de tus excepciones de dominio

namespace SGE.Dominio.Usuarios;

public record Correo
{
    public string Valor { get; init; }

    // Constructor para EF Core y recreación
    private Correo() 
    {
        Valor = null!;
    }

    public Correo(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioExcepcion("El correo electrónico no puede estar vacío.");

        // Validación estándar de formato de email
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        if (!emailRegex.IsMatch(valor))
            throw new DominioExcepcion($"El formato del correo '{valor}' no es válido.");

        Valor = valor.Trim().ToLower(); // Lo normalizamos a minúsculas
    }

}