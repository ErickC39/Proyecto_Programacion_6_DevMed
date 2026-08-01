USE [HospitalUTC_DB];
GO

/* ================================================================
   MODULO: EXAMENES MEDICOS
   Ejecutar despues de HospitalUTC_DB.sql.
   El script es reejecutable.
   ================================================================ */

/* ---- CATALOGO: TIPOS DE EXAMEN ---- */
IF OBJECT_ID(N'dbo.Tipos_Examen', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Tipos_Examen
    (
        IdTipoExamen INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Tipos_Examen PRIMARY KEY,
        Descripcion NVARCHAR(100) NOT NULL
            CONSTRAINT UQ_Tipos_Examen_Descripcion UNIQUE,
        Activo BIT NOT NULL
            CONSTRAINT DF_Tipos_Examen_Activo DEFAULT (1)
    );
END;
GO

/* ---- CATALOGO: ESTADOS DEL EXAMEN ---- */
IF OBJECT_ID(N'dbo.Estados_Examen', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Estados_Examen
    (
        IdEstadoExamen INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Estados_Examen PRIMARY KEY,
        Descripcion NVARCHAR(50) NOT NULL
            CONSTRAINT UQ_Estados_Examen_Descripcion UNIQUE
    );
END;
GO

/* ---- DATOS INICIALES DE LOS CATALOGOS ---- */
IF NOT EXISTS (SELECT 1 FROM dbo.Tipos_Examen WHERE Descripcion = N'Hemograma completo')
    INSERT INTO dbo.Tipos_Examen (Descripcion) VALUES (N'Hemograma completo');

IF NOT EXISTS (SELECT 1 FROM dbo.Tipos_Examen WHERE Descripcion = N'Examen general de orina')
    INSERT INTO dbo.Tipos_Examen (Descripcion) VALUES (N'Examen general de orina');

IF NOT EXISTS (SELECT 1 FROM dbo.Tipos_Examen WHERE Descripcion = N'Quimica sanguinea')
    INSERT INTO dbo.Tipos_Examen (Descripcion) VALUES (N'Quimica sanguinea');

IF NOT EXISTS (SELECT 1 FROM dbo.Tipos_Examen WHERE Descripcion = N'Radiografia')
    INSERT INTO dbo.Tipos_Examen (Descripcion) VALUES (N'Radiografia');

IF NOT EXISTS (SELECT 1 FROM dbo.Tipos_Examen WHERE Descripcion = N'Ultrasonido')
    INSERT INTO dbo.Tipos_Examen (Descripcion) VALUES (N'Ultrasonido');

IF NOT EXISTS (SELECT 1 FROM dbo.Tipos_Examen WHERE Descripcion = N'Electrocardiograma')
    INSERT INTO dbo.Tipos_Examen (Descripcion) VALUES (N'Electrocardiograma');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Examen WHERE Descripcion = N'Solicitado')
    INSERT INTO dbo.Estados_Examen (Descripcion) VALUES (N'Solicitado');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Examen WHERE Descripcion = N'En proceso')
    INSERT INTO dbo.Estados_Examen (Descripcion) VALUES (N'En proceso');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Examen WHERE Descripcion = N'Completado')
    INSERT INTO dbo.Estados_Examen (Descripcion) VALUES (N'Completado');

IF NOT EXISTS (SELECT 1 FROM dbo.Estados_Examen WHERE Descripcion = N'Cancelado')
    INSERT INTO dbo.Estados_Examen (Descripcion) VALUES (N'Cancelado');
GO

