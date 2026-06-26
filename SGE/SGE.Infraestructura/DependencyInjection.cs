using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SGE.Aplicacion; 
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Token;
using SGE.Infraestructura.Repository;
namespace SGE.Infraestructura;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
       // A. Base de Datos. Extraemos la cadena de conexión del archivo appsettings.json
        var connectionString = configuration.GetConnectionString("SGEDb");
        services.AddDbContext<SgeContext>(opciones => opciones.UseSqlite(connectionString));
        // B y C. Infraestructura y Seguridad (Scoped)
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        services.AddScoped<ITramiteRepository, TramiteTxtRepository>();
        services.AddScoped<IExpedienteRepository, ExpedienteTxtRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();


        services.AddScoped<IAutorizacionService, AutorizacionService>(); // aca probablemente se cambio algo de lugar
        return services;
    }
}