using API.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IPacienteService
    {
        Task<List<PacienteDto>> GetAllAsync();
        Task<PacienteDto?> GetByIdAsync(int id);
        Task<PacienteDto> CreateAsync(PacienteCreateDto dto);
        Task UpdateAsync(int id, PacienteCreateDto dto);
        Task DeleteAsync(int id);
    }
}