/* ---- TABLA PRINCIPAL ---- */
IF OBJECT_ID(N'dbo.Examenes_Medicos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Examenes_Medicos
    (
        IdExamenMedico INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_Examenes_Medicos PRIMARY KEY,
        IdPaciente INT NOT NULL,
        IdEmpleadoMedico INT NOT NULL,
        IdTipoExamen INT NOT NULL,
        FechaSolicitud DATETIME2(0) NOT NULL
            CONSTRAINT DF_Examenes_Medicos_FechaSolicitud DEFAULT (SYSDATETIME()),
        IdEstadoExamen INT NOT NULL,
        Resultado NVARCHAR(MAX) NULL,
        Observaciones NVARCHAR(1000) NULL,
        FechaResultado DATETIME2(0) NULL
    );
END;
GO

/* ---- LLAVES FORANEAS ---- */
IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_ExamenesMedicos_Pacientes'
)
BEGIN
    ALTER TABLE dbo.Examenes_Medicos WITH CHECK
    ADD CONSTRAINT FK_ExamenesMedicos_Pacientes
        FOREIGN KEY (IdPaciente) REFERENCES dbo.Pacientes (IdPaciente);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_ExamenesMedicos_Empleados'
)
BEGIN
    ALTER TABLE dbo.Examenes_Medicos WITH CHECK
    ADD CONSTRAINT FK_ExamenesMedicos_Empleados
        FOREIGN KEY (IdEmpleadoMedico) REFERENCES dbo.Empleados (IdEmpleado);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_ExamenesMedicos_TiposExamen'
)
BEGIN
    ALTER TABLE dbo.Examenes_Medicos WITH CHECK
    ADD CONSTRAINT FK_ExamenesMedicos_TiposExamen
        FOREIGN KEY (IdTipoExamen) REFERENCES dbo.Tipos_Examen (IdTipoExamen);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_ExamenesMedicos_EstadosExamen'
)
BEGIN
    ALTER TABLE dbo.Examenes_Medicos WITH CHECK
    ADD CONSTRAINT FK_ExamenesMedicos_EstadosExamen
        FOREIGN KEY (IdEstadoExamen) REFERENCES dbo.Estados_Examen (IdEstadoExamen);
END;
GO

/* ---- INDICES ---- */
IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ExamenesMedicos_IdPaciente'
      AND object_id = OBJECT_ID(N'dbo.Examenes_Medicos')
)
BEGIN
    CREATE INDEX IX_ExamenesMedicos_IdPaciente
        ON dbo.Examenes_Medicos (IdPaciente, FechaSolicitud DESC);
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_ExamenesMedicos_IdEstadoExamen'
      AND object_id = OBJECT_ID(N'dbo.Examenes_Medicos')
)
BEGIN
    CREATE INDEX IX_ExamenesMedicos_IdEstadoExamen
        ON dbo.Examenes_Medicos (IdEstadoExamen);
END;
GO

/* ---- VISTA PARA LISTAR Y VINCULAR AL EXPEDIENTE ---- */
CREATE OR ALTER VIEW dbo.vw_ExamenesMedicos
AS
SELECT
    em.IdExamenMedico,
    em.IdPaciente,
    p.Identificacion AS PacienteIdentificacion,
    p.Nombre + N' ' + p.Apellidos AS PacienteNombreCompleto,
    em.IdEmpleadoMedico,
    e.Nombre + N' ' + e.Apellidos AS MedicoNombreCompleto,
    em.IdTipoExamen,
    te.Descripcion AS TipoExamen,
    em.FechaSolicitud,
    em.IdEstadoExamen,
    ee.Descripcion AS EstadoExamen,
    em.Resultado,
    em.Observaciones,
    em.FechaResultado
FROM dbo.Examenes_Medicos em
INNER JOIN dbo.Pacientes p
    ON p.IdPaciente = em.IdPaciente
INNER JOIN dbo.Empleados e
    ON e.IdEmpleado = em.IdEmpleadoMedico
INNER JOIN dbo.Tipos_Examen te
    ON te.IdTipoExamen = em.IdTipoExamen
INNER JOIN dbo.Estados_Examen ee
    ON ee.IdEstadoExamen = em.IdEstadoExamen;
GO

