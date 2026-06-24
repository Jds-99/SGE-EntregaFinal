using SGE.Aplicacion;
using SGE.Dominio.Comun;
public class EliminarUsuarioUseCase
{
    private readonly IUsuarioRepository _repository;
    private readonly IUnidadDeTrabajo _unitOfWork; 

    // 2. Pasarlo por el constructor
    public EliminarUsuarioUseCase(IUsuarioRepository repository, IUnidadDeTrabajo unitOfWork)
    {
        _repository = repository;   
        _unitOfWork = unitOfWork; 
    }

    public void Ejecutar(EliminarUsuarioRequest request)
    {
        var operador = _repository.ObtenerPorId(request.OperadorId);
        if (operador == null || !operador.EsAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren privilegios de administrador.");

        var usuario = _repository.ObtenerPorId(request.UsuarioAEliminar);
        if (usuario == null)
            throw new DominioExcepcion("El usuario a eliminar no existe.");

        _repository.Eliminar(request.UsuarioAEliminar);

        _unitOfWork.Guardar(); 
    }
}
