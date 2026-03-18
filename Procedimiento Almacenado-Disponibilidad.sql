CREATE OR ALTER PROCEDURE sp_ValidarDisponibilidadMedico
    @MedicoId INT,
    @FechaHoraInicio DATETIME2,
    @FechaHoraFin DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Citas
        WHERE MedicoId = @MedicoId
          AND Estado = 1
          AND @FechaHoraInicio < FechaHoraFin
          AND @FechaHoraFin > FechaHoraInicio
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Disponible;
        RETURN;
    END

    SELECT CAST(1 AS BIT) AS Disponible;
END;