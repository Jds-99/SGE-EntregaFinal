using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion; // Donde guardaste tus interfaces (IUnidadDeTrabajo, etc.)
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
namespace SGE.Infraestructura.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services)
    {
        // 1. Registramos el Contexto de EF Core
        services.AddDbContext<SgeContext>();

        // 2. Registramos la Unidad de Trabajo real
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();

        // 3. Registramos los Repositorios reales
        services.AddScoped<IExpedienteRepository, ExpedienteTxtRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        // services.AddScoped<ITramiteRepository, TramiteRepository>();

        return services;
    }
}