using System;
using SGE.Dominio.Tramites;

namespace SGE.Aplicacion;

public interface ITramiteRepository
{
 void  Agregar();
 Tramite?  ObtenerPorId();
 void Modificar();
 void Eliminar();
 Tramite? ObetenerPorExedienteId(); // preguntar que tiene que retornar


}
