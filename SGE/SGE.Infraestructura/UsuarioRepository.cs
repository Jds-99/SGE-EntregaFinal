using System.Linq;
using SGE.Aplicacion; 
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SgeContext _context;

    // Inyectamos el contexto de Entity Framework
    public UsuarioRepository(SgeContext context)
    {
        _context = context;
    }

    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
    }

    public Usuario? ObtenerPorCorreo(string correoElectronico)
{
    var correoNormalizado = correoElectronico.Trim().ToLower();
    
    // Comparación directa de strings en LINQ
    return _context.Usuarios
        .FirstOrDefault(u => u.CorreoElectronico == correoNormalizado);
    }

    // Si tu interfaz tiene métodos como ObtenerPorId o Listar, se verían así:
    public Usuario? ObtenerPorId(Guid id)
    {
        return _context.Usuarios.Find(id);
    }
}