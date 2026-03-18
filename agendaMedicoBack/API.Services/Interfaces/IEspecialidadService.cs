using API.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IEspecialidadService
    {
        Task<List<EspecialidadDto>> GetAllAsync();
    }
}
