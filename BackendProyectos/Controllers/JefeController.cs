using BackendProyectos.Interfaces;
using ContratoProyectos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendProyectos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JefeController : ControllerBase
    {
        private readonly IJefe _jefeService;

        public JefeController(IJefe jefeService)
        {
            _jefeService = jefeService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_jefeService.ObtenerTodos());
        }

        [HttpGet("disponibles")]
        public IActionResult GetDisponibles()
        {
            return Ok(_jefeService.ObtenerDisponibles());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var jefe = _jefeService.ObtenerPorId(id);
            if (jefe == null)
            {
                return NotFound("No se encontró el Jefe con ese ID.");
            }
            return Ok(jefe);
        }

        [HttpPost]
        public IActionResult Post(JefeDTOs dto)
        {
            try
            {
                _jefeService.Crear(dto);
                return Ok("Jefe registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
