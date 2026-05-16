using System;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion;

public interface ITramiteRepository
{
    void  Agregar(Tramite tramite);
    Tramite?  ObtenerPorId(Guid id);
    void Modificar(Guid id);
    void Eliminar(Guid id);
    Tramite? ObetenerPorExedienteId(Guid id); // preguntar que tiene que retornar
}
