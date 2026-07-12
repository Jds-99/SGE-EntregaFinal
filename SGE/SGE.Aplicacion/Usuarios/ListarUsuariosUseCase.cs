using SGE.Aplicacion;
using SGE.Dominio.Usuarios;

public record ListarUsuariosRequest(Guid OperadorId);

public record UsuarioResponseDto(
    Guid Id, 
    string Nombre, 
    string CorreoElectronico, 
    bool EsAdministrador,
    IReadOnlyCollection<Permiso> Permisos
);

// Modificación en el Caso de Uso:
public class ListarUsuariosUseCase
{
    private readonly IUsuarioRepository _repository;

    public ListarUsuariosUseCase(IUsuarioRepository repository) => _repository = repository;

    public List<UsuarioResponseDto> Ejecutar(ListarUsuariosRequest request)
    {
        var operador = _repository.ObtenerPorId(request.OperadorId);
        if (operador == null || !operador.EsAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren privilegios de administrador.");

        var usuarios = _repository.ObtenerTodos();
        
        // Mapeamos las entidades de dominio a DTOs de salida seguros
        return usuarios.Select(u => new UsuarioResponseDto(
            u.Id, 
            u.Nombre, 
            u.CorreoElectronico.ToString(), 
            u.EsAdministrador,
            u.Permisos
        )).ToList();
    }
}