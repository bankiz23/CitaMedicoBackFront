using API.Models.DTO;
using API.Models.Entities;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardResumenDto> ObtenerResumenAsync()
        {
            var result = (await _context.Set<DashboardResumenDto>()
     .FromSqlRaw("EXEC sp_ResumenDashboard")
     .AsNoTracking()
     .ToListAsync())
     .FirstOrDefault();

            return result ?? new DashboardResumenDto();
        }
    }
}
