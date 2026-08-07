USE [HospitalUTC_DB];
GO
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* ================================================================
   FIX rama "Paula": el codigo C# de esta rama (Empleados.Activo,
   sp_Empleado_CambiarEstado, modulo Medicos/Especialidades/Horarios)
   nunca traia el script SQL correspondiente -> todo fallaba en
   tiempo de ejecucion contra la base real. Este script agrega
   exactamente lo que el C# ya espera, sin tocar nada mas.
   ================================================================ */

/* ---- 1) Empleados.Activo ---- */
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Empleados') AND name = 'Activo')
    ALTER TABLE dbo.Empleados ADD Activo BIT NOT NULL CONSTRAINT DF_Empleados_Activo DEFAULT (1);
GO

CREATE OR ALTER VIEW [dbo].[vw_Empleados] AS
SELECT e.IdEmpleado, e.Identificacion, e.Nombre, e.Apellidos, e.Especialidad,
       e.SalarioPorHora, e.Activo, e.IdUsuario, u.Username AS UsuarioAsignado, e.IdPacienteVinculado
FROM dbo.Empleados e
LEFT JOIN dbo.Usuarios u ON u.IdUsuario = e.IdUsuario;
GO

/* @Activo se agrega como parametro NUEVO y OPCIONAL: no rompe llamadas viejas
   que aun no lo envien, y ya deja de fallar con "@Activo no es un parametro". */
CREATE OR ALTER PROCEDURE [dbo].[sp_Empleado_Crear]
    @Nombre NVARCHAR(100), @Apellidos NVARCHAR(100),
    @Especialidad NVARCHAR(100)=NULL, @SalarioPorHora DECIMAL(10,2), @IdUsuario INT,
    @IdPacienteVinculado INT=NULL, @Activo BIT=1, @IdGenerado INT OUTPUT
AS BEGIN SET NOCOUNT ON;
 BEGIN TRY BEGIN TRANSACTION;
   IF NOT EXISTS(SELECT 1 FROM dbo.Usuarios WHERE IdUsuario=@IdUsuario)
     THROW 55000, 'El usuario asignado no existe.', 1;

   DECLARE @Siguiente INT =
       (SELECT ISNULL(MAX(TRY_CAST(RIGHT(Identificacion, 3) AS INT)), 0) + 1
        FROM dbo.Empleados WHERE Identificacion LIKE 'EMP-%');
   DECLARE @Identificacion NVARCHAR(50) = 'EMP-' + RIGHT('000' + CAST(@Siguiente AS VARCHAR(3)), 3);

   INSERT INTO dbo.Empleados (Identificacion, Nombre, Apellidos, Especialidad, SalarioPorHora, IdUsuario, IdPacienteVinculado, Activo)
   VALUES (@Identificacion, @Nombre, @Apellidos, @Especialidad, @SalarioPorHora, @IdUsuario, @IdPacienteVinculado, @Activo);
   SET @IdGenerado = SCOPE_IDENTITY();
   COMMIT TRANSACTION;
 END TRY BEGIN CATCH
   IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
   INSERT INTO dbo.Bitacora_Errores (Procedimiento_Trigger,NumeroError,MensajeError,LineaError)
   VALUES ('sp_Empleado_Crear',ERROR_NUMBER(),ERROR_MESSAGE(),ERROR_LINE()); THROW;
 END CATCH END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Empleado_Actualizar]
    @IdEmpleado INT, @Identificacion NVARCHAR(50), @Nombre NVARCHAR(100), @Apellidos NVARCHAR(100),
    @Especialidad NVARCHAR(100)=NULL, @SalarioPorHora DECIMAL(10,2), @IdUsuario INT,
    @IdPacienteVinculado INT=NULL, @Activo BIT=1
AS BEGIN SET NOCOUNT ON;
 BEGIN TRY BEGIN TRANSACTION;
   UPDATE dbo.Empleados SET Identificacion=@Identificacion, Nombre=@Nombre, Apellidos=@Apellidos,
     Especialidad=@Especialidad, SalarioPorHora=@SalarioPorHora, IdUsuario=@IdUsuario,
     IdPacienteVinculado=@IdPacienteVinculado, Activo=@Activo
   WHERE IdEmpleado=@IdEmpleado;
   COMMIT TRANSACTION;
 END TRY BEGIN CATCH
   IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
   INSERT INTO dbo.Bitacora_Errores (Procedimiento_Trigger,NumeroError,MensajeError,LineaError)
   VALUES ('sp_Empleado_Actualizar',ERROR_NUMBER(),ERROR_MESSAGE(),ERROR_LINE()); THROW;
 END CATCH END
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_Empleado_CambiarEstado]
    @IdEmpleado INT, @Activo BIT
