using SGE.Dominio;

namespace SGE.Aplicacion;

public interface IUsuarioRepository
{
    void Agregar(Usuario usuario);
    Usuario? ObtenerPorCorreo(string correoElectronico);
}