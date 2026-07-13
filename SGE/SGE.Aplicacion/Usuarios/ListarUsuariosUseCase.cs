using System;
using System.Collections.Generic;
using System.Linq;
using SGE.Aplicacion;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion;

public record ListarUsuariosRequest(Guid OperadorId);

public record UsuarioResponseDto(
    Guid Id, 
    string Nombre, 
    string CorreoElectronico, 
    bool EsAdministrador,
    IReadOnlyCollection<Permiso> Permisos
);

public record ListarUsuariosResponse(List<UsuarioResponseDto> Usuarios);

public class ListarUsuariosUseCase
{
    private readonly IUsuarioRepository _repository;

    public ListarUsuariosUseCase(IUsuarioRepository repository) => _repository = repository;

    // ✅ Ahora devuelve el objeto de respuesta estructurado
    public ListarUsuariosResponse  Ejecutar(ListarUsuariosRequest request)
    {
        var operador = _repository.ObtenerPorId(request.OperadorId);
        if (operador == null || !operador.EsAdministrador)
            throw new AutorizacionException("Acción denegada: Se requieren privilegios de administrador.");

        var usuarios = _repository.ObtenerTodos();
        
        // Mapeamos las entidades de dominio a DTOs de salida seguros
        var listaDtos = usuarios.Select(u => new UsuarioResponseDto(
            u.Id, 
            u.Nombre, 
            u.CorreoElectronico.Valor, 
            u.EsAdministrador,
            u.Permisos
        )).ToList();

        // ✅ Retornamos el contenedor con la lista adentro
        return new ListarUsuariosResponse(listaDtos);
    }
}