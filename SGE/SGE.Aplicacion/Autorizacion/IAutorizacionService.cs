using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;

public interface IAutorizacionService{
    bool PoseeElPermiso(Guid idUsuario,string permiso);
}