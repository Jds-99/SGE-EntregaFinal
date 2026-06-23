using SGE.Dominio;

namespace SGE.Aplicacion;

public interface ITokenService
{
        string GenerarToken(Usuario usuario);
}