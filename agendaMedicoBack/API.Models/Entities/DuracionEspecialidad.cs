using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class DuracionEspecialidad
    {
        public int Id { get; set; }
        public int Fk_IdEspecialidad { get; set; }
        public int DuracionMinutos { get; set; }
        public bool Activo { get; set; } = true;

        public CatalogoEspecialidad Especialidad { get; set; } = null!;
    }
}
