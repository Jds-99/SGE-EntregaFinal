using System;
using System.Collections.Generic;
using SGE.Dominio.Comun;

namespace SGE.Dominio.Usuarios;

public class Usuario
{
    public Guid Id { get; private set; } 
    public string Nombre { get; private set; } 
    public string CorreoElectronico { get; private set; } 
    public string contraseniaHash { get; private set; } 
    public bool EsAdministrador { get; private set; } 
    
    private readonly List<Permiso> _permisos = new List<Permiso>(); 
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly(); 

    // Constructor privado para ORM / Base de datos
    private Usuario() 
    {
        Id = Guid.NewGuid();
        Nombre = string.Empty; 
        CorreoElectronico = string.Empty; 
        contraseniaHash = string.Empty; 
    }

    public Usuario(string nombre, string correoElectronico, string contraseniaHash, bool esAdministrador = false)
    {
        if (string.IsNullOrWhiteSpace(nombre)) 
          throw new DominioException("El nombre no puede estar vacío."); 
        if (string.IsNullOrWhiteSpace(correoElectronico) || !correoElectronico.Contains("@")) 
        throw new DominioException("El formato del correo electrónico no es válido."); 
        if (string.IsNullOrWhiteSpace(contraseniaHash)) 
        throw new DominioException("La contrasenia hash no puede estar vacía."); 

            Id = Guid.NewGuid(); 
            Nombre = nombre; 
            CorreoElectronico = correoElectronico.Trim().ToLower(); 
            this.contraseniaHash = contraseniaHash; 
            EsAdministrador = esAdministrador; 
    }

    public void AsignarPermiso(Permiso permiso)
    {
       if (EsAdministrador) return; 
        if (!_permisos.Contains(permiso)) 
        {
            _permisos.Add(permiso);
        }
    }

    public void RevocarPermiso(Permiso permiso)
    {
       if (_permisos.Contains(permiso))
        {
         _permisos.Remove(permiso); 
        }
    }

    public void ReemplazarPermisos(List<Permiso> nuevosPermisos)
    {
        _permisos.Clear(); // Limpiamos sobre la lista privada 
        if (nuevosPermisos != null) 
        {
            foreach (var permiso in nuevosPermisos) 
            {
                if (!_permisos.Contains(permiso)) 
                {
                    _permisos.Add(permiso); 
                }
            }
        }
    }

    public bool TienePermiso(Permiso permiso)
    {
        if (EsAdministrador) return true; 
        return _permisos.Contains(permiso); 
    }

    // Métodos de comportamiento para modificar datos de forma segura
    public void ActualizarDatos(string nuevoNombre, string nuevoCorreo)
    {
        if (string.IsNullOrWhiteSpace(nuevoNombre))
            throw new DominioException("El nombre no puede estar vacío.");
        if (string.IsNullOrWhiteSpace(nuevoCorreo) || !nuevoCorreo.Contains("@"))
            throw new DominioException("El formato del correo electrónico no es válido.");

        Nombre = nuevoNombre;
        CorreoElectronico = nuevoCorreo.Trim().ToLower();
    }

    public void Cambiarcontrasenia(string nuevacontraseniaHash)
    {
        if (string.IsNullOrWhiteSpace(nuevacontraseniaHash))
            throw new DominioException("La contrasenia hash no puede estar vacía.");
        
        contraseniaHash = nuevacontraseniaHash;
    }
}