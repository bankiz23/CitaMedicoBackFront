using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.AgendaCitas.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var result = await _dashboardService.ObtenerResumenAsync();
            return Ok(result);
        }
    }
}
