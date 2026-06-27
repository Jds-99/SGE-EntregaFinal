namespace SGE.Aplicacion;

public interface IPasswordHasher
{
    // Convierte la contrasenia en texto plano en su representación Hash string
    string HashPassword(string password);

    // Compara una contrasenia ingresada en texto plano contra un Hash para verificar identidad
    bool VerifyPassword(string password, string hashedPassword);
}