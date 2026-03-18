using API.Models.Entities;
using API.Services.DTOs;
using API.Services.Exceptions;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly AppDbContext _context;

        public EspecialidadService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EspecialidadDto>> GetAllAsync()
        {
            return await _context.CatalogoEspecialidades
                .Where(x => x.Activo)
                .OrderBy(x => x.NombreEspecialidad)
                .Select(x => new EspecialidadDto
                {
                    Id = x.Id,
                    NombreEspecialidad = x.NombreEspecialidad
                })
                .ToListAsync();
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
        public async Task<List<DateTime>> ObtenerProximos5HorariosDisponiblesDesdeFechaHoraAsync(int medicoId, DateTime desde)
        {
            var medico = await _context.Medicos
                .Include(m => m.Horarios)
                .FirstOrDefaultAsync(m => m.Id == medicoId && m.Activo);

            if (medico == null)
                throw new NotFoundException("Médico no encontrado.");

            int duracion = await ObtenerDuracionPorEspecialidadAsync(medico.Fk_IdEspecialidad);

            var sugerencias = new List<DateTime>();

            // revisa hasta 15 días hacia adelante para encontrar 5 espacios
            for (int i = 0; i < 15 && sugerencias.Count < 5; i++)
            {
                var fecha = desde.Date.AddDays(i);
                var dia = fecha.DayOfWeek;

                var horariosDia = medico.Horarios
                    .Where(h => h.DiaSemana == dia)
                    .OrderBy(h => h.HoraInicio)
                    .ToList();

                if (!horariosDia.Any())
                    continue;

                foreach (var horario in horariosDia)
                {
                    var inicioJornada = fecha.Add(horario.HoraInicio);
                    var finJornada = fecha.Add(horario.HoraFin);

                    DateTime slotInicio;

                    if (i == 0)
                    {
                        // el primer día empieza desde la hora solicitada, no desde el inicio del día
                        slotInicio = desde > inicioJornada ? desde : inicioJornada;
                    }
                    else
                    {
                        slotInicio = inicioJornada;
                    }

                    slotInicio = RedondearASiguienteBloque(slotInicio, duracion);

                    while (slotInicio.AddMinutes(duracion) <= finJornada)
                    {
                        var slotFin = slotInicio.AddMinutes(duracion);

                        bool ocupado = await _context.Citas.AnyAsync(c =>
                            c.MedicoId == medicoId &&
                            c.Estado == EstadoCita.Agendada &&
                            slotInicio < c.FechaHoraFin &&
                            slotFin > c.FechaHoraInicio);

                        if (!ocupado && slotInicio > DateTime.Now)
                        {
                            sugerencias.Add(slotInicio);
                        }

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
        private static DateTime RedondearASiguienteBloque(DateTime fecha, int minutosBloque)
        {
            var baseFecha = new DateTime(fecha.Year, fecha.Month, fecha.Day, fecha.Hour, fecha.Minute, 0);

            int resto = baseFecha.Minute % minutosBloque;

            if (resto == 0 && fecha.Second == 0)
                return baseFecha;

            int minutosSumar = minutosBloque - resto;
            return baseFecha.AddMinutes(minutosSumar);
        }
        private async Task<int> ObtenerDuracionPorEspecialidadAsync(int fkIdEspecialidad)
        {
            var duracion = await _context.DuracionEspecialidades
                .Where(x => x.Fk_IdEspecialidad == fkIdEspecialidad && x.Activo)
                .Select(x => x.DuracionMinutos)
                .FirstOrDefaultAsync();

            if (duracion <= 0)
                throw new BadRequestException("La especialidad del médico no tiene una duración configurada.");

            return duracion;
        }
    }
}