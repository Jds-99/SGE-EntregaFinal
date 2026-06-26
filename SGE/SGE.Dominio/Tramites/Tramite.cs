using System;
using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite 
{
    public Guid Id { get; private set; }
    public Guid IdExpediente { get; private set; }
    public EtiquetaTramite Etiqueta { get; private set; }
    public Contenido contenido { get; private set; } 
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUlimoCambio { get; private set; }

    //Creacion por primera vez
    public Tramite(Guid idExpediente, Guid usuarioUltimoCambio, Contenido contenido)
        : this(Guid.NewGuid(), idExpediente, usuarioUltimoCambio, contenido, DateTime.Now, DateTime.Now, EtiquetaTramite.EscritoPresentado)
    { 
    }
     //Constructor Privado
    private Tramite(Guid id, Guid idExpediente, Guid usuarioUltimoCambio, Contenido contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion, EtiquetaTramite etiqueta)
    {
        // Todas las validaciones de las invariantes centralizadas acá [cite: 57, 80, 82]
        if (id == Guid.Empty)
            throw new DominioException("El ID del trámite no puede estar vacío."); 
        if (idExpediente == Guid.Empty)
            throw new DominioException("El ID del expediente no puede ser nulo o estar vacío."); 
        if (usuarioUltimoCambio == Guid.Empty)
            throw new DominioException("El usuario no puede estar vacío.");
        if (contenido == null)
            throw new DominioException("El contenido del trámite no puede ser nulo.");
        if (fechaCreacion > fechaUltimaModificacion)
            throw new DominioException("La fecha de creación no puede ser mayor a la de última modificación."); 

        // Asignación efectiva
        this.Id = id;
        this.IdExpediente = idExpediente;
        this.UsuarioUlimoCambio = usuarioUltimoCambio;
        this.contenido = contenido;
        this.FechaCreacion = fechaCreacion;
        this.FechaUltimaModificacion = fechaUltimaModificacion;
        this.Etiqueta = etiqueta;
    }
    
    public static Tramite FactoryMethodTramite(Guid id, Guid idExpediente, Guid usuarioUltimoCambio, Contenido contenido, DateTime fechaCreacion, DateTime fechaUltimaModificacion, EtiquetaTramite etiqueta)
    {
        return new Tramite(id, idExpediente, usuarioUltimoCambio, contenido, fechaCreacion, fechaUltimaModificacion, etiqueta);    
    }
   


    // =========================================================================
    // NUEVO MÉTODO DE DOMINIO: DELEGACIÓN DE MODIFICACIÓN
    // =========================================================================
    public void Modificar(Contenido nuevoContenido, EtiquetaTramite nuevaEtiqueta, Guid usuarioModificador)
    {
        if (usuarioModificador == Guid.Empty)
        {
            throw new DominioException("El usuario que realiza la modificación no puede estar vacío.");
        }

        if (nuevoContenido == null)
        {
            throw new DominioException("El nuevo contenido del trámite no puede ser nulo.");
        }

        // El estado se modifica internamente protegiendo las reglas de negocio
        this.contenido = nuevoContenido;
        this.Etiqueta = nuevaEtiqueta;
        this.UsuarioUlimoCambio = usuarioModificador;
        this.FechaUltimaModificacion = DateTime.Now;
    }
    private Tramite()
    {
        contenido = null!;
    }
}