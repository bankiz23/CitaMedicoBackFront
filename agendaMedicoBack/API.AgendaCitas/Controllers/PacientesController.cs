using API.Services.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.AgendaCitas.Controllers
{
    [ApiController]
    [Route("api/pacientes")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;
        private readonly ICitaService _citaService;

        public PacientesController(IPacienteService pacienteService, ICitaService citaService)
        {
            _pacienteService = pacienteService;
            _citaService = citaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _pacienteService.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var paciente = await _pacienteService.GetByIdAsync(id);
            return paciente == null ? NotFound(new { mensaje = "Paciente no encontrado." }) : Ok(paciente);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PacienteCreateDto dto)
        {
            var paciente = await _pacienteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, paciente);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PacienteCreateDto dto)
        {
            await _pacienteService.UpdateAsync(id, dto);
            return Ok(new { mensaje = "Paciente actualizado correctamente." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _pacienteService.DeleteAsync(id);
            return Ok(new { mensaje = "Paciente eliminado correctamente." });
        }

        [HttpGet("{id:int}/historial")]
        public async Task<IActionResult> Historial(int id)
        {
            var historial = await _citaService.ObtenerHistorialPacienteAsync(id);
            return Ok(historial);
        }
    }
}
