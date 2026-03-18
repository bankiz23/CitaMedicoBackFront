using API.Services.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.AgendaCitas.Controllers
{
    [ApiController]
    [Route("api/medicos")]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicosController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _medicoService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medico = await _medicoService.GetByIdAsync(id);
            if (medico == null)
                return NotFound(new { mensaje = "Médico no encontrado." });

            return Ok(medico);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MedicoCreateDto dto)
        {
            var medico = await _medicoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = medico.Id }, medico);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicoCreateDto dto)
        {
            await _medicoService.UpdateAsync(id, dto);
            return Ok(new { mensaje = "Médico actualizado correctamente." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _medicoService.DeleteAsync(id);
            return Ok(new { mensaje = "Médico eliminado correctamente." });
        }

        [HttpGet("{medicoId:int}/horarios")]
        public async Task<IActionResult> GetHorarios(int medicoId)
        {
            return Ok(await _medicoService.GetHorariosAsync(medicoId));
        }

        [HttpPost("{medicoId:int}/horarios")]
        public async Task<IActionResult> AddHorario(int medicoId, [FromBody] HorarioMedicoCreateDto dto)
        {
            var horario = await _medicoService.AddHorarioAsync(medicoId, dto);
            return Ok(horario);
        }

        [HttpPut("{medicoId:int}/horarios/{horarioId:int}")]
        public async Task<IActionResult> UpdateHorario(int medicoId, int horarioId, [FromBody] HorarioMedicoCreateDto dto)
        {
            await _medicoService.UpdateHorarioAsync(medicoId, horarioId, dto);
            return Ok(new { mensaje = "Horario actualizado correctamente." });
        }

        [HttpDelete("{medicoId:int}/horarios/{horarioId:int}")]
        public async Task<IActionResult> DeleteHorario(int medicoId, int horarioId)
        {
            await _medicoService.DeleteHorarioAsync(medicoId, horarioId);
            return Ok(new { mensaje = "Horario eliminado correctamente." });
        }
    }
}