/* ---- SP: CREAR SOLICITUD ---- */
CREATE OR ALTER PROCEDURE dbo.sp_ExamenMedico_Crear
    @IdPaciente INT,
    @IdEmpleadoMedico INT,
    @IdTipoExamen INT,
    @FechaSolicitud DATETIME2(0),
    @Observaciones NVARCHAR(1000) = NULL,
    @IdGenerado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Pacientes
            WHERE IdPaciente = @IdPaciente
        )
            THROW 56000, 'El paciente seleccionado no existe.', 1;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Empleados e
            INNER JOIN dbo.Usuarios u ON u.IdUsuario = e.IdUsuario
            INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
            WHERE e.IdEmpleado = @IdEmpleadoMedico
              AND r.NombreRol = N'Medico'
              AND u.Activo = 1
        )
            THROW 56001, 'El empleado seleccionado no es un medico activo.', 1;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Tipos_Examen
            WHERE IdTipoExamen = @IdTipoExamen
              AND Activo = 1
        )
            THROW 56002, 'El tipo de examen seleccionado no existe o esta inactivo.', 1;

        DECLARE @IdEstadoSolicitado INT;
        SELECT @IdEstadoSolicitado = IdEstadoExamen
        FROM dbo.Estados_Examen
        WHERE Descripcion = N'Solicitado';

        IF @IdEstadoSolicitado IS NULL
            THROW 56003, 'No existe el estado Solicitado en el catalogo.', 1;

        INSERT INTO dbo.Examenes_Medicos
        (
            IdPaciente,
            IdEmpleadoMedico,
            IdTipoExamen,
            FechaSolicitud,
            IdEstadoExamen,
            Observaciones
        )
        VALUES
        (
            @IdPaciente,
            @IdEmpleadoMedico,
            @IdTipoExamen,
            @FechaSolicitud,
            @IdEstadoSolicitado,
            @Observaciones
        );

        SET @IdGenerado = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            ('sp_ExamenMedico_Crear', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());

        THROW;
    END CATCH
END;
GO

/* ---- SP: ACTUALIZAR ESTADO Y RESULTADO ---- */
CREATE OR ALTER PROCEDURE dbo.sp_ExamenMedico_Actualizar
    @IdExamenMedico INT,
    @IdPaciente INT,
    @IdEmpleadoMedico INT,
    @IdTipoExamen INT,
    @FechaSolicitud DATETIME2(0),
    @IdEstadoExamen INT,
    @Resultado NVARCHAR(MAX) = NULL,
    @Observaciones NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Examenes_Medicos
            WHERE IdExamenMedico = @IdExamenMedico
        )
            THROW 56010, 'El examen medico no existe.', 1;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Pacientes
            WHERE IdPaciente = @IdPaciente
        )
            THROW 56011, 'El paciente seleccionado no existe.', 1;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Empleados e
            INNER JOIN dbo.Usuarios u ON u.IdUsuario = e.IdUsuario
            INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
            WHERE e.IdEmpleado = @IdEmpleadoMedico
              AND r.NombreRol = N'Medico'
              AND u.Activo = 1
        )
            THROW 56012, 'El empleado seleccionado no es un medico activo.', 1;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Tipos_Examen
            WHERE IdTipoExamen = @IdTipoExamen
              AND Activo = 1
        )
            THROW 56013, 'El tipo de examen seleccionado no existe o esta inactivo.', 1;

        DECLARE @Estado NVARCHAR(50);
        SELECT @Estado = Descripcion
        FROM dbo.Estados_Examen
        WHERE IdEstadoExamen = @IdEstadoExamen;

        IF @Estado IS NULL
            THROW 56014, 'El estado seleccionado no existe.', 1;

        IF @Estado = N'Completado'
           AND NULLIF(LTRIM(RTRIM(@Resultado)), N'') IS NULL
            THROW 56015, 'Debe registrar el resultado para completar el examen.', 1;

        UPDATE dbo.Examenes_Medicos
        SET IdPaciente = @IdPaciente,
            IdEmpleadoMedico = @IdEmpleadoMedico,
            IdTipoExamen = @IdTipoExamen,
            FechaSolicitud = @FechaSolicitud,
            IdEstadoExamen = @IdEstadoExamen,
            Resultado = NULLIF(LTRIM(RTRIM(@Resultado)), N''),
            Observaciones = NULLIF(LTRIM(RTRIM(@Observaciones)), N''),
            FechaResultado =
                CASE
                    WHEN @Estado = N'Completado'
                        THEN COALESCE(FechaResultado, SYSDATETIME())
                    ELSE NULL
                END
        WHERE IdExamenMedico = @IdExamenMedico;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            ('sp_ExamenMedico_Actualizar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());

        THROW;
    END CATCH
END;
GO

/* ---- SP: ELIMINAR ---- */
CREATE OR ALTER PROCEDURE dbo.sp_ExamenMedico_Eliminar
    @IdExamenMedico INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS
        (
            SELECT 1 FROM dbo.Examenes_Medicos
            WHERE IdExamenMedico = @IdExamenMedico
        )
            THROW 56020, 'El examen medico no existe.', 1;

        DELETE FROM dbo.Examenes_Medicos
        WHERE IdExamenMedico = @IdExamenMedico;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            ('sp_ExamenMedico_Eliminar', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());

        THROW;
    END CATCH
END;
GO

/* ---- TRIGGER DE AUDITORIA ---- */
CREATE OR ALTER TRIGGER dbo.tr_ExamenesMedicos_Auditoria
ON dbo.Examenes_Medicos
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Accion NVARCHAR(50);

    SET @Accion =
        CASE
            WHEN EXISTS (SELECT 1 FROM inserted)
             AND EXISTS (SELECT 1 FROM deleted) THEN N'ACTUALIZAR'
            WHEN EXISTS (SELECT 1 FROM inserted) THEN N'CREAR'
            ELSE N'ELIMINAR'
        END;

    INSERT INTO dbo.Bitacora_Auditoria
    (
        Fecha,
        Accion,
        TablaAfectada,
        DetalleRegistroAntiguo,
        DetalleRegistroNuevo
    )
    VALUES
    (
        GETDATE(),
        @Accion,
        N'Examenes_Medicos',
        CASE
            WHEN EXISTS (SELECT 1 FROM deleted)
                THEN (SELECT * FROM deleted FOR JSON PATH)
            ELSE NULL
        END,
        CASE
            WHEN EXISTS (SELECT 1 FROM inserted)
                THEN (SELECT * FROM inserted FOR JSON PATH)
            ELSE NULL
        END
    );
END;
GO

/* ---- MATRIZ DE PERMISOS ---- */
DECLARE @IdRolAdministrador INT =
    (SELECT IdRol FROM dbo.Roles WHERE NombreRol = N'Administrador');
DECLARE @IdRolMedico INT =
    (SELECT IdRol FROM dbo.Roles WHERE NombreRol = N'Medico');
DECLARE @IdRolEnfermeria INT =
    (SELECT IdRol FROM dbo.Roles WHERE NombreRol = N'Enfermeria');

IF @IdRolAdministrador IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1 FROM dbo.Roles_Permisos
        WHERE IdRol = @IdRolAdministrador AND Modulo = N'ExamenesMedicos'
    )
        UPDATE dbo.Roles_Permisos
        SET PuedeVer = 1, PuedeCrear = 1, PuedeEditar = 1, PuedeEliminar = 1
        WHERE IdRol = @IdRolAdministrador AND Modulo = N'ExamenesMedicos';
    ELSE
        INSERT INTO dbo.Roles_Permisos
            (IdRol, Modulo, PuedeVer, PuedeCrear, PuedeEditar, PuedeEliminar)
        VALUES
            (@IdRolAdministrador, N'ExamenesMedicos', 1, 1, 1, 1);
