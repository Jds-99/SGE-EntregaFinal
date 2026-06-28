using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion;

public class RegistrarUsuarioRequest
{
    public string? Nombre { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string contrasenia { get; set; } = string.Empty; // Texto plano que viaja por única vez a la api

    public bool EsAdministrador { get; set; } = false;
}