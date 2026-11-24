using Contratos.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiWeb.Interfaces;

namespace WebApiWeb.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _auth;

        public AuthController(IAuthInterface auth)
        {
            _auth = auth;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var respuesta = _auth.Login(dto);
            if (respuesta is null)
            {
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos." });
            }

            return Ok(respuesta);
        }

        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] LoginDto dto)
        {
            try
            {
                _auth.Registrar(dto);
                return Ok(new { mensaje = "Usuario registrado correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
