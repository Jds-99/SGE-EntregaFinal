using System;
using SGE.Aplicacion.Token;
namespace SGE.Aplicacion;
public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    // constructor para inyeccion de independencias

    public LoginUseCase(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService
    )
    {
        _usuarioRepository= usuarioRepository;
        _passwordHasher=passwordHasher;
        _tokenService=tokenService;
    }
    public LoginResponse Ejecutar(LoginRequest request)
    {
        if(request==null || string.IsNullOrEmpty(request.CorreoElectronico) || string.IsNullOrEmpty(request.contrasenia)){
            throw new ArgumentException (" EL correo y contrasenia son oblgatorios");
        }
        // buscamos el usuario en base de datos por correo electronico normalizando a minisculas como se guardo en el registro
        string correoNormalizado = request.CorreoElectronico.Trim().ToLower();
        var usuario = _usuarioRepository.ObtenerPorCorreo(correoNormalizado);
        /*
        // si no existe tiramos excepcion 
        if (usuario== null)
        {
            throw new CredencialesInvalidadException (" Credenciales incorrectas ");
        }
        // verificar el hash de la contrasenia para ver si coincide con el de la base de datos
        bool claveValida= _passwordHasher.VerifyPassword(request.contrasenia,usuario.contraseniaHash!);
        if (!claveValida)
        {
            throw new CredencialesInvalidadException(" Credenciales incorrectas" );
        }
        */
        //generar el token 
        if (usuario.contraseniaHash != request.contrasenia)
    {
    throw new CredencialesInvalidadException(" Credenciales incorrectas");
    }

        string tokenGenerado= _tokenService.GenerarToken(usuario);

        // retornar el response para la web api
        return new LoginResponse
        {
            Token=tokenGenerado,
            Nombre=usuario.Nombre,
            CorreoElectronico= usuario.CorreoElectronico,
            EsAdministrador = usuario.EsAdministrador
        };
    }
}