using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class Medico
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public int Fk_IdEspecialidad { get; set; }
        public CatalogoEspecialidad Especialidad { get; set; } = null!;
        public bool Activo { get; set; } = true;

        public ICollection<HorarioMedico> Horarios { get; set; } = new List<HorarioMedico>();
        public ICollection<Cita> Citas { get; set; } = new List<Cita>();
    }
}
