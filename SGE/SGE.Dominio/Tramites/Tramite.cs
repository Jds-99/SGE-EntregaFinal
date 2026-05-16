using System;
using SGE.Dominio.Comun;

namespace SGE.Dominio.Tramites;

public class Tramite 
{
    public Guid Id {get; private set;}
    public Guid IdExpediente {get; private set;}
    public EtiquetaTramite Etiqueta {get; private set;}
    public Contenido contenido {get; private set;}
    public DateTime FechaCreacion {get; private set;}
    public DateTime FechaUltimaModificacion {get; private set;}
    public Guid UsuarioUlimoCambio {get; private set;}

    public Tramite(Guid idExpediente, Guid UsuarioUltimoCambio, Contenido contenido)
    {
        this.Id = Guid.NewGuid();
        if (Id == Guid.Empty)
        {
            throw new DominioExcepcion(" el id del tramite no puede estar vacio");
        }
        if(idExpediente== Guid.Empty)
        {
            throw new DominioExcepcion("el id del expdiente no puede ser nulo");
        }
        this.contenido=contenido;
        this.UsuarioUlimoCambio=UsuarioUlimoCambio;
        this.FechaCreacion=DateTime.Now;
        this.FechaUltimaModificacion=DateTime.Now;
        this.Etiqueta=EtiquetaTramite.EscritoPresentado;//hola me presento
    }
    private Tramite(Guid id,Guid idExpediente,Guid UsuarioUltimoCambio, Contenido contenido, DateTime FechaCreacion, DateTime FechaUltimaModificacion, EtiquetaTramite etiqueta)
    {
        this.Id=id;
        this.IdExpediente=idExpediente;
        this.UsuarioUlimoCambio=UsuarioUlimoCambio;
        this.Etiqueta=etiqueta;
        this.contenido=contenido;
        this.FechaCreacion=FechaCreacion;
        this.FechaUltimaModificacion=FechaUltimaModificacion;
    }
        
    public static Tramite FactoryMethodTramite(Guid id,Guid idExpediente,Guid UsuarioUltimoCambio, Contenido contenido, DateTime FechaCreacion, DateTime FechaUltimaModificacion, EtiquetaTramite etiqueta)
    {
        if( id == Guid.Empty)
            throw new DominioExcepcion("El ID no puede estar vacio");
        if (idExpediente==Guid.Empty)
            throw new DominioExcepcion("El ID del expediente no puede estar vacio");
        if (UsuarioUltimoCambio==Guid.Empty)
            throw new DominioExcepcion("El usuario no puede estar vacio");
        if (FechaCreacion>FechaUltimaModificacion)
            throw new DominioExcepcion("La fecha de creacion es mayor a la del ultimo cambio");
        return new Tramite(id,idExpediente,UsuarioUltimoCambio,contenido,FechaCreacion,FechaUltimaModificacion,etiqueta);    
    }
    }

   

