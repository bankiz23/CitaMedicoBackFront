using API.Models.Entities;
using API.Services.DTOs;
using API.Services.Exceptions;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace API.Services.Services
{

public class PacienteService : IPacienteService
{
    private readonly AppDbContext _context;

    public PacienteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PacienteDto>> GetAllAsync()
    {
        return await _context.Pacientes
            .Where(x => x.Activo)
            .Select(x => new PacienteDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                FechaNacimiento = x.FechaNacimiento,
                Telefono = x.Telefono,
                CorreoElectronico = x.CorreoElectronico,
                Activo = x.Activo
            })
            .ToListAsync();
    }

    public async Task<PacienteDto?> GetByIdAsync(int id)
    {
        return await _context.Pacientes
            .Where(x => x.Id == id && x.Activo)
            .Select(x => new PacienteDto
            {
                Id = x.Id,
                Nombre = x.Nombre,
                FechaNacimiento = x.FechaNacimiento,
                Telefono = x.Telefono,
                CorreoElectronico = x.CorreoElectronico,
                Activo = x.Activo
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PacienteDto> CreateAsync(PacienteCreateDto dto)
    {
        var existeCorreo = await _context.Pacientes
            .AnyAsync(x => x.CorreoElectronico == dto.CorreoElectronico && x.Activo);

        if (existeCorreo)
            throw new ConflictException("Ya existe un paciente con ese correo electrónico.");

        var paciente = new Paciente
        {
            Nombre = dto.Nombre,
            FechaNacimiento = dto.FechaNacimiento,
            Telefono = dto.Telefono,
            CorreoElectronico = dto.CorreoElectronico,
            Activo = true
        };

        _context.Pacientes.Add(paciente);
        await _context.SaveChangesAsync();

        return new PacienteDto
        {
            Id = paciente.Id,
            Nombre = paciente.Nombre,
            FechaNacimiento = paciente.FechaNacimiento,
            Telefono = paciente.Telefono,
            CorreoElectronico = paciente.CorreoElectronico,
            Activo = paciente.Activo
        };
    }

    public async Task UpdateAsync(int id, PacienteCreateDto dto)
    {
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(x => x.Id == id && x.Activo);

        if (paciente == null)
            throw new NotFoundException("Paciente no encontrado.");

        bool existeCorreo = await _context.Pacientes.AnyAsync(x =>
            x.Id != id &&
            x.CorreoElectronico == dto.CorreoElectronico &&
            x.Activo);

        if (existeCorreo)
            throw new ConflictException("Ya existe otro paciente con ese correo electrónico.");

        paciente.Nombre = dto.Nombre;
        paciente.FechaNacimiento = dto.FechaNacimiento;
        paciente.Telefono = dto.Telefono;
        paciente.CorreoElectronico = dto.CorreoElectronico;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var paciente = await _context.Pacientes.FirstOrDefaultAsync(x => x.Id == id && x.Activo);

        if (paciente == null)
            throw new NotFoundException("Paciente no encontrado.");

        paciente.Activo = false;
        await _context.SaveChangesAsync();
    }
}
}
