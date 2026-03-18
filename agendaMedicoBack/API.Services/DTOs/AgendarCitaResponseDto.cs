using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.DTOs
{
    public class AgendarCitaResponseDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = null!;
        public int? CitaId { get; set; }
        public bool AlertaCancelaciones { get; set; }
        public List<DateTime> Sugerencias { get; set; } = new();
    }
}
