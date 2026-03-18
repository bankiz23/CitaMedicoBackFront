CREATE OR ALTER PROCEDURE sp_ObtenerAgendaMedicoPorRango
    @MedicoId INT,
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Inicio DATETIME2 = CAST(@FechaInicio AS DATETIME2);
    DECLARE @Fin DATETIME2 = DATEADD(DAY, 1, CAST(@FechaFin AS DATETIME2));

    SELECT
        c.Id,
        c.MedicoId,
        m.Nombre AS Medico,
        c.PacienteId,
        p.Nombre AS Paciente,
        c.FechaHoraInicio,
        c.FechaHoraFin,
        c.Motivo,
        CASE c.Estado
            WHEN 1 THEN 'Agendada'
            WHEN 2 THEN 'Cancelada'
            WHEN 3 THEN 'Completada'
            ELSE 'Desconocido'
        END AS Estado
    FROM Citas c
    INNER JOIN Medicos m ON c.MedicoId = m.Id
    INNER JOIN Pacientes p ON c.PacienteId = p.Id
    WHERE c.MedicoId = @MedicoId
      AND c.FechaHoraInicio >= @Inicio
      AND c.FechaHoraInicio < @Fin
    ORDER BY c.FechaHoraInicio;
END;