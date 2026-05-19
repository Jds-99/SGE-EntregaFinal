using System;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion;

public interface ITramiteRepository
{
    void  Agregar(Tramite tramite);
    void Modificar(Tramite tramite);
    void Eliminar(Guid id);
    IEnumerable<Tramite> ObtenerPorExpedienteId(Guid id);
}
