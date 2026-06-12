namespace SGE.Consola;

using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Autorizacion;
using SGE.Dominio.Tramites;
using SGE.Dominio.Expedientes;
using SGE.Infraestructura.Persistencia; 
using SGE.Infraestructura.Autorizacion;
using System;

public class Program 
{
    static void Main(string[] args)
    {
        // 1. Inicialización de Repositorios y Servicios
        ITramiteRepository repoTramite = new TramiteTxtRepository();
        IExpedienteRepository repoExpediente = new ExpedienteTxtRepository();
        var autorizacion = new AutorizacionProvisionalService();
        var coordinador = new ActualizacionEstadoExpedienteService(repoExpediente, repoTramite);

        // 2. Casos de Uso
        var altaExpediente = new AgregarExpedienteUseCase(repoExpediente, autorizacion);
        var altaTramite = new AgregarTramiteUseCase(repoTramite, autorizacion, coordinador);
        var listarExpedientes = new ListarExpedientesUseCase(repoExpediente, autorizacion);

        Guid operadorId = Guid.NewGuid();

        Console.WriteLine("=== SIMULACIÓN DE PRUEBAS SGE (FASE 1) ===\n");

        // ------------------------------------------------------------
        // FLUJO 1: CAMINO FELIZ (Alta y Evolución del Estado)
        // ------------------------------------------------------------
        try
        {
            Console.WriteLine("[CAMINO FELIZ]");
            
            // Creación
            var res = altaExpediente.Ejecutar(new AgregarExpedienteRequest("Expediente de Prueba", operadorId));
            Guid expId = res.Id;
            Console.WriteLine($"1. Expediente creado. ID: {expId.ToString().Substring(0,8)}... | Estado: RecienIniciado");

            // Agregar Trámite 1 -> Muta a ParaResolver
            altaTramite.Ejecutar(new AgregarTramiteRequest(expId, operadorId, "Informe Técnico (PaseAEstudio)"));
            var exp = repoExpediente.ObtenerPorId(expId);
            Console.WriteLine($"2. Trámite 'PaseAEstudio' agregado. -> Nuevo Estado: {exp?.Estado}");

            // Agregar Trámite 2 -> Muta a ConResolucion
            altaTramite.Ejecutar(new AgregarTramiteRequest(expId, operadorId, "Firma del Acto (Resolucion)"));
            exp = repoExpediente.ObtenerPorId(expId);
            Console.WriteLine($"3. Trámite 'Resolucion' agregado.    -> Nuevo Estado: {exp?.Estado}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inesperado: {ex.Message}\n");
        }

        // ------------------------------------------------------------
        // FLUJO 2: CAMINO DE ERROR (Validación del Dominio)
        // ------------------------------------------------------------
        Console.WriteLine("[CAMINO DE ERROR]");
        try
        {
            Console.WriteLine("Intentando crear expediente con carátula vacía...");
            altaExpediente.Ejecutar(new AgregarExpedienteRequest("", operadorId));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✔️ Excepción capturada con éxito: {ex.Message}");
        }

        Console.WriteLine("\n===========================================");
    }
}