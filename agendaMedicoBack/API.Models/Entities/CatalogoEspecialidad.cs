using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Models.Entities
{
    public class CatalogoEspecialidad
    {
        public int Id { get; set; }
        public string NombreEspecialidad { get; set; } = null!;
        public bool Activo { get; set; } = true;

        public ICollection<Medico> Medicos { get; set; } = new List<Medico>();
        public ICollection<DuracionEspecialidad> Duraciones { get; set; } = new List<DuracionEspecialidad>();
    }
}
