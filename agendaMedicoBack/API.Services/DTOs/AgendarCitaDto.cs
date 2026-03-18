using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.DTOs
{
    public class AgendarCitaDto
    {
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public string Motivo { get; set; } = null!;
    }
}
