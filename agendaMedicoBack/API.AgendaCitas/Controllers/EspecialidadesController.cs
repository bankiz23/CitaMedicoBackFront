using API.Services.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.AgendaCitas.Controllers
{
    [ApiController]
    [Route("api/especialidades")]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IEspecialidadService _especialidadService;

        public EspecialidadesController(IEspecialidadService especialidadService)
        {
            _especialidadService = especialidadService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _especialidadService.GetAllAsync());
        }
    }
}