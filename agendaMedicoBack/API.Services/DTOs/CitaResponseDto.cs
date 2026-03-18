using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.DTOs
{
    /*public class CitaResponseDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string Medico { get; set; } = null!;
        public int PacienteId { get; set; }
        public string Paciente { get; set; } = null!;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Motivo { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }*/
    public class CitaResponseDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string Medico { get; set; } = null!;
        public int PacienteId { get; set; }
        public string Paciente { get; set; } = null!;
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Motivo { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }
}
