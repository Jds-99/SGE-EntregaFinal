using System;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using SGE.Dominio.Comun;
using SGE.Dominio.Tramites;

namespace SGE.Dominio.Expedientes;

public class Expediente
{
    public Guid Id {get; private set;}
    public CaratulaExpendiente Caratula {get; private set;}
    public DateTime FechaCreacion {get; private set;}
    public DateTime FechaUltimaModificacion {get; private set;}
    public Guid UsuarioUltimoCambio {get; private set;}
    public EstadoExpediente Estado {get; private set;}
    public List<Tramite> Tramites { get; private set; } = new List<Tramite>();
    
    // Constructor para usuarios nuevos
    public Expediente(CaratulaExpendiente caratula, Guid usuarioUltCambio)
        : this(Guid.NewGuid(), caratula, usuarioUltCambio, DateTime.Now, DateTime.Now, EstadoExpediente.RecienIniciado)
    {
        
    }

    // Reconstrucción desde la capa de Infraestructura
    public static Expediente FactoryMethod(Guid id, CaratulaExpendiente caratula, Guid usuarioUltCambio, DateTime fechaCreacion, DateTime fechaUltModif, EstadoExpediente estado)
    {
        // Directamente invoca al constructor privado (el cual se va a encargar de validar las invariantes)
        return new Expediente(id, caratula, usuarioUltCambio, fechaCreacion, fechaUltModif, estado);
    }

    // Constructor privado, valida y asigna
    private Expediente(Guid id, CaratulaExpendiente caratula, Guid usuarioUltCambio, DateTime fechaCreacion, DateTime fechaUltModif, EstadoExpediente estado)
    {
        // Centralización de validaciones de Invariantes
        if (id == Guid.Empty) 
            throw new DominioException("El id del expediente no puede estar vacío.");
            
        if (usuarioUltCambio == Guid.Empty) 
            throw new DominioException("El usuario responsable no puede estar vacío.");
            
        if (caratula == null) 
            throw new DominioException("La carátula del expediente no puede ser nula o estar vacía.");
            
        if (fechaCreacion > fechaUltModif)
            throw new DominioException("La fecha de última modificación no puede ser menor a la de creación.");

        // Asignación efectiva de las propiedades internas
        this.Id = id;
        this.Caratula = caratula;
        this.UsuarioUltimoCambio = usuarioUltCambio;
        this.FechaCreacion = fechaCreacion;
        this.FechaUltimaModificacion = fechaUltModif;
        this.Estado = estado;
    }


    public void ModificarCaratula(CaratulaExpendiente nuevaCaratula, Guid idUsuario)
    {
        if (idUsuario== Guid.Empty)
            throw new DominioException("El ID es incorrecto"); 
        else // El ID es valido entonces cambio la  caratula.
        {
            this.Caratula=nuevaCaratula;
            this.UsuarioUltimoCambio=idUsuario;
            this.FechaUltimaModificacion=DateTime.Now;
        }
    }

    public bool ActualizarEstadoAutomatico(EtiquetaTramite? etiqueta, Guid idUsuario)
    {
    EstadoExpediente estadoAnterior = this.Estado;

    if (idUsuario == Guid.Empty)
        throw new DominioException("Usuario invalido");

    // 🌟 Si es null o es el escrito inicial, no cambia el estado
    if (etiqueta == null || etiqueta == EtiquetaTramite.EscritoPresentado) 
        return false;

    // 🌟 Comparamos ENUM contra ENUM de forma directa y segura
    if (etiqueta == EtiquetaTramite.PaseAEstudio)
        this.Estado = EstadoExpediente.ParaResolver;
        
    else if (etiqueta == EtiquetaTramite.Resolucion) // 🌟 Cambié a Resolucion que es lo que manda tu Program.cs
        this.Estado = EstadoExpediente.ConResolucion;
        
    else if (etiqueta == EtiquetaTramite.PaseAlArchivo)
        this.Estado = EstadoExpediente.Finalizado;

    // Si el estado actual es distinto al que tenía al principio, mutó.
    if (this.Estado != estadoAnterior)
    {
        this.UsuarioUltimoCambio = idUsuario;
        this.FechaUltimaModificacion = DateTime.Now;
        return true; // 🌟 Le avisa al servicio que guarde los cambios en el archivo
    }

    return false;
}

    public void CambiarEstado(EstadoExpediente estado, Guid id)
    {
        if (id == Guid.Empty) throw new DominioException("id invalido");
        this.Estado=estado;        
    }

    private Expediente()
    {
    // EF Core va a usar este constructor "en blanco" y luego inyectará los datos reales 
    // directamente en las propiedades usando Reflection.
    Caratula = null!; 
    Estado = default; // El enum de estado por defecto
    }
}
