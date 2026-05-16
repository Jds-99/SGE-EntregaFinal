using SGE.Aplicacion;
using SGE.Dominio.Tramites;

public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _nombreArchivo = "Tramites.txt";
    void Agregar(Tramite tramite)
    {
        using var sw = new StreamWriter(_nombreArchivo, true);
        sw.WriteLine(tramite.Id);
        sw.WriteLine(tramite.IdExpediente);
        sw.WriteLine(tramite.Etiqueta);
        sw.WriteLine(tramite.contenido);
        sw.WriteLine(tramite.FechaCreacion);
        sw.WriteLine(tramite.FechaUltimaModificacion);
        sw.WriteLine(tramite.UsuarioUlimoCambio);
    }

}