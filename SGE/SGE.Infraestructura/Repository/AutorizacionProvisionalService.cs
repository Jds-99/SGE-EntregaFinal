using SGE.Aplicacion.Autorizacion;

namespace SGE.Infraestructura;

public class AutorizacionProvisionalService : IAutorizacionService {
    public bool PoseeElPermiso(Guid usuarioId, string permiso)
    {
        // De momento solo devuelve true, pedido por el
        return true;
    }
}