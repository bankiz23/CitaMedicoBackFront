using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class Cita
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public int PacienteId { get; set; }

        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }

        public string Motivo { get; set; } = null!;
        public EstadoCita Estado { get; set; } = EstadoCita.Agendada;
        public string? MotivoCancelacion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public Medico Medico { get; set; } = null!;
        public Paciente Paciente { get; set; } = null!;
    }
}
