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
    if (request == null || string.IsNullOrWhiteSpace(request.CorreoElectronico))
    {
        throw new ArgumentException("El correo electrónico es obligatorio.");
    }

    string correoNormalizado = request.CorreoElectronico.Trim().ToLower();
    
    var usuarioExistente = _usuarioRepository.ObtenerPorCorreo(new Correo(correoNormalizado));
    if (usuarioExistente is not null)
    {
        throw new DominioException("El correo electrónico ya se encuentra registrado.");
    }


    string hashcontrasenia = _passwordHasher.HashPassword(request.contrasenia);

   
    var nuevoUsuario = new Usuario(
        request.Nombre ?? "Usuario sin nombre", 
        correoNormalizado, 
        hashcontrasenia, 
        esAdministrador: false 
    );

    _usuarioRepository.Agregar(nuevoUsuario);

    _unidadDeTrabajo.Guardar(); 

    return new RegistrarUsuarioResponse
    {
        Id = nuevoUsuario.Id,
        Nombre = nuevoUsuario.Nombre!,
        CorreoElectronico = nuevoUsuario.CorreoElectronico.Valor, 
        Mensaje = "Usuario registrado exitosamente."
    };   
}
}
