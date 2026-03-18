using API.Models.DTO;
using API.Models.Entities;
using API.Services.DTOs;
using API.Services.Exceptions;
using API.Services.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace API.Tests.Services
{
    public class CitaServiceTests
    {
        private AppDbContext CrearContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private async Task SeedBaseAsync(AppDbContext context)
        {
            var especialidad = new CatalogoEspecialidad
            {
                NombreEspecialidad = "Medicina General",
                Activo = true
            };

            context.CatalogoEspecialidades.Add(especialidad);
            await context.SaveChangesAsync();

            var duracion = new DuracionEspecialidad
            {
                Fk_IdEspecialidad = especialidad.Id,
                DuracionMinutos = 20,
                Activo = true
            };

            context.DuracionEspecialidades.Add(duracion);
            await context.SaveChangesAsync();

            var medico = new Medico
            {
                Nombre = "Dr. Juan Pérez",
                Fk_IdEspecialidad = especialidad.Id,
                Activo = true
            };

            context.Medicos.Add(medico);
            await context.SaveChangesAsync();

            var horario = new HorarioMedico
            {
                MedicoId = medico.Id,
                DiaSemana = DayOfWeek.Monday,
                HoraInicio = new TimeSpan(8, 0, 0),
                HoraFin = new TimeSpan(13, 0, 0)
            };

            context.HorariosMedico.Add(horario);

            var paciente = new Paciente
            {
                Nombre = "Paciente Uno",
                FechaNacimiento = new DateTime(1995, 1, 1),
                Telefono = "2221234567",
                CorreoElectronico = "paciente@test.com",
                Activo = true
            };

            context.Pacientes.Add(paciente);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task AgendarAsync_NoDebePermitirCitaEnPasado()
        {
            using var context = CrearContexto();
            await SeedBaseAsync(context);

            var service = new CitaService(context);

            var dto = new AgendarCitaDto
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = DateTime.Now.AddHours(-2),
                Motivo = "Consulta general"
            };

            await Assert.ThrowsAsync<BadRequestException>(() => service.AgendarAsync(dto));
        }

        [Fact]
        public async Task AgendarAsync_NoDebePermitirCitaFueraDeHorarioDelMedico()
        {
            using var context = CrearContexto();
            await SeedBaseAsync(context);

            var service = new CitaService(context);

            var fecha = ProximoDiaSemana(DayOfWeek.Monday).AddHours(7);

            var dto = new AgendarCitaDto
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = fecha,
                Motivo = "Consulta fuera de horario"
            };

            await Assert.ThrowsAsync<ConflictException>(() => service.AgendarAsync(dto));
        }

        [Fact]
        public async Task AgendarAsync_DebeCalcularDuracionSegunEspecialidad()
        {
            using var context = CrearContexto();
            await SeedBaseAsync(context);

            var service = new CitaService(context);

            var fecha = ProximoDiaSemana(DayOfWeek.Monday).AddHours(9);

            var dto = new AgendarCitaDto
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = fecha,
                Motivo = "Consulta de prueba"
            };

            var result = await service.AgendarAsync(dto);

            Assert.True(result.Exito);
            Assert.NotNull(result.CitaId);

            var cita = await context.Citas.FirstAsync(x => x.Id == result.CitaId);
            Assert.Equal(fecha, cita.FechaHoraInicio);
            Assert.Equal(fecha.AddMinutes(20), cita.FechaHoraFin);
        }

        [Fact]
        public async Task AgendarAsync_DebeRegresarSugerenciasCuandoHayConflicto()
        {
            using var context = CrearContexto();
            await SeedBaseAsync(context);

            var fechaBase = ProximoDiaSemana(DayOfWeek.Monday).AddHours(10);

            context.Citas.Add(new Cita
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = fechaBase,
                FechaHoraFin = fechaBase.AddMinutes(20),
                Motivo = "Cita existente",
                Estado = EstadoCita.Agendada,
                FechaRegistro = DateTime.Now
            });

            await context.SaveChangesAsync();

            var service = new CitaService(context);

            var dto = new AgendarCitaDto
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = fechaBase,
                Motivo = "Nueva cita"
            };

            var result = await service.AgendarAsync(dto);

            Assert.False(result.Exito);
            Assert.Equal("El horario solicitado no está disponible.", result.Mensaje);
            Assert.NotNull(result.Sugerencias);
            Assert.True(result.Sugerencias.Count > 0);
            Assert.True(result.Sugerencias.Count <= 5);
        }

        [Fact]
        public async Task AgendarAsync_DebeActivarAlertaSiPacienteTieneTresCancelacionesEn30Dias()
        {
            using var context = CrearContexto();
            await SeedBaseAsync(context);

            var fecha1 = DateTime.Now.AddDays(-5);
            var fecha2 = DateTime.Now.AddDays(-10);
            var fecha3 = DateTime.Now.AddDays(-15);

            context.Citas.AddRange(
                new Cita
                {
                    MedicoId = 1,
                    PacienteId = 1,
                    FechaHoraInicio = fecha1,
                    FechaHoraFin = fecha1.AddMinutes(20),
                    Motivo = "Cancelada 1",
                    Estado = EstadoCita.Cancelada,
                    FechaRegistro = DateTime.Now
                },
                new Cita
                {
                    MedicoId = 1,
                    PacienteId = 1,
                    FechaHoraInicio = fecha2,
                    FechaHoraFin = fecha2.AddMinutes(20),
                    Motivo = "Cancelada 2",
                    Estado = EstadoCita.Cancelada,
                    FechaRegistro = DateTime.Now
                },
                new Cita
                {
                    MedicoId = 1,
                    PacienteId = 1,
                    FechaHoraInicio = fecha3,
                    FechaHoraFin = fecha3.AddMinutes(20),
                    Motivo = "Cancelada 3",
                    Estado = EstadoCita.Cancelada,
                    FechaRegistro = DateTime.Now
                }
            );

            await context.SaveChangesAsync();

            var service = new CitaService(context);

            var fechaNueva = ProximoDiaSemana(DayOfWeek.Monday).AddHours(11);

            var dto = new AgendarCitaDto
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = fechaNueva,
                Motivo = "Nueva cita con alerta"
            };

            var result = await service.AgendarAsync(dto);

            Assert.True(result.Exito);
            Assert.True(result.AlertaCancelaciones);
        }

        [Fact]
        public async Task CancelarAsync_DebeCambiarEstadoACancelada()
        {
            using var context = CrearContexto();
            await SeedBaseAsync(context);

            var fecha = ProximoDiaSemana(DayOfWeek.Monday).AddHours(9);

            context.Citas.Add(new Cita
            {
                MedicoId = 1,
                PacienteId = 1,
                FechaHoraInicio = fecha,
                FechaHoraFin = fecha.AddMinutes(20),
                Motivo = "Consulta",
                Estado = EstadoCita.Agendada,
                FechaRegistro = DateTime.Now
            });

            await context.SaveChangesAsync();

            var service = new CitaService(context);

            await service.CancelarAsync(1, new CancelarCitaDto
            {
                MotivoCancelacion = "El paciente no asistirá"
            });

            var citaActualizada = await context.Citas.FirstAsync(x => x.Id == 1);

            Assert.Equal(EstadoCita.Cancelada, citaActualizada.Estado);
            Assert.Equal("El paciente no asistirá", citaActualizada.MotivoCancelacion);
        }

        private static DateTime ProximoDiaSemana(DayOfWeek dayOfWeek)
        {
            var fecha = DateTime.Today.AddDays(1);

            while (fecha.DayOfWeek != dayOfWeek)
            {
                fecha = fecha.AddDays(1);
            }

            return fecha;
        }
    }
}