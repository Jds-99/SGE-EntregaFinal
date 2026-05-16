using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

public interface IExpedienteRepository
{
    void  Agregar(Expediente expediente);
    void Modificar(Guid id);
    void Eliminar(Guid id);
    Expediente? ObtenerPorId(Guid id);
    IEnumerable<Tramite> ObtenerTodos(Guid id);
    
}
