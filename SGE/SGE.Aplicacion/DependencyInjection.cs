using Microsoft.Extensions.DependencyInjection;
using SGE.Aplicacion.Expedientes;

namespace SGE.Aplicacion;

public static class DependencyInjection
{
    public static IServiceCollection AddAplicacion(this IServiceCollection services)
    {
        // Registramos los Casos de Uso. 
        // Se usa Transient porque se crean, ejecutan su acción y se destruyen.
        services.AddTransient<AgregarExpedienteUseCase>();
        
        // El día de mañana vas a ir agregando los otros acá abajo:
        // services.AddTransient<RegistrarUsuarioUseCase>();
        // services.AddTransient<AltaTramiteUseCase>();

        return services;
    }
}