/*GO
Create DATABASE CitaSalud;
GO*/
CREATE TABLE Medicos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Especialidad NVARCHAR(100) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE HorariosMedico (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MedicoId INT NOT NULL,
    DiaSemana INT NOT NULL, -- 0 domingo, 1 lunes...
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    CONSTRAINT FK_HorariosMedico_Medicos FOREIGN KEY (MedicoId) REFERENCES Medicos(Id)
);

CREATE TABLE Pacientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    CorreoElectronico NVARCHAR(150) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);

CREATE TABLE Citas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MedicoId INT NOT NULL,
    PacienteId INT NOT NULL,
    FechaHoraInicio DATETIME2 NOT NULL,
    FechaHoraFin DATETIME2 NOT NULL,
    Motivo NVARCHAR(250) NOT NULL,
    Estado INT NOT NULL, -- 1 Agendada, 2 Cancelada, 3 Completada
    MotivoCancelacion NVARCHAR(250) NULL,
    FechaRegistro DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Citas_Medicos FOREIGN KEY (MedicoId) REFERENCES Medicos(Id),
    CONSTRAINT FK_Citas_Pacientes FOREIGN KEY (PacienteId) REFERENCES Pacientes(Id)
);