using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.AgendaCitas.Controllers
{
    [ApiController]
    [Route("api")]
    public class AgendaController : ControllerBase
    {
        private readonly ICitaService _citaService;

        public AgendaController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet("agenda")]
        public async Task<IActionResult> GetAgendaDia([FromQuery] int medicoId, [FromQuery] DateOnly fecha)
        {
            var agenda = await _citaService.ObtenerAgendaDiaAsync(medicoId, fecha);
            return Ok(agenda);
        }

        [HttpGet("disponibilidad")]
        public async Task<IActionResult> GetDisponibilidad([FromQuery] int medicoId, [FromQuery] DateOnly fecha)
        {
            var horarios = await _citaService.ObtenerProximos5HorariosDisponiblesAsync(medicoId, fecha);
            return Ok(horarios);
        }
    }
}
