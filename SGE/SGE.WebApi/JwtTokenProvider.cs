using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SGE.Aplicacion.Token;
using SGE.Dominio.Usuarios;
namespace SGE.WebApi;
public class JwtTokenProvider(IConfiguration config) : ITokenService
{
public string GenerarToken(Usuario usuario)
{
// Guardamos únicamente el ID del usuario para identificarlo
var claims = new[] {
new Claim("ID", usuario.Id.ToString()),
};
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);// Es similar a un DTO, pero hace
// algunas validaciones
var token = new JwtSecurityToken(
issuer: config["Jwt:Issuer"],
audience: config["Jwt:Audience"],
claims: claims,
expires: DateTime.UtcNow.AddHours(2),
signingCredentials: creds
);
return new JwtSecurityTokenHandler().WriteToken(token);
}
}