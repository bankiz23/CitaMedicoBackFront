using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.DTO
{
    public class CitaResponseDto
    {
        /// <summary>
        /// public CitaResponseDto();
        /// </summary>

        public int Id { get; set; }
        public int MedicoId { get; set; }
        public string Medico { get; set; }
        public int PacienteId { get; set; }
        public string Paciente { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; }
    }
}
