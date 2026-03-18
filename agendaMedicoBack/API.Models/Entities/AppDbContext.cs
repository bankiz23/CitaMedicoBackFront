using API.Models.DTO;
using Microsoft.EntityFrameworkCore;
namespace API.Models.Entities
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<DashboardResumenDto> DashboardResumenDtos { get; set; }
        public DbSet<CitaResponseDto> CitaResponseDtos { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<HorarioMedico> HorariosMedico { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<CatalogoEspecialidad> CatalogoEspecialidades { get; set; }
        public DbSet<DuracionEspecialidad> DuracionEspecialidades { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DashboardResumenDto>().HasNoKey();
            modelBuilder.Entity<CitaResponseDto>().HasNoKey();
            modelBuilder.Entity<Medico>(entity =>
            {
                entity.ToTable("Medicos");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Activo)
                    .HasDefaultValue(true);

                entity.HasOne(x => x.Especialidad)
                    .WithMany(x => x.Medicos)
                    .HasForeignKey(x => x.Fk_IdEspecialidad)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Horarios)
                    .WithOne(x => x.Medico)
                    .HasForeignKey(x => x.MedicoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Citas)
                    .WithOne(x => x.Medico)
                    .HasForeignKey(x => x.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<HorarioMedico>(entity =>
            {
                entity.ToTable("HorariosMedico");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.DiaSemana)
                    .IsRequired();

                entity.Property(x => x.HoraInicio)
                    .IsRequired();

                entity.Property(x => x.HoraFin)
                    .IsRequired();
            });

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("Pacientes");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Telefono)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.CorreoElectronico)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.FechaNacimiento)
                    .IsRequired();

                entity.Property(x => x.Activo)
                    .HasDefaultValue(true);

                entity.HasMany(x => x.Citas)
                    .WithOne(x => x.Paciente)
                    .HasForeignKey(x => x.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("Citas");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FechaHoraInicio)
                    .IsRequired();

                entity.Property(x => x.FechaHoraFin)
                    .IsRequired();

                entity.Property(x => x.Motivo)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(x => x.MotivoCancelacion)
                    .HasMaxLength(250);

                entity.Property(x => x.Estado)
                    .IsRequired();

                entity.Property(x => x.FechaRegistro)
                    .HasDefaultValueSql("SYSDATETIME()");
            });
            modelBuilder.Entity<CatalogoEspecialidad>(entity =>
            {
                entity.ToTable("CatalogoEspecialidad");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.NombreEspecialidad)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Activo)
                    .HasDefaultValue(true);

                entity.HasMany(x => x.Medicos)
                    .WithOne(x => x.Especialidad)
                    .HasForeignKey(x => x.Fk_IdEspecialidad)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(x => x.Duraciones)
                    .WithOne(x => x.Especialidad)
                    .HasForeignKey(x => x.Fk_IdEspecialidad)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DuracionEspecialidad>(entity =>
            {
                entity.ToTable("DuracionEspecialidad");
                entity.HasKey(x => x.Id);

                entity.Property(x => x.DuracionMinutos)
                    .IsRequired();

                entity.Property(x => x.Activo)
                    .HasDefaultValue(true);
            });
        }
    }
}
