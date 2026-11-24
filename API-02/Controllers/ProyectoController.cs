using Contratos.DTOs;
using Contratos.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contratos.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProyectoController : ControllerBase
    {
        private readonly IProyectoInterface _proyectoService;

        public ProyectoController(IProyectoInterface proyectoService)
        {
            _proyectoService = proyectoService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProyectoDto>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(_proyectoService.ObtenerTodos());
        }

        // Buscar un proyecto por id, incluyendo los datos de su jefe
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProyectoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(Guid id)
        {
            var proyecto = _proyectoService.ObtenerPorId(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return Ok(proyecto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProyectoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Post(CrearProyectoDto dto)
        {
            try
            {
                var proyecto = _proyectoService.Crear(dto);
                return Ok(proyecto);
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
