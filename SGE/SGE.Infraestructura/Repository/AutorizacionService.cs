using System;
using SGE.Aplicacion;
using SGE.Dominio.Usuarios;

namespace SGE.Infraestructura.Seguridad;

public class AutorizacionService : IAutorizacionService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AutorizacionService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public bool PoseeElPermiso(Guid usuarioId, string permisoRequeridoTexto)
    {
        var usuario = _usuarioRepository.ObtenerPorId(usuarioId);
        if (usuario == null) return false;

     
        if (usuario.EsAdministrador) return true;
        if (!Enum.TryParse<Permiso>(permisoRequeridoTexto, out var permisoRequerido))
        {
            return false; // Si mandan un texto que no existe en el enum, no tiene permiso
        }

   // regla de implicancia 3.3
        if (permisoRequerido == Permiso.TramiteBaja && usuario.TienePermiso(Permiso.ExpedienteBaja))
        {
            return true;
        }

      
        return usuario.TienePermiso(permisoRequerido);
    }
}