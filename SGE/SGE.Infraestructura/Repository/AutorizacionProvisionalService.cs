using SGE.Infraestructura;

namespace SGE.Infraestructura;

public class AutorizacionProvisionalService : IAutorizacionService {
    public bool PoseeElPermiso(Guid usuarioId, string permiso)
    {
        // De momento solo devuelve true, pedido por el enunciado.
        return true;
    }
}