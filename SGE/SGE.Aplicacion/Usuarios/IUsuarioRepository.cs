using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion;

public interface IUsuarioRepository
{
    void Agregar(Usuario usuario);
    Usuario? ObtenerPorCorreo(string correoElectronico);
    Usuario? ObtenerPorId(Guid id);
    List<Usuario> ObtenerTodos();
    void Modificar(Usuario usuario);
    void Eliminar(Guid id);
}