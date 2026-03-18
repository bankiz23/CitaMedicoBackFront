using API.Models.Entities;
using API.Services.DTOs;
using API.Services.Exceptions;
using API.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Services
{
    public class CitaService : ICitaService
    {
        private readonly AppDbContext _context;

        public CitaService(AppDbContext context)
        {
            _context = context;
        }
        /*
        public async Task<AgendarCitaResponseDto> AgendarAsync(AgendarCitaDto dto)
        {
            var medico = await _context.Medicos
                .Include(m => m.Horarios)
                .FirstOrDefaultAsync(m => m.Id == dto.MedicoId && m.Activo);

            if (medico == null)
                throw new NotFoundException("Médico no encontrado.");

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Id == dto.PacienteId && p.Activo);

            if (paciente == null)
                throw new NotFoundException("Paciente no encontrado.");

            if (dto.FechaHoraInicio <= DateTime.Now)
                throw new BadRequestException("No se pueden agendar citas en fechas u horas pasadas.");

            int duracionMin = await ObtenerDuracionPorEspecialidadAsync(medico.Fk_IdEspecialidad);
            DateTime fechaHoraFin = dto.FechaHoraInicio.AddMinutes(duracionMin);

            ValidarHorarioMedico(medico, dto.FechaHoraInicio, fechaHoraFin);

            bool hayConflicto = await _context.Citas.AnyAsync(c =>
                c.MedicoId == dto.MedicoId &&
                c.Estado == EstadoCita.Agendada &&
                dto.FechaHoraInicio < c.FechaHoraFin &&
                fechaHoraFin > c.FechaHoraInicio);

            if (hayConflicto)
            {
                var sugerencias = await ObtenerProximos5HorariosDisponiblesDesdeFechaHoraAsync(dto.MedicoId, dto.FechaHoraInicio);
                return new AgendarCitaResponseDto
                {
                    Exito = false,
                    Mensaje = "El horario solicitado no está disponible.",
                    Sugerencias = sugerencias
                };
            }

            bool alerta = await TieneAlertaCancelacionesAsync(dto.PacienteId);

            var cita = new Cita
            {
                MedicoId = dto.MedicoId,
                PacienteId = dto.PacienteId,
                FechaHoraInicio = dto.FechaHoraInicio,
                FechaHoraFin = fechaHoraFin,
                Motivo = dto.Motivo,
                Estado = EstadoCita.Agendada
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return new AgendarCitaResponseDto
            {
                Exito = true,
                Mensaje = alerta
                    ? "Cita agendada correctamente. El paciente tiene 3 o más cancelaciones en los últimos 30 días."
                    : "Cita agendada correctamente.",
                CitaId = cita.Id,
                AlertaCancelaciones = alerta
            };
        }
        */
        public async Task<AgendarCitaResponseDto> AgendarAsync(AgendarCitaDto dto)
        {
            var medico = await _context.Medicos
                .Include(m => m.Horarios)
                .FirstOrDefaultAsync(m => m.Id == dto.MedicoId && m.Activo);

            if (medico == null)
                throw new NotFoundException("Médico no encontrado.");

            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Id == dto.PacienteId && p.Activo);

            if (paciente == null)
                throw new NotFoundException("Paciente no encontrado.");

            if (dto.FechaHoraInicio <= DateTime.Now)
                throw new BadRequestException("No se pueden agendar citas en fechas u horas pasadas.");

            int duracionMin = await ObtenerDuracionPorEspecialidadAsync(medico.Fk_IdEspecialidad);
            DateTime fechaHoraFin = dto.FechaHoraInicio.AddMinutes(duracionMin);

            ValidarHorarioMedico(medico, dto.FechaHoraInicio, fechaHoraFin);

            bool hayConflicto = await _context.Citas.AnyAsync(c =>
                c.MedicoId == dto.MedicoId &&
                c.Estado == EstadoCita.Agendada &&
                dto.FechaHoraInicio < c.FechaHoraFin &&
                fechaHoraFin > c.FechaHoraInicio);

            if (hayConflicto)
            {
                var sugerencias = await ObtenerProximos5HorariosDisponiblesDesdeFechaHoraAsync(
                    dto.MedicoId,
                    dto.FechaHoraInicio);

                return new AgendarCitaResponseDto
                {
                    Exito = false,
                    Mensaje = "El horario solicitado no está disponible.",
                    Sugerencias = sugerencias,
                    AlertaCancelaciones = false
                };
            }

            bool alerta = await TieneAlertaCancelacionesAsync(dto.PacienteId);

            var cita = new Cita
            {
                MedicoId = dto.MedicoId,
                PacienteId = dto.PacienteId,
                FechaHoraInicio = dto.FechaHoraInicio,
                FechaHoraFin = fechaHoraFin,
                Motivo = dto.Motivo,
                Estado = EstadoCita.Agendada
            };

            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();

            return new AgendarCitaResponseDto
            {
                Exito = true,
                Mensaje = alerta
                    ? "Cita agendada correctamente. El paciente tiene 3 o más cancelaciones en los últimos 30 días."
                    : "Cita agendada correctamente.",
                CitaId = cita.Id,
                AlertaCancelaciones = alerta,
                Sugerencias = new List<DateTime>()
            };
        }
        public async Task CancelarAsync(int citaId, CancelarCitaDto dto)
        {
            var cita = await _context.Citas.FirstOrDefaultAsync(x => x.Id == citaId);

            if (cita == null)
                throw new NotFoundException("Cita no encontrada.");

            if (cita.Estado == EstadoCita.Cancelada)
                throw new BadRequestException("La cita ya se encuentra cancelada.");

            cita.Estado = EstadoCita.Cancelada;
            cita.MotivoCancelacion = dto.MotivoCancelacion;

            await _context.SaveChangesAsync();
        }

        public async Task<CitaResponseDto?> ObtenerPorIdAsync(int citaId)
        {
            return await _context.Citas
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .Where(c => c.Id == citaId)
                .Select(c => new CitaResponseDto
                {
                    Id = c.Id,
                    MedicoId = c.MedicoId,
                    Medico = c.Medico.Nombre,
                    PacienteId = c.PacienteId,
                    Paciente = c.Paciente.Nombre,
                    FechaHoraInicio = c.FechaHoraInicio,
                    FechaHoraFin = c.FechaHoraFin,
                    Motivo = c.Motivo,
                    Estado = c.Estado.ToString()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<API.Models.DTO.CitaResponseDto>> ObtenerAgendaDiaAsync(int medicoId, DateOnly fecha)
        {
            


            var medicoIdParam = new SqlParameter("@MedicoId", medicoId);
            var fechaInicioParam = new SqlParameter("@FechaInicio", fecha.ToDateTime(TimeOnly.MinValue));
            var fechaFinParam = new SqlParameter("@FechaFin", fecha.ToDateTime(TimeOnly.MinValue));

            return await _context.CitaResponseDtos
                .FromSqlRaw(
                    "EXEC sp_ObtenerAgendaMedicoPorRango @MedicoId, @FechaInicio, @FechaFin",
                    medicoIdParam,
                    fechaInicioParam,
                    fechaFinParam)
                .ToListAsync();
        }

        public async Task<List<CitaResponseDto>> ObtenerHistorialPacienteAsync(int pacienteId)
        {
            return await _context.Citas
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.FechaHoraInicio)
                .Select(c => new CitaResponseDto
                {
                    Id = c.Id,
                    MedicoId = c.MedicoId,
                    Medico = c.Medico.Nombre,
                    PacienteId = c.PacienteId,
                    Paciente = c.Paciente.Nombre,
                    FechaHoraInicio = c.FechaHoraInicio,
                    FechaHoraFin = c.FechaHoraFin,
                    Motivo = c.Motivo,
                    Estado = c.Estado.ToString()
                })
                .ToListAsync();
        }

        public async Task<List<DateTime>> ObtenerProximos5HorariosDisponiblesAsync(int medicoId, DateOnly fecha)
        {
            return await ObtenerProximos5HorariosDisponiblesDesdeFechaHoraAsync(
                medicoId,
                fecha.ToDateTime(TimeOnly.MinValue));
        }

        private async Task<List<DateTime>> ObtenerProximos5HorariosDisponiblesDesdeFechaHoraAsync(int medicoId, DateTime desde)
        {
            var medico = await _context.Medicos
                .Include(m => m.Horarios)
                .FirstOrDefaultAsync(m => m.Id == medicoId && m.Activo);

            if (medico == null)
                throw new NotFoundException("Médico no encontrado.");

            int duracion = await ObtenerDuracionPorEspecialidadAsync(medico.Fk_IdEspecialidad);
            var sugerencias = new List<DateTime>();

            for (int i = 0; i < 15 && sugerencias.Count < 5; i++)
            {
                var fecha = desde.Date.AddDays(i);
                var dia = fecha.DayOfWeek;

                var horariosDia = medico.Horarios
                    .Where(h => h.DiaSemana == dia)
                    .OrderBy(h => h.HoraInicio)
                    .ToList();

                foreach (var horario in horariosDia)
                {
                    var slotInicio = fecha.Add(horario.HoraInicio);
                    var slotFinLimite = fecha.Add(horario.HoraFin);

                    if (slotInicio < desde)
                        slotInicio = RedondearASiguienteBloque(desde, duracion);

                    while (slotInicio.AddMinutes(duracion) <= slotFinLimite)
                    {
                        var slotFin = slotInicio.AddMinutes(duracion);

                        bool ocupado = await _context.Citas.AnyAsync(c =>
                            c.MedicoId == medicoId &&
                            c.Estado == EstadoCita.Agendada &&
                            slotInicio < c.FechaHoraFin &&
                            slotFin > c.FechaHoraInicio);

                        if (!ocupado && slotInicio > DateTime.Now)
                            sugerencias.Add(slotInicio);

                        if (sugerencias.Count == 5)
                            break;

                        slotInicio = slotInicio.AddMinutes(duracion);
                    }

                    if (sugerencias.Count == 5)
                        break;
                }
            }

            return sugerencias;
        }

        private async Task<int> ObtenerDuracionPorEspecialidadAsync(int especialidadId)
        {
            var duracion = await _context.DuracionEspecialidades
                .Where(x => x.Fk_IdEspecialidad == especialidadId && x.Activo)
                .Select(x => x.DuracionMinutos)
                .FirstOrDefaultAsync();

            if (duracion <= 0)
                throw new BadRequestException("La especialidad del médico no tiene una duración configurada.");

            return duracion;
        }
        private static void ValidarHorarioMedico(Medico medico, DateTime inicio, DateTime fin)
        {
            var dia = inicio.DayOfWeek;

            var horario = medico.Horarios.FirstOrDefault(h =>
                h.DiaSemana == dia &&
                inicio.TimeOfDay >= h.HoraInicio &&
                fin.TimeOfDay <= h.HoraFin);

            if (horario == null)
                throw new ConflictException("La cita está fuera del horario de consulta del médico.");
        }

        private async Task<bool> TieneAlertaCancelacionesAsync(int pacienteId)
        {
            var fechaLimite = DateTime.Now.AddDays(-30);

            int canceladas = await _context.Citas.CountAsync(c =>
                c.PacienteId == pacienteId &&
                c.Estado == EstadoCita.Cancelada &&
                c.FechaHoraInicio >= fechaLimite);

            return canceladas >= 3;
        }

        private static DateTime RedondearASiguienteBloque(DateTime fecha, int minutos)
        {
            int resto = fecha.Minute % minutos;
            if (resto == 0 && fecha.Second == 0)
                return new DateTime(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, 0);

            int minutosSumar = minutos - resto;
            var nueva = fecha.AddMinutes(minutosSumar);
            return new DateTime(nueva.Year, nueva.Month, nueva.Day, nueva.Hour, nueva.Minute, 0);
        }
    }
}
