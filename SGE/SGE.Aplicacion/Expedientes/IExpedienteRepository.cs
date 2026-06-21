namespace SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

public interface IExpedienteRepository
{
    void  Agregar(Expediente expediente);
    void Modificar(Expediente expediente);
    void Eliminar(Guid id);
    Expediente? ObtenerPorId(Guid id);
    IEnumerable<Expediente> ObtenerTodos();
    
}
