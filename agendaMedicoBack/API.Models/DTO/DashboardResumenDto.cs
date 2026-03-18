using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class DashboardResumenDto
    {
        public int TotalMedicos { get; set; }
        public int TotalPacientes { get; set; }
        public int TotalCitasHoy { get; set; }
        public int TotalCitasCanceladas { get; set; }
    }
}
