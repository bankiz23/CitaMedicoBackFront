using API.Services.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.AgendaCitas.Controllers
{
    [ApiController]
    [Route("api/citas")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cita = await _citaService.ObtenerPorIdAsync(id);
            if (cita == null) return NotFound(new { mensaje = "Cita no encontrada." });
            return Ok(cita);
        }

        [HttpPost("agendar")]
        public async Task<IActionResult> Agendar([FromBody] AgendarCitaDto dto)
        {
            var result = await _citaService.AgendarAsync(dto);

            if (!result.Exito)
                return Conflict(result);

            return Ok(result);
        }

        [HttpPut("{id:int}/cancelar")]
        public async Task<IActionResult> Cancelar(int id, [FromBody] CancelarCitaDto dto)
        {
            await _citaService.CancelarAsync(id, dto);
            return Ok(new { mensaje = "Cita cancelada correctamente." });
        }
    }
}
