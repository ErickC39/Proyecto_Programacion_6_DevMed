USE [HospitalUTC_DB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/*
    MIGRACION DEL ESQUEMA ANTERIOR AL ESQUEMA ACTUAL

    Ejecutar una sola vez antes de DatosPrueba_DevMed.sql.
    El script es reejecutable y conserva los registros existentes.
*/

BEGIN TRY
    BEGIN TRANSACTION;

    /* Usuarios: el proyecto actual requiere un nombre visible. */
    IF COL_LENGTH(N'dbo.Usuarios', N'Nombre') IS NULL
        ALTER TABLE dbo.Usuarios ADD Nombre NVARCHAR(100) NULL;

    EXEC(N'UPDATE dbo.Usuarios
           SET Nombre = Username
           WHERE Nombre IS NULL OR LTRIM(RTRIM(Nombre)) = N'''';');

    EXEC(N'ALTER TABLE dbo.Usuarios ALTER COLUMN Nombre NVARCHAR(100) NOT NULL;');

    /* Citas: columnas agregadas por el flujo de atencion y emergencias. */
    IF COL_LENGTH(N'dbo.Citas_Medicas', N'FechaHoraInicioAtencion') IS NULL
        ALTER TABLE dbo.Citas_Medicas ADD FechaHoraInicioAtencion DATETIME NULL;

    IF COL_LENGTH(N'dbo.Citas_Medicas', N'EsCitaControl') IS NULL
        ALTER TABLE dbo.Citas_Medicas ADD EsCitaControl BIT NULL;

    EXEC(N'UPDATE dbo.Citas_Medicas SET EsCitaControl = 0 WHERE EsCitaControl IS NULL;');
    EXEC(N'ALTER TABLE dbo.Citas_Medicas ALTER COLUMN EsCitaControl BIT NOT NULL;');

    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.object_id = dc.parent_object_id
           AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Citas_Medicas')
          AND c.name = N'EsCitaControl')
        EXEC(N'ALTER TABLE dbo.Citas_Medicas
               ADD CONSTRAINT DF_CitasMedicas_EsCitaControl DEFAULT (0) FOR EsCitaControl;');

    IF COL_LENGTH(N'dbo.Citas_Medicas', N'IdCitaOrigen') IS NULL
        ALTER TABLE dbo.Citas_Medicas ADD IdCitaOrigen INT NULL;

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE parent_object_id = OBJECT_ID(N'dbo.Citas_Medicas')
          AND name = N'FK_CitasMedicas_IdCitaOrigen')
        EXEC(N'ALTER TABLE dbo.Citas_Medicas WITH CHECK
               ADD CONSTRAINT FK_CitasMedicas_IdCitaOrigen
               FOREIGN KEY (IdCitaOrigen) REFERENCES dbo.Citas_Medicas(IdCita);');

    IF COL_LENGTH(N'dbo.Citas_Medicas', N'FueReagendadaPorEmergencia') IS NULL
        ALTER TABLE dbo.Citas_Medicas ADD FueReagendadaPorEmergencia BIT NULL;

    EXEC(N'UPDATE dbo.Citas_Medicas
           SET FueReagendadaPorEmergencia = 0
           WHERE FueReagendadaPorEmergencia IS NULL;');

    EXEC(N'ALTER TABLE dbo.Citas_Medicas
           ALTER COLUMN FueReagendadaPorEmergencia BIT NOT NULL;');

    IF NOT EXISTS (
        SELECT 1 FROM sys.default_constraints dc
        INNER JOIN sys.columns c
            ON c.object_id = dc.parent_object_id
           AND c.column_id = dc.parent_column_id
        WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Citas_Medicas')
          AND c.name = N'FueReagendadaPorEmergencia')
        EXEC(N'ALTER TABLE dbo.Citas_Medicas
               ADD CONSTRAINT DF_CitasMedicas_FueReagendada
               DEFAULT (0) FOR FueReagendadaPorEmergencia;');

    /* Notificaciones: conservar las columnas viejas y agregar las actuales. */
    IF OBJECT_ID(N'dbo.Bitacora_Notificaciones', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Bitacora_Notificaciones
        (
            IdNotificacion INT IDENTITY(1,1) NOT NULL
                CONSTRAINT PK_Bitacora_Notificaciones PRIMARY KEY,
            Fecha DATETIME NOT NULL
                CONSTRAINT DF_Bitacora_Notificaciones_Fecha DEFAULT (GETDATE()),
            IdCitaAfectada INT NULL,
            IdCitaEmergencia INT NULL,
            Mensaje NVARCHAR(500) NOT NULL
        );
    END;

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'IdCitaAfectada') IS NULL
        ALTER TABLE dbo.Bitacora_Notificaciones ADD IdCitaAfectada INT NULL;

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'IdCitaEmergencia') IS NULL
        ALTER TABLE dbo.Bitacora_Notificaciones ADD IdCitaEmergencia INT NULL;

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'IdCita') IS NOT NULL
        EXEC(N'UPDATE dbo.Bitacora_Notificaciones
               SET IdCitaAfectada = IdCita
               WHERE IdCitaAfectada IS NULL AND IdCita IS NOT NULL;');

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'IdPaciente') IS NOT NULL
        ALTER TABLE dbo.Bitacora_Notificaciones ALTER COLUMN IdPaciente INT NULL;

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'TipoNotificacion') IS NOT NULL
        ALTER TABLE dbo.Bitacora_Notificaciones ALTER COLUMN TipoNotificacion NVARCHAR(100) NULL;

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'Fecha') IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM sys.default_constraints dc
           INNER JOIN sys.columns c
               ON c.object_id = dc.parent_object_id
              AND c.column_id = dc.parent_column_id
           WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Bitacora_Notificaciones')
             AND c.name = N'Fecha')
        ALTER TABLE dbo.Bitacora_Notificaciones
            ADD CONSTRAINT DF_BitacoraNotificaciones_Fecha DEFAULT (GETDATE()) FOR Fecha;

    IF COL_LENGTH(N'dbo.Bitacora_Notificaciones', N'Leida') IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM sys.default_constraints dc
           INNER JOIN sys.columns c
               ON c.object_id = dc.parent_object_id
              AND c.column_id = dc.parent_column_id
           WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Bitacora_Notificaciones')
             AND c.name = N'Leida')
        ALTER TABLE dbo.Bitacora_Notificaciones
            ADD CONSTRAINT DF_BitacoraNotificaciones_Leida DEFAULT (0) FOR Leida;

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE parent_object_id = OBJECT_ID(N'dbo.Bitacora_Notificaciones')
          AND name = N'FK_BitacoraNotificaciones_CitaAfectada')
        EXEC(N'ALTER TABLE dbo.Bitacora_Notificaciones WITH CHECK
               ADD CONSTRAINT FK_BitacoraNotificaciones_CitaAfectada
               FOREIGN KEY (IdCitaAfectada) REFERENCES dbo.Citas_Medicas(IdCita);');

    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE parent_object_id = OBJECT_ID(N'dbo.Bitacora_Notificaciones')
          AND name = N'FK_BitacoraNotificaciones_CitaEmergencia')
        EXEC(N'ALTER TABLE dbo.Bitacora_Notificaciones WITH CHECK
               ADD CONSTRAINT FK_BitacoraNotificaciones_CitaEmergencia
               FOREIGN KEY (IdCitaEmergencia) REFERENCES dbo.Citas_Medicas(IdCita);');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    IF OBJECT_ID(N'dbo.Bitacora_Errores', N'U') IS NOT NULL
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'Migracion_EsquemaActual', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());

    THROW;
END CATCH;
GO

/* Vistas consumidas por los repositorios actuales. */
CREATE OR ALTER VIEW dbo.vw_Usuarios
AS
SELECT
    u.IdUsuario,
    u.Nombre,
    u.Username,
    u.IdRol,
    r.NombreRol AS Rol,
    u.Activo,
    u.FechaCreacion
FROM dbo.Usuarios u
LEFT JOIN dbo.Roles r ON r.IdRol = u.IdRol;
GO

CREATE OR ALTER VIEW dbo.vw_Citas
AS
SELECT
    c.IdCita,
    c.IdPaciente,
    p.Identificacion AS PacienteIdentificacion,
    p.Nombre + N' ' + p.Apellidos AS PacienteNombreCompleto,
    c.IdEmpleado_Medico,
    m.Nombre + N' ' + m.Apellidos AS MedicoNombreCompleto,
    m.Especialidad,
    c.FechaHoraCita,
    c.FechaHoraLlegada,
    CASE
        WHEN c.EstadoCita = N'En espera' AND c.FechaHoraLlegada IS NOT NULL
            THEN DATEDIFF(MINUTE, c.FechaHoraLlegada, GETDATE())
        ELSE ISNULL(c.TiempoEsperaMinutos, 0)
    END AS TiempoEsperaMinutos,
    c.EstadoCita,
    c.ResultadoConsulta,
    ISNULL(c.RequiereControl, 0) AS RequiereControl,
    c.PrioridadCita,
    c.EsCitaControl,
    CAST(CASE WHEN EXISTS (
        SELECT 1
        FROM dbo.Citas_Medicas cp
        WHERE cp.IdEmpleado_Medico = c.IdEmpleado_Medico
          AND cp.EstadoCita = N'En Progreso'
          AND cp.EsCitaControl = 1
    ) THEN 1 ELSE 0 END AS BIT) AS CitaPreviaEsControl,
    c.FueReagendadaPorEmergencia,
    (
        SELECT TOP (1) b.Mensaje
        FROM dbo.Bitacora_Notificaciones b
        WHERE b.IdCitaAfectada = c.IdCita
        ORDER BY b.Fecha DESC
    ) AS MensajeReagendo
FROM dbo.Citas_Medicas c
INNER JOIN dbo.Pacientes p ON p.IdPaciente = c.IdPaciente
INNER JOIN dbo.Empleados m ON m.IdEmpleado = c.IdEmpleado_Medico;
GO

/* Procedimientos de usuarios con la firma usada por UsuarioAdminRepository. */
CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Actualizar
    @IdUsuario INT,
    @Nombre NVARCHAR(100),
    @Username NVARCHAR(50),
    @IdRol INT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (
            SELECT 1 FROM dbo.Usuarios
            WHERE Username = @Username AND IdUsuario <> @IdUsuario)
            THROW 56001, 'Ya existe otro usuario con ese nombre.', 1;

        UPDATE dbo.Usuarios
        SET Nombre = @Nombre,
            Username = @Username,
            IdRol = @IdRol,
            Activo = @Activo
        WHERE IdUsuario = @IdUsuario;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Usuario_Actualizar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Crear
    @Nombre NVARCHAR(100),
    @Username NVARCHAR(50),
    @Password NVARCHAR(100),
    @IdRol INT,
    @Activo BIT = 1,
    @IdGenerado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Username = @Username)
            THROW 56000, 'Ya existe un usuario con ese nombre.', 1;

        DECLARE @Salt VARBINARY(64) = CRYPT_GEN_RANDOM(64);
        DECLARE @Hash VARBINARY(64) =
            HASHBYTES('SHA2_512', @Salt + CONVERT(VARBINARY(200), @Password));

        INSERT INTO dbo.Usuarios
            (Nombre, Username, PasswordHash, PasswordSalt, IdRol, Activo)
        VALUES
            (@Nombre, @Username, @Hash, @Salt, @IdRol, @Activo);

        SET @IdGenerado = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Usuario_Crear', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Usuario_Eliminar
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM dbo.Empleados WHERE IdUsuario = @IdUsuario)
            THROW 56002, 'No se puede eliminar: el usuario esta vinculado a un empleado.', 1;

        DELETE FROM dbo.Usuarios WHERE IdUsuario = @IdUsuario;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Usuario_Eliminar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

/* Procedimientos de citas con las firmas usadas por CitaRepository. */
CREATE OR ALTER PROCEDURE dbo.sp_Cita_Agendar
    @Identificacion NVARCHAR(50),
    @IdMedico INT,
    @FechaHoraCita DATETIME,
    @PrioridadCita NVARCHAR(20),
    @IdCitaGenerada INT OUTPUT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT,
    @HoraSugerida NVARCHAR(10) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @IdCitaGenerada = 0;
    SET @HoraSugerida = NULL;

    BEGIN TRY
        DECLARE @DuracionMin INT = 30;
        DECLARE @IdPaciente INT;
        DECLARE @Dia DATE = CAST(@FechaHoraCita AS DATE);

        SELECT @IdPaciente = IdPaciente
        FROM dbo.Pacientes
        WHERE Identificacion = @Identificacion;

        IF @IdPaciente IS NULL
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'No se encontro un paciente registrado con esa identificacion.';
            RETURN;
        END;

        IF EXISTS (
            SELECT 1 FROM dbo.Citas_Medicas
            WHERE IdEmpleado_Medico = @IdMedico
              AND EstadoCita NOT IN (N'Cancelada', N'Finalizada')
              AND FechaHoraCita < DATEADD(MINUTE, @DuracionMin, @FechaHoraCita)
              AND DATEADD(MINUTE, @DuracionMin, FechaHoraCita) > @FechaHoraCita)
        BEGIN
            DECLARE @InicioDia DATETIME = CAST(@Dia AS DATETIME) + CAST('07:00' AS DATETIME);
            DECLARE @FinDia DATETIME = CAST(@Dia AS DATETIME) + CAST('16:00' AS DATETIME);
            DECLARE @Hora DATETIME = @FechaHoraCita;

            SET @Hora = DATEADD(MINUTE,
                CEILING(DATEDIFF(MINUTE, @InicioDia, @Hora) * 1.0 / @DuracionMin) * @DuracionMin,
                @InicioDia);
            IF @Hora < @InicioDia SET @Hora = @InicioDia;

            WHILE @Hora <= @FinDia
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM dbo.Citas_Medicas
                    WHERE IdEmpleado_Medico = @IdMedico
                      AND EstadoCita NOT IN (N'Cancelada', N'Finalizada')
                      AND FechaHoraCita < DATEADD(MINUTE, @DuracionMin, @Hora)
                      AND DATEADD(MINUTE, @DuracionMin, FechaHoraCita) > @Hora)
                BEGIN
                    SET @HoraSugerida = FORMAT(@Hora, 'HH\:mm');
                    BREAK;
                END;
                SET @Hora = DATEADD(MINUTE, @DuracionMin, @Hora);
            END;

            IF @HoraSugerida IS NULL
            BEGIN
                SET @Hora = @InicioDia;
                WHILE @Hora < @FechaHoraCita
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM dbo.Citas_Medicas
                        WHERE IdEmpleado_Medico = @IdMedico
                          AND EstadoCita NOT IN (N'Cancelada', N'Finalizada')
                          AND FechaHoraCita < DATEADD(MINUTE, @DuracionMin, @Hora)
                          AND DATEADD(MINUTE, @DuracionMin, FechaHoraCita) > @Hora)
                    BEGIN
                        SET @HoraSugerida = FORMAT(@Hora, 'HH\:mm');
                        BREAK;
                    END;
                    SET @Hora = DATEADD(MINUTE, @DuracionMin, @Hora);
                END;
            END;

            SET @CodigoSalida = CASE WHEN @HoraSugerida IS NULL THEN 2 ELSE 3 END;
            SET @MensajeSalida = CASE
                WHEN @HoraSugerida IS NULL THEN N'El medico no tiene espacio disponible ese dia.'
                ELSE N'No hay espacio a esa hora para este medico.' END;
            RETURN;
        END;

        BEGIN TRANSACTION;
        INSERT INTO dbo.Citas_Medicas
            (IdPaciente, IdEmpleado_Medico, FechaHoraCita, PrioridadCita,
             EstadoCita, TiempoEsperaMinutos, EsCitaControl)
        VALUES
            (@IdPaciente, @IdMedico, @FechaHoraCita,
             ISNULL(NULLIF(@PrioridadCita, N''), N'Normal'), N'Agendada', 0, 0);

        SET @IdCitaGenerada = SCOPE_IDENTITY();

        UPDATE dbo.Pacientes
        SET AntecedentesMedicos = ISNULL(AntecedentesMedicos + CHAR(13) + CHAR(10), N'')
            + N'Proxima revision agendada: ' + CONVERT(NVARCHAR, @FechaHoraCita, 120)
        WHERE IdPaciente = @IdPaciente;

        COMMIT TRANSACTION;
        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Cita agendada correctamente.';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_Agendar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Cita_AgendarEmergencia
    @IdEmpleado INT,
    @IdMedico INT,
    @FechaHoraCita DATETIME,
    @IdCitaGenerada INT OUTPUT,
    @CitasReagendadas INT OUTPUT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @IdCitaGenerada = 0;
    SET @CitasReagendadas = 0;

    BEGIN TRY
        DECLARE @DuracionMin INT = 30;
        DECLARE @IdPaciente INT;
        SELECT @IdPaciente = IdPacienteVinculado
        FROM dbo.Empleados WHERE IdEmpleado = @IdEmpleado;

        IF @IdPaciente IS NULL
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'El empleado no tiene un paciente vinculado para emergencia.';
            RETURN;
        END;

        BEGIN TRANSACTION;
        DECLARE @IdCitaChoque INT;
        DECLARE @NuevaHora DATETIME;
        DECLARE CitasChoque CURSOR LOCAL FAST_FORWARD FOR
            SELECT IdCita
            FROM dbo.Citas_Medicas
            WHERE IdEmpleado_Medico = @IdMedico
              AND EstadoCita IN (N'Agendada', N'En espera')
              AND FechaHoraCita < DATEADD(MINUTE, @DuracionMin, @FechaHoraCita)
              AND DATEADD(MINUTE, @DuracionMin, FechaHoraCita) > @FechaHoraCita;

        OPEN CitasChoque;
        FETCH NEXT FROM CitasChoque INTO @IdCitaChoque;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @NuevaHora = DATEADD(MINUTE, @DuracionMin, @FechaHoraCita);

            WHILE EXISTS (
                SELECT 1 FROM dbo.Citas_Medicas
                WHERE IdEmpleado_Medico = @IdMedico
                  AND IdCita <> @IdCitaChoque
                  AND EstadoCita NOT IN (N'Cancelada', N'Finalizada')
                  AND FechaHoraCita < DATEADD(MINUTE, @DuracionMin, @NuevaHora)
                  AND DATEADD(MINUTE, @DuracionMin, FechaHoraCita) > @NuevaHora)
                SET @NuevaHora = DATEADD(MINUTE, @DuracionMin, @NuevaHora);

            UPDATE dbo.Citas_Medicas
            SET FechaHoraCita = @NuevaHora,
                FueReagendadaPorEmergencia = 1
            WHERE IdCita = @IdCitaChoque;

            INSERT INTO dbo.Bitacora_Notificaciones (IdCitaAfectada, Mensaje)
            VALUES
                (@IdCitaChoque,
                 N'Su cita fue trasladada a ' + CONVERT(NVARCHAR, @NuevaHora, 120)
                 + N' debido a una emergencia.');

            SET @CitasReagendadas += 1;
            FETCH NEXT FROM CitasChoque INTO @IdCitaChoque;
        END;

        CLOSE CitasChoque;
        DEALLOCATE CitasChoque;

        INSERT INTO dbo.Citas_Medicas
            (IdPaciente, IdEmpleado_Medico, FechaHoraCita, PrioridadCita,
             EstadoCita, TiempoEsperaMinutos, EsCitaControl)
        VALUES
            (@IdPaciente, @IdMedico, @FechaHoraCita, N'Alta', N'Agendada', 0, 0);

        SET @IdCitaGenerada = SCOPE_IDENTITY();

        INSERT INTO dbo.Bitacora_Notificaciones (IdCitaEmergencia, Mensaje)
        VALUES (@IdCitaGenerada, N'Cita de emergencia agendada.');

        COMMIT TRANSACTION;
        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Cita de emergencia agendada correctamente.';
    END TRY
    BEGIN CATCH
        IF CURSOR_STATUS('local', 'CitasChoque') >= 0 CLOSE CitasChoque;
        IF CURSOR_STATUS('local', 'CitasChoque') >= -1 DEALLOCATE CitasChoque;
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_AgendarEmergencia', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Cita_Cancelar
    @IdCita INT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Estado NVARCHAR(50);
        SELECT @Estado = EstadoCita FROM dbo.Citas_Medicas WHERE IdCita = @IdCita;

        IF @Estado IS NULL
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'La cita indicada no existe.';
            RETURN;
        END;

        IF @Estado IN (N'Finalizada', N'Cancelada')
        BEGIN
            SET @CodigoSalida = 2;
            SET @MensajeSalida = N'La cita no se puede cancelar en su estado actual.';
            RETURN;
        END;

        BEGIN TRANSACTION;
        UPDATE dbo.Citas_Medicas SET EstadoCita = N'Cancelada' WHERE IdCita = @IdCita;
        COMMIT TRANSACTION;

        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Cita cancelada correctamente.';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_Cancelar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Cita_Eliminar
    @IdCita INT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Citas_Medicas WHERE IdCita = @IdCita)
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'La cita indicada no existe.';
            RETURN;
        END;

        BEGIN TRANSACTION;
        DELETE FROM dbo.Bitacora_Notificaciones
        WHERE IdCitaAfectada = @IdCita OR IdCitaEmergencia = @IdCita;

        UPDATE dbo.Citas_Medicas SET IdCitaOrigen = NULL WHERE IdCitaOrigen = @IdCita;
        DELETE FROM dbo.Citas_Medicas WHERE IdCita = @IdCita;
        COMMIT TRANSACTION;

        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Cita eliminada correctamente.';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_Eliminar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Cita_Finalizar
    @IdCita INT,
    @ResultadoConsulta NVARCHAR(MAX),
    @RequiereControl BIT,
    @FechaControl DATETIME = NULL,
    @DetallesControl NVARCHAR(MAX) = NULL,
    @IdCitaControlGenerada INT OUTPUT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @IdCitaControlGenerada = 0;

    BEGIN TRY
        DECLARE @Estado NVARCHAR(50), @IdPaciente INT, @IdMedico INT;
        SELECT @Estado = EstadoCita,
               @IdPaciente = IdPaciente,
               @IdMedico = IdEmpleado_Medico
        FROM dbo.Citas_Medicas WHERE IdCita = @IdCita;

        IF @Estado IS NULL
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'La cita indicada no existe.';
            RETURN;
        END;

        IF @Estado <> N'En Progreso'
        BEGIN
            SET @CodigoSalida = 2;
            SET @MensajeSalida = N'Solo se puede finalizar una cita en estado En Progreso.';
            RETURN;
        END;

        BEGIN TRANSACTION;
        UPDATE dbo.Citas_Medicas
        SET EstadoCita = N'Finalizada',
            ResultadoConsulta = @ResultadoConsulta,
            RequiereControl = @RequiereControl
        WHERE IdCita = @IdCita;

        UPDATE dbo.Pacientes
        SET AntecedentesMedicos = ISNULL(AntecedentesMedicos + CHAR(13) + CHAR(10), N'')
            + N'Resultado de consulta (' + CONVERT(NVARCHAR, GETDATE(), 120) + N'): '
            + @ResultadoConsulta
        WHERE IdPaciente = @IdPaciente;

        IF @RequiereControl = 1 AND @FechaControl IS NOT NULL
        BEGIN
            INSERT INTO dbo.Citas_Medicas
                (IdPaciente, IdEmpleado_Medico, FechaHoraCita, PrioridadCita,
                 EstadoCita, TiempoEsperaMinutos, EsCitaControl, IdCitaOrigen)
            VALUES
                (@IdPaciente, @IdMedico, @FechaControl, N'Normal', N'Agendada', 0, 1, @IdCita);

            SET @IdCitaControlGenerada = SCOPE_IDENTITY();
        END;

        COMMIT TRANSACTION;
        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Cita finalizada correctamente.';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_Finalizar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Cita_IniciarAtencion
    @IdCita INT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Estado NVARCHAR(50), @FechaLlegada DATETIME;
        SELECT @Estado = EstadoCita, @FechaLlegada = FechaHoraLlegada
        FROM dbo.Citas_Medicas WHERE IdCita = @IdCita;

        IF @Estado IS NULL
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'La cita indicada no existe.';
            RETURN;
        END;

        IF @Estado <> N'En espera'
        BEGIN
            SET @CodigoSalida = 2;
            SET @MensajeSalida = N'Solo se puede iniciar una cita en estado En espera.';
            RETURN;
        END;

        BEGIN TRANSACTION;
        UPDATE dbo.Citas_Medicas
        SET FechaHoraInicioAtencion = GETDATE(),
            EstadoCita = N'En Progreso',
            TiempoEsperaMinutos = DATEDIFF(MINUTE, @FechaLlegada, GETDATE())
        WHERE IdCita = @IdCita;
        COMMIT TRANSACTION;

        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Atencion iniciada.';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_IniciarAtencion', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Cita_RegistrarLlegada
    @IdCita INT,
    @MensajeSalida NVARCHAR(500) OUTPUT,
    @CodigoSalida INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @Estado NVARCHAR(50);
        SELECT @Estado = EstadoCita FROM dbo.Citas_Medicas WHERE IdCita = @IdCita;

        IF @Estado IS NULL
        BEGIN
            SET @CodigoSalida = 1;
            SET @MensajeSalida = N'La cita indicada no existe.';
            RETURN;
        END;

        IF @Estado <> N'Agendada'
        BEGIN
            SET @CodigoSalida = 2;
            SET @MensajeSalida = N'Solo se puede registrar la llegada de una cita agendada.';
            RETURN;
        END;

        BEGIN TRANSACTION;
        UPDATE dbo.Citas_Medicas
        SET FechaHoraLlegada = GETDATE(),
            EstadoCita = N'En espera',
            TiempoEsperaMinutos = 0
        WHERE IdCita = @IdCita;
        COMMIT TRANSACTION;

        SET @CodigoSalida = 0;
        SET @MensajeSalida = N'Llegada registrada correctamente.';
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'sp_Cita_RegistrarLlegada', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
        THROW;
    END CATCH;
END;
GO

PRINT N'Migracion del esquema finalizada correctamente.';
PRINT N'Ahora puede ejecutar DatosPrueba_DevMed.sql.';
GO
