using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;

namespace SGE.Aplicacion;

public static class DependencyInjection
{
    public static IServiceCollection AddAplicacion(this IServiceCollection services)
    {
        // Registramos los Casos de Uso. 
        // Se usa Transient porque se crean, ejecutan su acción y se destruyen.
        services.AddScoped<AgregarExpedienteUseCase>();
        services.AddScoped<CambiarEstadoExpedienteUseCase>();
        services.AddScoped<EliminarExpedienteUseCase>();
        services.AddScoped<ModificarCaratulaUseCase>();
        services.AddScoped<ObtenerPorIdUseCase>();
        services.AddScoped<ObtenerTodosExpedientesUseCase>();
        services.AddTransient<ActualizacionEstadoExpedienteService>();

        services.AddScoped<AgregarTramiteUseCase>();
        services.AddScoped<EliminarTramiteUseCase>();
        services.AddScoped<ModificarTramiteUseCase>();
        services.AddScoped<ObtenerTramitePorIdUseCase>();
        services.AddScoped<ObtenerTramitesPorExpedienteIdUseCase>();

        services.AddScoped<RegistrarUsuarioUseCase>();
        services.AddScoped<IPasswordHasher, CryptographyPasswordHasher>();
        // El día de mañana vas a ir agregando los otros acá abajo:
        // services.AddTransient<RegistrarUsuarioUseCase>();
        // services.AddTransient<AltaTramiteUseCase>();

        return services;
    }
}