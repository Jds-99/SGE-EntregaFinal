namespace SGE.Aplicacion;

public class RegistrarUsuarioResponse
{
    public Guid Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string CorreoElectronico { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}