END;

IF @IdRolMedico IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1 FROM dbo.Roles_Permisos
        WHERE IdRol = @IdRolMedico AND Modulo = N'ExamenesMedicos'
    )
        UPDATE dbo.Roles_Permisos
        SET PuedeVer = 1, PuedeCrear = 1, PuedeEditar = 1, PuedeEliminar = 0
        WHERE IdRol = @IdRolMedico AND Modulo = N'ExamenesMedicos';
    ELSE
        INSERT INTO dbo.Roles_Permisos
            (IdRol, Modulo, PuedeVer, PuedeCrear, PuedeEditar, PuedeEliminar)
        VALUES
            (@IdRolMedico, N'ExamenesMedicos', 1, 1, 1, 0);
END;

IF @IdRolEnfermeria IS NOT NULL
BEGIN
    IF EXISTS
    (
        SELECT 1 FROM dbo.Roles_Permisos
        WHERE IdRol = @IdRolEnfermeria AND Modulo = N'ExamenesMedicos'
    )
        UPDATE dbo.Roles_Permisos
        SET PuedeVer = 1, PuedeCrear = 0, PuedeEditar = 0, PuedeEliminar = 0
        WHERE IdRol = @IdRolEnfermeria AND Modulo = N'ExamenesMedicos';
    ELSE
        INSERT INTO dbo.Roles_Permisos
            (IdRol, Modulo, PuedeVer, PuedeCrear, PuedeEditar, PuedeEliminar)
        VALUES
            (@IdRolEnfermeria, N'ExamenesMedicos', 1, 0, 0, 0);
END;
GO
