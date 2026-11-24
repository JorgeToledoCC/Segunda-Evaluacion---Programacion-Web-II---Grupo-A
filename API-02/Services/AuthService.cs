using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contratos.Data;
using Contratos.DTOs;
using Microsoft.IdentityModel.Tokens;
using WebApiWeb.Interfaces;
using WebApiWeb.Models;

namespace WebApiWeb.Services
{
    public class AuthService : IAuthInterface
    {
        private readonly BaseDatos _context;
        private readonly IConfiguration _configuracion;

        public AuthService(BaseDatos context, IConfiguration configuracion)
        {
            _context = context;
            _configuracion = configuracion;
        }

        public void Registrar(LoginDto dto)
        {
            var nombre = (dto.NombreUsuario ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Usuario y contraseña son obligatorios.");
            }

            if (_context.Usuarios.Any(u => u.NombreUsuario == nombre))
            {
                throw new InvalidOperationException("El usuario ya existe.");
            }

            _context.Usuarios.Add(new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = nombre,
                Rol = "Usuario",
                PasswordHash = Seguridad.Hashear(dto.Password)
            });
            _context.SaveChanges();
        }

        public LoginRespuestaDTO? Login(LoginDto dto)
        {
            var nombre = (dto.NombreUsuario ?? string.Empty).Trim();
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombre);

            if (usuario is null || !Seguridad.Verificar(dto.Password ?? string.Empty, usuario.PasswordHash))
            {
                return null;
            }

            var expira = DateTime.UtcNow.AddMinutes(MinutosExpiracion());

            return new LoginRespuestaDTO
            {
                Token = CrearToken(usuario, expira),
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Expira = expira
            };
        }

        private string CrearToken(Usuario usuario, DateTime expira)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuracion["Jwt:Key"] ?? "clave-de-desarrollo-proyectos-2026-0123456789"));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NombreUsuario),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuracion["Jwt:Issuer"] ?? "ProyectosApi",
                audience: _configuracion["Jwt:Audience"] ?? "ProyectosWeb",
                claims: claims,
                expires: expira,
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int MinutosExpiracion()
        {
            return int.TryParse(_configuracion["Jwt:ExpiraMinutos"], out var minutos) ? minutos : 60;
        }
    }
}
