namespace SGE.Aplicacion.Tramites;
using System;
using SGE.Dominio.Tramites;

public interface ITramiteRepository
{
    void  Agregar(Tramite tramite);
    void Modificar(Tramite tramite);
    void Eliminar(Guid id);
    IEnumerable<Tramite> ObtenerPorExpedienteId(Guid id);

    public IEnumerable<Tramite> ObtenerTodos();
}
