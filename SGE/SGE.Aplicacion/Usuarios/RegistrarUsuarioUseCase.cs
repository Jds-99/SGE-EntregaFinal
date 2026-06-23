using System;
using SGE.Dominio;
using SGE.Dominio.Comun;
using SGE.Dominio.Usuarios;
namespace SGE.Aplicacion;

public class RegistrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    // Actualizamos el constructor para recibir la UoW
    public RegistrarUsuarioUseCase(
        IUsuarioRepository usuarioRepository, 
        IPasswordHasher passwordHasher,
        IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        // Validar si el usuario ya existe por correo
        var usuarioExistente = _usuarioRepository.ObtenerPorCorreo(request.CorreoElectronico);
        if (usuarioExistente is not null)
        {
            // 🚨 TIP PRO: Usá tu excepción personalizada de dominio, no la genérica Exception
            throw new DominioExcepcion("El correo electrónico ya se encuentra registrado.");
        }

        // Hashear la contraseña usando SHA-256 
        string hashContraseña = _passwordHasher.HashPassword(request.Contraseña);

        // Crear la entidad de Dominio de forma segura (Acá nace tu Value Object Correo)
        var nuevoUsuario = new Usuario(
            request.Nombre ?? "Usuario sin nombre", 
            request.CorreoElectronico, 
            hashContraseña, 
            request.EsAdministrador
        );

        // Agregamos la entidad al contexto a través del repositorio
        _usuarioRepository.Agregar(nuevoUsuario);

        // Confirmamos los cambios en SQLite
        _unidadDeTrabajo.Guardar(); 

        // Mapear al Response para devolver solo lo necesario a la capa Web API
        return new RegistrarUsuarioResponse
        {
            Id = nuevoUsuario.Id,
            Nombre = nuevoUsuario.Nombre,
            // Modificación del Value Object: Accedemos al string interno mediante .Valor
            CorreoElectronico = nuevoUsuario.CorreoElectronico, 
            Mensaje = "Usuario registrado exitosamente."
        };
    }
}
