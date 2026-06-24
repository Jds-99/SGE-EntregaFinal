using System;

namespace SGE.Aplicacion;

public class EliminarUsuarioRequest
{
    public Guid OperadorId { get; set; }
    public Guid UsuarioAEliminar {get;set;}
}