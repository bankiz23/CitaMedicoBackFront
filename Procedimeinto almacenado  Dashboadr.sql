CREATE OR ALTER PROCEDURE sp_ResumenDashboard
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
    DECLARE @Manana DATE = DATEADD(DAY, 1, @Hoy);

    SELECT
        (SELECT COUNT(*) FROM Medicos WHERE Activo = 1) AS TotalMedicos,
        (SELECT COUNT(*) FROM Pacientes WHERE Activo = 1) AS TotalPacientes,
        (SELECT COUNT(*) FROM Citas WHERE FechaHoraInicio >= @Hoy AND FechaHoraInicio < @Manana) AS TotalCitasHoy,
        (SELECT COUNT(*) FROM Citas WHERE Estado = 2) AS TotalCitasCanceladas;
END;