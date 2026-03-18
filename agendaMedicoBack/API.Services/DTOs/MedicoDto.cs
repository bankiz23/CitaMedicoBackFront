using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.DTOs
{
    public class MedicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int Fk_IdEspecialidad { get; set; }
        public string Especialidad { get; set; } = null!;
        public bool Activo { get; set; }
    }

    public class HorarioMedicoDto
    {
        public int Id { get; set; }
        public int MedicoId { get; set; }
        public DayOfWeek DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
    }
}
