using SGE.Aplicacion;

namespace SGE.Infraestructura;

public class UnidadDeTrabajo : IUnidadDeTrabajo
{
    private readonly SgeContext _context;

    public UnidadDeTrabajo(SgeContext context)
    {
        _context = context;
    }

    public void Guardar()
    {
        // EF Core genera el SQL real
        // y lo manda en una sola transacción atómica a SQLite
        _context.SaveChanges();
    }
}