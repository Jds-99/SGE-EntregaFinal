using System;
using System.Collections.Generic;
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

        // Traer los usuarios a memoria y comparar el valor textual del Correo
        return _context.Usuarios
            .AsEnumerable()
            .FirstOrDefault(u => u.CorreoElectronico.ToString().Trim().ToLower() == correoNormalizado);
    }

    // Si tu interfaz tiene métodos como ObtenerPorId o Listar, se verían así:
    public Usuario? ObtenerPorId(Guid id)
    {
        return _context.Usuarios.Find(id);
    }
    public List<Usuario> ObtenerTodos()
    {
        // Trae todas las filas de la tabla Usuarios y las convierte en una Lista de C#
        return _context.Usuarios.ToList();
    }
    public void Modificar(Usuario usuario)
    {
        // Le avisa a EF Core que las propiedades de este usuario cambiaron 
        // para que cuando se llame a Guardar(), genere el comando SQL "UPDATE..."
        _context.Usuarios.Update(usuario);
    }
    public void Eliminar(Guid id)
    {
        // Primero buscamos si el usuario existe en la base de datos
        var usuario = _context.Usuarios.Find(id);
        
        if (usuario != null)
        {
            // Si lo encuentra, lo marca para borrado. Al Guardar() generará el "DELETE..."
            _context.Usuarios.Remove(usuario);
        }
        else
        {
            throw new Exception($"No se puede eliminar el usuario. El ID {id} no existe.");
        }
    }
}