AS BEGIN SET NOCOUNT ON;
 BEGIN TRY BEGIN TRANSACTION;
   IF NOT EXISTS (SELECT 1 FROM dbo.Empleados WHERE IdEmpleado=@IdEmpleado)
     THROW 55010, 'El empleado no existe.', 1;
   UPDATE dbo.Empleados SET Activo=@Activo WHERE IdEmpleado=@IdEmpleado;
   COMMIT TRANSACTION;
 END TRY BEGIN CATCH
   IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
   INSERT INTO dbo.Bitacora_Errores (Procedimiento_Trigger,NumeroError,MensajeError,LineaError)
   VALUES ('sp_Empleado_CambiarEstado',ERROR_NUMBER(),ERROR_MESSAGE(),ERROR_LINE()); THROW;
 END CATCH END
GO

/* ---- 2) Especialidades (catalogo) ---- */
IF OBJECT_ID(N'dbo.Especialidades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Especialidades
    (
        IdEspecialidad INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Especialidades PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL CONSTRAINT UQ_Especialidades_Nombre UNIQUE
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Medicina General')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Medicina General');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Pediatria')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Pediatria');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Ginecologia')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Ginecologia');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Cardiologia')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Cardiologia');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Dermatologia')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Dermatologia');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Ortopedia')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Ortopedia');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Psiquiatria')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Psiquiatria');
IF NOT EXISTS (SELECT 1 FROM dbo.Especialidades WHERE Nombre = N'Cirugia General')
    INSERT INTO dbo.Especialidades (Nombre) VALUES (N'Cirugia General');
GO

/* ---- 3) Medicos (1 medico = 1 empleado; evita duplicados con UNIQUE) ---- */
IF OBJECT_ID(N'dbo.Medicos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Medicos
    (
        IdMedico INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Medicos PRIMARY KEY,
        IdEmpleado INT NOT NULL CONSTRAINT UQ_Medicos_IdEmpleado UNIQUE,
        IdEspecialidad INT NOT NULL,
        IdPacienteVinculado INT NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Medicos_Empleados')
    ALTER TABLE dbo.Medicos WITH CHECK
    ADD CONSTRAINT FK_Medicos_Empleados FOREIGN KEY (IdEmpleado) REFERENCES dbo.Empleados (IdEmpleado);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Medicos_Especialidades')
    ALTER TABLE dbo.Medicos WITH CHECK
    ADD CONSTRAINT FK_Medicos_Especialidades FOREIGN KEY (IdEspecialidad) REFERENCES dbo.Especialidades (IdEspecialidad);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Medicos_Pacientes')
    ALTER TABLE dbo.Medicos WITH CHECK
    ADD CONSTRAINT FK_Medicos_Pacientes FOREIGN KEY (IdPacienteVinculado) REFERENCES dbo.Pacientes (IdPaciente);
GO

/* ---- 4) Horarios_Medicos ---- */
IF OBJECT_ID(N'dbo.Horarios_Medicos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Horarios_Medicos
    (
        IdHorario INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Horarios_Medicos PRIMARY KEY,
        IdMedico INT NOT NULL,
        DiaSemana TINYINT NOT NULL CONSTRAINT CK_Horarios_DiaSemana CHECK (DiaSemana BETWEEN 1 AND 7),
        HoraInicio TIME NOT NULL,
        HoraFin TIME NOT NULL,
        CONSTRAINT CK_Horarios_Rango CHECK (HoraFin > HoraInicio)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_HorariosMedicos_Medicos')
    ALTER TABLE dbo.Horarios_Medicos WITH CHECK
    ADD CONSTRAINT FK_HorariosMedicos_Medicos FOREIGN KEY (IdMedico) REFERENCES dbo.Medicos (IdMedico);
GO

/* ---- 5) Auditoria (mismo patron que el resto del sistema) ---- */
CREATE OR ALTER TRIGGER dbo.tr_Medicos_Auditoria
ON dbo.Medicos
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Accion NVARCHAR(50) =
        CASE
            WHEN EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted) THEN N'UPDATE'
            WHEN EXISTS (SELECT 1 FROM inserted) THEN N'INSERT'
            ELSE N'DELETE'
        END;
    INSERT INTO dbo.Bitacora_Auditoria (Fecha, Accion, TablaAfectada, DetalleRegistroAntiguo, DetalleRegistroNuevo)
    VALUES (
        GETDATE(), @Accion, N'Medicos',
        CASE WHEN EXISTS (SELECT 1 FROM deleted) THEN (SELECT * FROM deleted FOR JSON PATH) ELSE NULL END,
        CASE WHEN EXISTS (SELECT 1 FROM inserted) THEN (SELECT * FROM inserted FOR JSON PATH) ELSE NULL END
    );
END;
GO

/* ---- 6) Verificacion ---- */
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Empleados' AND COLUMN_NAME='Activo';
SELECT name FROM sys.tables WHERE name IN ('Especialidades','Medicos','Horarios_Medicos');
SELECT name FROM sys.procedures WHERE name = 'sp_Empleado_CambiarEstado';
