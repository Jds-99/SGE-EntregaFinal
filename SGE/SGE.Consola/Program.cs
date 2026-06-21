namespace SGE.Consola;
using System;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Expedientes;
using SGE.Infraestructura.Repository;
using SGE.Infraestructura;
using SGE.Dominio.Comun;
public class Program 
{
    static void Main(string[] args)
    {
        // 1. Inicialización de Repositorios y Servicios
        ITramiteRepository repoTramite = new TramiteTxtRepository();
        IExpedienteRepository repoExpediente = new ExpedienteTxtRepository();
        var autorizacion = new AutorizacionProvisionalService();
        var coordinador = new ActualizacionEstadoExpedienteService(repoExpediente,repoTramite);

        // 2. Casos de Uso
        var altaExpediente = new AgregarExpedienteUseCase(repoExpediente, autorizacion);
        var altaTramite = new AgregarTramiteUseCase(repoTramite,repoExpediente, autorizacion, coordinador);
        var listarExpedientes = new ObtenerTodosExpedientesUseCase(repoExpediente);

        Guid operadorId = Guid.NewGuid();

        Console.WriteLine("=== SIMULACIÓN DE PRUEBAS SGE (FASE 1) ===\n");

        // ------------------------------------------------------------
        // FLUJO 1: CAMINO FELIZ (Alta y Evolución del Estado)
        // ------------------------------------------------------------
        try
        {
            Console.WriteLine("--- CAMINO FELIZ ---");
    
        // 1. Crear el expediente mandando Carátula y UsuarioId
        var resExpediente = altaExpediente.Ejecutar(new AgregarExpedienteRequest("Expediente de Prueba de C#", operadorId));
    
        // Parseamos a Guid de forma segura ya que el Response devuelve strings
        Guid expId = resExpediente.IdExpediente; 
        Console.WriteLine($"[ÉXITO] Expediente creado con ID: {expId.ToString()[..8]}... | Estado Inicial: RecienIniciado");

        // 2. Agregar Trámite 1 (Pase a estudio) -> El estado del expediente debería mutar a 'ParaResolver'
        // Formato DTO: (ExpedienteId, Detalle, Etiqueta, UsuarioId)
        Console.WriteLine("\nAgregando trámite de pase a estudio...");
        altaTramite.Ejecutar(new AgregarTramiteRequest(expId, "Revisión inicial de documentación", "PaseAEstudio", operadorId));
    
        var exp = repoExpediente.ObtenerPorId(expId);
        Console.WriteLine($"-> Estado actual del expediente: {exp?.Estado}");

        // 3. Agregar Trámite 2 (Resolución) -> El estado del expediente debería mutar a 'ConResolucion'
        Console.WriteLine("\nAgregando trámite de resolución...");
        altaTramite.Ejecutar(new AgregarTramiteRequest(expId, "Firma del acto administrativo final", "Resolucion", operadorId));
    
        exp = repoExpediente.ObtenerPorId(expId);
        Console.WriteLine($"-> Estado actual del expediente: {exp?.Estado}\n");
        }
        catch (DominioExcepcion ex)
        {
        Console.WriteLine($"❌ Error de Dominio en el camino feliz: {ex.Message}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inesperado: {ex.Message}\n");
        }

        // ------------------------------------------------------------
        // FLUJO 2: CAMINO DE ERROR (Validaciones del Dominio Rico)
        // ------------------------------------------------------------
        Console.WriteLine("--- CAMINO DE ERROR ---");
        try
        {
            Console.WriteLine("Intentando crear un expediente con la carátula vacía...");
    
        // Esto va a viajar, instanciará el Value Object 'Caratula' y el dominio debería rebotarlo
        altaExpediente.Ejecutar(new AgregarExpedienteRequest("   ", operadorId));
    
        Console.WriteLine("❌ ERROR: El sistema dejó pasar una carátula vacía.");
        }
        catch (DominioExcepcion ex)
        {
            // Captura amigable recomendada por el enunciado para errores de negocio
        Console.WriteLine($"✔️ [VALIDACIÓN CORRECTA] El dominio rechazó la operación: {ex.Message}");
        }
        catch (Exception ex)
        {
        Console.WriteLine($"❌ Se produjo un error inesperado de otro tipo: {ex.Message}");
        }

        Console.WriteLine("\n===========================================");
    }
}