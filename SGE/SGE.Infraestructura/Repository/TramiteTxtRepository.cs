namespace SGE.Infraestructura.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SGE.Aplicacion;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Tramites;

public class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _nombreArchivo = "Tramites.txt";

    public void Agregar(Tramite tramite)
    {
        using var sw = new StreamWriter(_nombreArchivo, append: true);
        // Cambiamos a punto y coma (;) y accedemos a la propiedad .Valor (o .ToString()) del VO
        sw.WriteLine($"{tramite.Id};{tramite.IdExpediente};{(int)tramite.Etiqueta};{tramite.contenido};{tramite.FechaCreacion:o};{tramite.FechaUltimaModificacion:o};{tramite.UsuarioUlimoCambio}");
    }

    public IEnumerable<Tramite> ObtenerTodos()
    {
        var lista = new List<Tramite>();    
        if (!File.Exists(_nombreArchivo)) return lista;
        
        string[] lineas = File.ReadAllLines(_nombreArchivo);
        foreach (string linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;

            //  Separamos usando punto y coma (;)
            string[] datos = linea.Split(';');

            Guid id = Guid.Parse(datos[0]);
            Guid idExpediente = Guid.Parse(datos[1]);
            EtiquetaTramite etiqueta = (EtiquetaTramite)int.Parse(datos[2]);
            string contenidoTexto = datos[3];
            DateTime fechaCreacion = DateTime.Parse(datos[4]);
            DateTime fechaUltimaModificacion = DateTime.Parse(datos[5]);
            Guid usuarioUltimoCambio = Guid.Parse(datos[6]);

            var contenidoVO = new Contenido(contenidoTexto);
            
            var tramite = Tramite.FactoryMethodTramite(
                id, 
                idExpediente, 
                usuarioUltimoCambio, 
                contenidoVO, 
                fechaCreacion, 
                fechaUltimaModificacion, 
                etiqueta
            );
            lista.Add(tramite);
        }
        return lista; 
    }

    private void GuardarTodoElArchivo(List<Tramite> listaTramites)
    {
        using var sw = new StreamWriter(_nombreArchivo, append: false);    
        foreach (var tramite in listaTramites)
        {
            // Formato consistente con punto y coma (;)
            sw.WriteLine($"{tramite.Id};{tramite.IdExpediente};{(int)tramite.Etiqueta};{tramite.contenido};{tramite.FechaCreacion:o};{tramite.FechaUltimaModificacion:o};{tramite.UsuarioUlimoCambio}");
        }
    }

    public void Modificar(Tramite tramiteModificado)
    {
        var todos = ObtenerTodos().ToList();
        int indice = todos.FindIndex(t => t.Id == tramiteModificado.Id);
        
        if (indice == -1)
        {
            throw new RepositorioException($"No se encontró el trámite con ID {tramiteModificado.Id} para modificar.");
        }
        
        todos[indice] = tramiteModificado;
        GuardarTodoElArchivo(todos);
    }

    public void Eliminar(Guid id)
    {
        var todos = ObtenerTodos().ToList();
        var listaFiltrada = todos.Where(e => e.Id != id).ToList(); 
        
        if (todos.Count == listaFiltrada.Count)
        {
            throw new RepositorioException($"No se encontró el trámite con ID {id} para eliminar.");
        }
        
        GuardarTodoElArchivo(listaFiltrada);
    }

    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid idExpediente)
    {
        if (!File.Exists(_nombreArchivo))
        {
            return new List<Tramite>();
        }
        return ObtenerTodos().Where(t => t.IdExpediente == idExpediente).ToList();
    }
}