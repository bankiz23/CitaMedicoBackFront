using API.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface ICitaService
    {
        Task<AgendarCitaResponseDto> AgendarAsync(AgendarCitaDto dto);
        Task CancelarAsync(int citaId, CancelarCitaDto dto);
        Task<CitaResponseDto?> ObtenerPorIdAsync(int citaId);
        Task<List<API.Models.DTO.CitaResponseDto>> ObtenerAgendaDiaAsync(int medicoId, DateOnly fecha);
        Task<List<CitaResponseDto>> ObtenerHistorialPacienteAsync(int pacienteId);
        Task<List<DateTime>> ObtenerProximos5HorariosDisponiblesAsync(int medicoId, DateOnly fecha);
    }
}
