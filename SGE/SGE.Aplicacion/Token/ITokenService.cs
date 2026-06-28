using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Token;

public interface ITokenService
{
        string GenerarToken(Usuario usuario);
}