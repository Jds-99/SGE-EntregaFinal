namespace SGE.Aplicacion;
public class LoginResponse
{
    public string? Token {get; set;}
    public string? Nombre{get; set;}
    public string? CorreoElectronico{get; set;}
    public bool EsAdministrador{get;set;}
}