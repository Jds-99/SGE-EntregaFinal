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
    //Constructor para usuarios nuevos.
    public Expediente(CaratulaExpendiente Caratula, Guid usuarioUltCambio)
    {
        this.Id = Guid.NewGuid();
        if (usuarioUltCambio == Guid.Empty) 
            throw new DominioExcepcion("El usuario no puede estar vacio.");
        this.UsuarioUltimoCambio = usuarioUltCambio;
        this.FechaCreacion = DateTime.Now;
        this.Estado = EstadoExpediente.RecienIniciado;
        this.FechaUltimaModificacion = DateTime.Now;
        this.Caratula = Caratula ?? throw new DominioExcepcion("La caratula no puede estar vacia."); 
    }
    //Constructor que usa el factory method para re-crear un expediente
    private Expediente(Guid id, CaratulaExpendiente caratula, 
                    Guid usuarioUltCambio, DateTime fechaCreacion, 
                    DateTime fechaUltModif,EstadoExpediente estado)
    {
        this.Caratula = caratula;
        this.FechaCreacion = fechaCreacion;
        this.Estado = estado;
        this.FechaUltimaModificacion = fechaUltModif;
        this.Id = id;
        this.UsuarioUltimoCambio = usuarioUltCambio;
    }
    public static Expediente FactoryMethod(Guid id, CaratulaExpendiente caratula, Guid usuarioUltCambio, 
                      DateTime fechaCreacion, DateTime fechaUltModif, EstadoExpediente estado){
        if(id == Guid.Empty) 
            throw new DominioExcepcion("El id no puede estar vacio.");
        if(fechaCreacion > fechaUltModif)
            throw new DominioExcepcion("La fecha de modificacion es menor a la de creacion.");
        if(usuarioUltCambio == Guid.Empty)
            throw new DominioExcepcion("El id del usuario no puede estar vacio.");
        return new Expediente(id,caratula,usuarioUltCambio,fechaCreacion,fechaUltModif,estado);
    }

    public void ModificarCaratula(CaratulaExpendiente nuevaCaratula, Guid idUsuario)
    {
        if (idUsuario== Guid.Empty)
            throw new DominioExcepcion("El ID es incorrecto"); 
        else // El ID es valido entonces cambio la  caratula.
        {
            this.Caratula=nuevaCaratula;
            this.UsuarioUltimoCambio=idUsuario;
            this.FechaUltimaModificacion=DateTime.Now;
        }
    }

    public bool ActualizarEstadoAutomatico (EtiquetaTramite? etiqueta, Guid idUsuario)
    {
        EstadoExpediente estadoActual = Estado;
        if (idUsuario == Guid.Empty)
            throw new DominioExcepcion("Usuario invalido");
        if (etiqueta.Equals("Escritoresentado")) 
            return false;
        if (etiqueta.Equals("PaseAEstudio"))
            this.Estado = EstadoExpediente.ParaResolver;
        if (etiqueta.Equals("PaseAlAchivo"))
            this.Estado = EstadoExpediente.ConResolucion;
        if (etiqueta.Equals("Finalizado"))
            this.Estado = EstadoExpediente.RecienIniciado;
        return (estadoActual!=this.Estado);
    }

    public void CambiarEstado(EstadoExpediente estado, Guid id)
    {
        if (id == Guid.Empty) throw new DominioExcepcion("id invalido");
        this.Estado=estado;        
    }
}
