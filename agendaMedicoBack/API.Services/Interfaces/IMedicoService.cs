using API.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IMedicoService
    {
        Task<List<MedicoDto>> GetAllAsync();
        Task<MedicoDto?> GetByIdAsync(int id);
        Task<MedicoDto> CreateAsync(MedicoCreateDto dto);
        Task UpdateAsync(int id, MedicoCreateDto dto);
        Task DeleteAsync(int id);

        Task<List<HorarioMedicoDto>> GetHorariosAsync(int medicoId);
        Task<HorarioMedicoDto> AddHorarioAsync(int medicoId, HorarioMedicoCreateDto dto);
        Task UpdateHorarioAsync(int medicoId, int horarioId, HorarioMedicoCreateDto dto);
        Task DeleteHorarioAsync(int medicoId, int horarioId);
    }
}
