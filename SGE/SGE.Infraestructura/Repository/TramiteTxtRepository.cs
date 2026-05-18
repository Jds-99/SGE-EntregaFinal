using SGE.Aplicacion;
using SGE.Dominio.Tramites;

namespace SGE.Infraestructura.Tramites;

public class TramiteTxtRepository : ITramiteRepository{
    private readonly string _nombreArchivo = "Tramites.txt";
    public void Agregar(Tramite tramite){
        using var sw = new StreamWriter(_nombreArchivo, true);
        EscribirTramiteEnStream(sw, tramite);
    }

    public IEnumerable<Tramite> ObtenerTodos(){
        var lista = new List<Tramite>();    
        if (!File.Exists(_nombreArchivo)) return lista;
        using var sr = new StreamReader(_nombreArchivo);
        while (!sr.EndOfStream){
            var id = Guid.Parse(sr.ReadLine() ?? "");
            var idExpediente = Guid.Parse(sr.ReadLine() ?? "");
            var etiqueta = (EtiquetaTramite)int.Parse(sr.ReadLine() ?? "0");
            var contenido = sr.ReadLine() ?? "";
            var fechaCreacion = DateTime.Parse(sr.ReadLine() ?? "");
            var fechaUltimaModificacion = DateTime.Parse(sr.ReadLine() ?? "");
            var usuarioUltimoCambio = Guid.Parse(sr.ReadLine() ?? "");
            var contenidoVO = new ContenidoTramite(contenido);
            var tramite = Tramite.Reconstruir(id, idExpediente, etiqueta, contenidoVO, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio);
            lista.Add(tramite);
        }
        return lista; 
    }
    
    private void EscribirTramiteEnStream(StreamWriter writer, Tramite tramite){
        writer.WriteLine(tramite.Id);
        writer.WriteLine(tramite.IdExpediente);
        writer.WriteLine((int)tramite.Etiqueta);
        writer.WriteLine(tramite.contenido); 
        writer.WriteLine(tramite.FechaCreacion.ToString()); // Formato "o" (round-trip) evita problemas de fechas
        writer.WriteLine(tramite.FechaUltimaModificacion.ToString());
        writer.WriteLine(tramite.UsuarioUltimoCambio);
    }

    private void GuardarTodoElArchivo(List<Tramite> listaTramites){
        // append: false sobrescribe el archivo por completo. Usamos _nombreArchivo correctamente.
        using var writer = new StreamWriter(_nombreArchivo, append: false);    
        foreach (var tramite in listaTramites){
            EscribirTramiteEnStream(writer, tramite); // Reutilizamos el formato de líneas consecutivas
        }
    }

    public void Modificar(Tramite tramiteModificado){
        var todos = ObtenerTodos().ToList();
        int indice = -1;
        for (int i = 0; i < todos.Count; i++){
            if (todos[i].Id == tramiteModificado.Id){
                indice = i;
                break; 
            }
        }
        if (indice == -1){
            throw new RepositorioException($"No se encontró el trámite con ID {tramiteModificado.Id} para modificar.");
        }
        todos[indice] = tramiteModificado;
        GuardarTodoElArchivo(todos);
    }

    public void Eliminar(Guid id){
        var todos = ObtenerTodos().ToList(); // Lo pasamos a lista para poder usar la propiedad .Count de forma segura
        var listaFiltrada = todos.Where(e => e.Id != id).ToList(); 
        if (todos.Count == listaFiltrada.Count){
            throw new RepositorioException($"No se encontró el trámite con ID {id} para eliminar.");
        }
        GuardarTodoElArchivo(listaFiltrada);
    }
}