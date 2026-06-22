using System;
using SGE.Dominio;

namespace SGE.Aplicacion;

public class RegistrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;

    // Constructor para inyección de dependencias
    public RegistrarUsuarioUseCase(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
    }
    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        // Validar si el usuario ya existe por correo
        var usuarioExistente = _usuarioRepository.ObtenerPorCorreo(request.CorreoElectronico);
        if (usuarioExistente != null)
        {
            throw new Exception("El correo electrónico ya se encuentra registrado.");
        }

        // Hashear la contraseña usando SHA-256 
        string hashContraseña = _passwordHasher.HashPassword(request.Contraseña);

        // Crear la entidad de Dominio de forma segura
        var nuevoUsuario = new Usuario(
            request.Nombre, 
            request.CorreoElectronico, 
            hashContraseña, 
            request.EsAdministrador
        );
        // Guardar en la Base de Datos a través del repositorio
        _usuarioRepository.Agregar(nuevoUsuario);

        // Mapear al Response para devolver solo lo necesario a la capa Web API
        
        return new RegistrarUsuarioResponse
        {
            Id = nuevoUsuario.Id,
            Nombre = nuevoUsuario.Nombre!,
            CorreoElectronico = nuevoUsuario.CorreoElectronico!,
            Mensaje = "Usuario registrado exitosamente."
        };
    }
}
