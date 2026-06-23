using System;
using System.Collections.Generic;
using SGE.Dominio.Comun;

namespace SGE.Dominio.Usuarios;

public class Usuario
{
    public Guid Id {get;private set;}
    public String? Nombre{get; private set;}
    public String? CorreoElectronico{get; private set;}
    public String? ContraseñaHash{ get;private set;}
    public bool EsAdministrador {get; private set;}
    // lista de permisos
    private readonly List<Permiso> _permisos= new List<Permiso>();
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();

    // constructor privado para la base de datos

    private Usuario() 
    {
        Id = Guid.NewGuid();
        Nombre = string.Empty;
        CorreoElectronico = string.Empty;
        ContraseñaHash = string.Empty;
    }

    public Usuario(string nombre, string correoElectronico, string contraseñaHash, bool esAdministrador = false)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DominioExcepcion("El nombre no puede estar vacío.");
            
        if (string.IsNullOrWhiteSpace(correoElectronico) || !correoElectronico.Contains("@"))
            throw new DominioExcepcion("El formato del correo electrónico no es válido.");

        if (string.IsNullOrWhiteSpace(contraseñaHash))
            throw new DominioExcepcion("La contraseña hash no puede estar vacía.");

        Id = Guid.NewGuid();
        Nombre = nombre;
        CorreoElectronico = correoElectronico.Trim().ToLower(); // Normalizamos el correo
        ContraseñaHash = contraseñaHash;
        EsAdministrador = esAdministrador;
    }

    // asignar permiso
    public void AsignarPermiso(Permiso permiso)
    {
        // Si es administrador, conceptualmente ya tiene todos los permisos
        if (EsAdministrador) return;

        if (!_permisos.Contains(permiso))
        {
            _permisos.Add(permiso);
        }
    }

    // revocar permisos
    public void RevocarPermiso(Permiso permiso)
    {
        if (_permisos.Contains(permiso))
        {
            _permisos.Remove(permiso);
        }
    }

    // ver si tiene permiso
    public bool TienePermiso(Permiso permiso)
    {
        // El administrador siempre tiene acceso a todo
        if (EsAdministrador) return true;
        
        return _permisos.Contains(permiso);
    }
}