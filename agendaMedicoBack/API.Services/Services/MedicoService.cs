
using API.Models.Entities;
using API.Services.DTOs;
using API.Services.Exceptions;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Services
{

    public class MedicoService : IMedicoService
    {
        private readonly AppDbContext _context;

        public MedicoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MedicoDto>> GetAllAsync()
        {
            return await _context.Medicos
                .Include(x => x.Especialidad)
                .Where(x => x.Activo)
                .Select(x => new MedicoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Fk_IdEspecialidad = x.Fk_IdEspecialidad,
                    Especialidad = x.Especialidad.NombreEspecialidad,
                    Activo = x.Activo
                })
                .ToListAsync();
        }
        public async Task<MedicoDto?> GetByIdAsync(int id)
        {
            return await _context.Medicos
                .Where(x => x.Id == id && x.Activo)
                .Select(x => new MedicoDto
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    Especialidad = x.Especialidad.NombreEspecialidad,
                    Activo = x.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MedicoDto> CreateAsync(MedicoCreateDto dto)
        {
            var especialidadExiste = await _context.CatalogoEspecialidades
                .AnyAsync(x => x.Id == dto.Fk_IdEspecialidad && x.Activo);

            if (!especialidadExiste)
                throw new NotFoundException("La especialidad no existe.");

            var medico = new Medico
            {
                Nombre = dto.Nombre,
                Fk_IdEspecialidad = dto.Fk_IdEspecialidad,
                Activo = true
            };

            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();

            medico = await _context.Medicos
                .Include(x => x.Especialidad)
                .FirstAsync(x => x.Id == medico.Id);

            return new MedicoDto
            {
                Id = medico.Id,
                Nombre = medico.Nombre,
                Fk_IdEspecialidad = medico.Fk_IdEspecialidad,
                Especialidad = medico.Especialidad.NombreEspecialidad,
                Activo = medico.Activo
            };
        }
        public async Task UpdateAsync(int id, MedicoCreateDto dto)
        {
            var medico = await _context.Medicos.FirstOrDefaultAsync(x => x.Id == id && x.Activo);

            if (medico == null)
                throw new NotFoundException("Médico no encontrado.");

            var especialidadExiste = await _context.CatalogoEspecialidades
                .AnyAsync(x => x.Id == dto.Fk_IdEspecialidad && x.Activo);

            if (!especialidadExiste)
                throw new NotFoundException("La especialidad no existe.");

            medico.Nombre = dto.Nombre;
            medico.Fk_IdEspecialidad = dto.Fk_IdEspecialidad;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var medico = await _context.Medicos.FirstOrDefaultAsync(x => x.Id == id && x.Activo);

            if (medico == null)
                throw new NotFoundException("Médico no encontrado.");

            medico.Activo = false;
            await _context.SaveChangesAsync();
        }

        public async Task<List<HorarioMedicoDto>> GetHorariosAsync(int medicoId)
        {
            bool existeMedico = await _context.Medicos.AnyAsync(x => x.Id == medicoId && x.Activo);
            if (!existeMedico)
                throw new NotFoundException("Médico no encontrado.");

            return await _context.HorariosMedico
                .Where(x => x.MedicoId == medicoId)
                .OrderBy(x => x.DiaSemana)
                .ThenBy(x => x.HoraInicio)
                .Select(x => new HorarioMedicoDto
                {
                    Id = x.Id,
                    MedicoId = x.MedicoId,
                    DiaSemana = x.DiaSemana,
                    HoraInicio = x.HoraInicio,
                    HoraFin = x.HoraFin
                })
                .ToListAsync();
        }

        public async Task<HorarioMedicoDto> AddHorarioAsync(int medicoId, HorarioMedicoCreateDto dto)
        {
            bool existeMedico = await _context.Medicos.AnyAsync(x => x.Id == medicoId && x.Activo);
            if (!existeMedico)
                throw new NotFoundException("Médico no encontrado.");

            if (dto.HoraInicio >= dto.HoraFin)
                throw new BadRequestException("La hora de inicio debe ser menor a la hora fin.");

            bool traslape = await _context.HorariosMedico.AnyAsync(x =>
                x.MedicoId == medicoId &&
                x.DiaSemana == dto.DiaSemana &&
                dto.HoraInicio < x.HoraFin &&
                dto.HoraFin > x.HoraInicio);

            if (traslape)
                throw new ConflictException("El horario se traslapa con otro horario existente.");

            var horario = new HorarioMedico
            {
                MedicoId = medicoId,
                DiaSemana = dto.DiaSemana,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin
            };

            _context.HorariosMedico.Add(horario);
            await _context.SaveChangesAsync();

            return new HorarioMedicoDto
            {
                Id = horario.Id,
                MedicoId = horario.MedicoId,
                DiaSemana = horario.DiaSemana,
                HoraInicio = horario.HoraInicio,
                HoraFin = horario.HoraFin
            };
        }

        public async Task UpdateHorarioAsync(int medicoId, int horarioId, HorarioMedicoCreateDto dto)
        {
            var horario = await _context.HorariosMedico
                .FirstOrDefaultAsync(x => x.Id == horarioId && x.MedicoId == medicoId);

            if (horario == null)
                throw new NotFoundException("Horario no encontrado.");

            if (dto.HoraInicio >= dto.HoraFin)
                throw new BadRequestException("La hora de inicio debe ser menor a la hora fin.");

            bool traslape = await _context.HorariosMedico.AnyAsync(x =>
                x.Id != horarioId &&
                x.MedicoId == medicoId &&
                x.DiaSemana == dto.DiaSemana &&
                dto.HoraInicio < x.HoraFin &&
                dto.HoraFin > x.HoraInicio);

            if (traslape)
                throw new ConflictException("El horario se traslapa con otro horario existente.");

            horario.DiaSemana = dto.DiaSemana;
            horario.HoraInicio = dto.HoraInicio;
            horario.HoraFin = dto.HoraFin;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteHorarioAsync(int medicoId, int horarioId)
        {
            var horario = await _context.HorariosMedico
                .FirstOrDefaultAsync(x => x.Id == horarioId && x.MedicoId == medicoId);

            if (horario == null)
                throw new NotFoundException("Horario no encontrado.");

            _context.HorariosMedico.Remove(horario);
            await _context.SaveChangesAsync();
        }
    }
}
