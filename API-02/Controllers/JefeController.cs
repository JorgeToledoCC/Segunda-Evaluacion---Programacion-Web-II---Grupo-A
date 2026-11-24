using Contratos.DTOs;
using Contratos.Interfaces;
using Contratos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contratos.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JefeController : ControllerBase
    {
        private readonly IJefeInterface _jefeService;

        public JefeController(IJefeInterface jefeService)
        {
            _jefeService = jefeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Jefe>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(_jefeService.ObtenerTodos());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Jefe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            var jefe = _jefeService.ObtenerPorId(id);
            if (jefe == null)
            {
                return NotFound();
            }

            return Ok(jefe);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Jefe), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Post(CrearJefeDto dto)
        {
            try
            {
                var jefe = _jefeService.Crear(dto);
                return Ok(jefe);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }
    }
}
