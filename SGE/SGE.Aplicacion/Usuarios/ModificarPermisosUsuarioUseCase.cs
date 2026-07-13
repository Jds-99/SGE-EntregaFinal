using System;
using System.Collections.Generic;
using SGE.Aplicacion;
using SGE.Dominio.Usuarios;
namespace SGE.Aplicacion;

public class ModificarPermisosRequest
{
    public Guid UsuarioId { get; set; }
    public List<Permiso> NuevosPermisos { get; set; } = new List<Permiso>();
    public Guid IdUsuarioOperador { get; set; }
}

public class ModificarPermisosResponse
{
    public bool Exitoso { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}

public class ModificarPermisosUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ModificarPermisosUsuarioUseCase(IUsuarioRepository usuarioRepository, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepository = usuarioRepository;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public ModificarPermisosResponse Ejecutar(ModificarPermisosRequest request)
    {
        if (request == null) throw new ArgumentException("Request inválido.");

        var operador = _usuarioRepository.ObtenerPorId(request.IdUsuarioOperador);
        if (operador == null || !operador.EsAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren permisos de Administrador.");

        var usuario = _usuarioRepository.ObtenerPorId(request.UsuarioId);
        if (usuario == null)
            throw new CredencialesInvalidadException("El usuario especificado no existe.");

        usuario.ReemplazarPermisos(request.NuevosPermisos);
        _usuarioRepository.Modificar(usuario);
        _unidadDeTrabajo.Guardar();
        return new ModificarPermisosResponse { Exitoso = true, Mensaje = "Permisos actualizados con éxito." };
    }
}