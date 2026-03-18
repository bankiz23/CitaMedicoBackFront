using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.DTOs
{
    public class MedicoCreateDto
    {
        public string Nombre { get; set; } = null!;
        public int Fk_IdEspecialidad { get; set; }
    }
}
