using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;

namespace SGE.Infraestructura.Expedientes;

public class ExpedienteTxtRepository : IExpedienteRepository
{
    private readonly string _rutaArchivo = "expedientes.txt";
    public IEnumerable<Expediente> ObtenerTodos()
    {
        var expedientes = new List<Expediente>();
        if (!File.Exists(_rutaArchivo)){
            return expedientes; // Retorna colección vacía si el archivo no existe implícitamente
        }
        // Leemos todas las líneas del archivo de texto plano
        string[] lineas = File.ReadAllLines(_rutaArchivo);
        foreach (string linea in lineas){
            if (string.IsNullOrWhiteSpace(linea)) continue;
            // Suponiendo una persistencia simple delimitada por comas (o la estructura que definas)
            string[] datos = linea.Split(',');
            // Parseo de los tipos primitivos persistidos
            Guid id = Guid.Parse(datos[0]);
            string caratulaTexto = datos[1];
            DateTime fechaCreacion = DateTime.Parse(datos[2]);
            DateTime fechaModificacion = DateTime.Parse(datos[3]);
            Guid usuarioId = Guid.Parse(datos[4]);
            EstadoExpediente estado = Enum.Parse<EstadoExpediente>(datos[5]);
            // CRÍTICO: Reconstrucción obligatoria mediante Factory Method (No usar constructor común)
            Expediente expedienteReconstruido = Expediente.Reconstruir(
                id, 
                new Caratula(caratulaTexto), 
                fechaCreacion, 
                fechaModificacion, 
                usuarioId, 
                estado
            );
            expedientes.Add(expedienteReconstruido);
        }
        // C# convierte implícitamente List<T> a IEnumerable<T> hacia afuera
        return expedientes; 
    }

    public void Agregar(Expediente expediente){
        // Persistencia simple: Append al archivo de texto plano
        using var sw = new StreamWriter(_rutaArchivo, append: true);
        // Serialización simple: Convertir a formato delimitado por comas (o el formato que definas)
        string linea = $"{expediente.Id},{expediente.Caratula.Texto},{expediente.FechaCreacion},{expediente.FechaUltimaModificacion},{expediente.UsuarioUltimoCambio},{expediente.Estado}";
        sw.WriteLine(linea);
    }

    private void GuardarTodoElArchivo(List<Expediente> listaExpedientes){
        // append: false sobreescribe el archivo por completo con los cambios nuevos
        using var writer = new StreamWriter(_rutaArchivo, append: false);    
        foreach (var exp in listaExpedientes){
            string linea = $"{exp.Id},{exp.Caratula.Texto},{exp.FechaCreacion},{exp.FechaUltimaModificacion},{exp.UsuarioUltimoCambio},{exp.Estado}";
            writer.WriteLine(linea);
        }
    }

    public void Modificar(Expediente expedienteModificado){
        var todos = ObtenerTodos().ToList();
         //Buscamos el índice usando un for
        int indice = -1;
        for (int i = 0; i < todos.Count; i++){
            if (todos[i].Id == expedienteModificado.Id){
                indice = i;
                break; 
            }
        }
        if (indice == -1){
            throw new RepositorioException($"No se encontró el expediente con ID {expedienteModificado.Id} para modificar.");
        }
        //Reemplazamos el expediente antiguo por el modificado en la lista
        todos[indice] = expedienteModificado;
        GuardarTodoElArchivo(todos);
    }

    public void Eliminar(Guid id){
        var todos = ObtenerTodos();
        // Filtramos la lista: tomamos todos MENOS el que queremos borrar
        var listaFiltrada = todos.Where(e => e.Id != id).ToList(); //todos.Where devuelve IEnumerable, ToList() lo convierte a List para poder comparar el conteo (sugerido por la IA, como una solucion mas practica)
        if (todos.Count == listaFiltrada.Count){
            throw new InvalidOperationException($"No se encontró el expediente con ID {id} para eliminar.");
        }
        //Volvemos a escribir el archivo completo con los que quedaron
        GuardarTodoElArchivo(listaFiltrada);
    }

}
