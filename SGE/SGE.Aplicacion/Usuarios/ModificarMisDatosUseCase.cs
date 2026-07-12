using SGE.Dominio.Comun;
using SGE.Aplicacion;

public record ModificarMisDatosRequest(
    Guid UserIdDesdeToken, 
    Guid UserIdAModificar, 
    string NuevoNombre, 
    string NuevoCorreo, 
    string NuevacontraseniaPura
);

// Modificación en el Caso de Uso:
public class ModificarMisDatosUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher _hasher;
    private readonly IUnidadDeTrabajo? _unidadDeTrabajo;

    public ModificarMisDatosUseCase(IUsuarioRepository repository, IPasswordHasher hasher)
    {
        _repository = repository;
        _hasher = hasher;
    }

    public void Ejecutar(ModificarMisDatosRequest request)
    {
        if (request.UserIdDesdeToken != request.UserIdAModificar)
            throw new DominioException("No posee autorización para modificar los datos de otro usuario.");

        var usuario = _repository.ObtenerPorId(request.UserIdAModificar);
        if (usuario == null)
            throw new DominioException("Usuario no encontrado.");

        usuario.ActualizarDatos(request.NuevoNombre, request.NuevoCorreo);
        
        if (!string.IsNullOrWhiteSpace(request.NuevacontraseniaPura))
        {
            string nuevoHash = _hasher.HashPassword(request.NuevacontraseniaPura);
            usuario.Cambiarcontrasenia(nuevoHash);
        }

        _repository.Modificar(usuario);
        _unidadDeTrabajo.Guardar();
    }
}