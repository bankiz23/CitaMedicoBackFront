using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class HorarioMedico
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public Medico Medico { get; set; } = null!;
    }
}
