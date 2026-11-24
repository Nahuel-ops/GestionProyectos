using BackendProyectos.Interfaces;
using ContratoProyectos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendProyectos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProyectoController : ControllerBase
    {
        private readonly IProyecto _proyectoService;

        public ProyectoController(IProyecto proyectoService)
        {
            _proyectoService = proyectoService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_proyectoService.ObtenerTodos());
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var proyecto = _proyectoService.ObtenerPorId(id);
            if (proyecto == null)
            {
                return NotFound("No se encontró ningún Proyecto con ese ID.");
            }
            return Ok(proyecto);
        }

        [HttpPost]
        public IActionResult Post(ProyectoDTOs dto)
        {
            try
            {
                _proyectoService.Crear(dto);
                return Ok("Proyecto registrado correctamente.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
