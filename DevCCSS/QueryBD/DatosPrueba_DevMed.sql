USE [HospitalUTC_DB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/*
    DATOS DE PRUEBA PARA HOSPITAL DEVMED

    Requisitos:
    1. Ejecutar primero DevMedBD.sql.
    2. Ejecutar este archivo sobre HospitalUTC_DB.

    El script es reejecutable. Los registros de demostracion se identifican
    mediante usernames, identificaciones, nombres o fechas reservadas.

    Usuarios de prueba:
      demo.admin        Administrador
      demo.medico       Medico
      demo.enfermeria   Enfermeria
      demo.recepcion    Recepcionista
      demo.facturacion  Facturacion

    Clave para todos: DevMed2026*
    Estas credenciales son solo para desarrollo y demostracion.
*/

BEGIN TRY
    BEGIN TRANSACTION;

    /* ================================================================
       1. CATALOGOS
       ================================================================ */

    INSERT INTO dbo.Roles (NombreRol, Descripcion)
    SELECT v.NombreRol, v.Descripcion
    FROM (VALUES
        (N'Administrador', N'Acceso completo al sistema'),
        (N'Medico', N'Atencion clinica y expediente medico'),
        (N'Enfermeria', N'Apoyo clinico y hospitalizacion'),
        (N'Recepcionista', N'Registro de pacientes, citas y visitantes'),
        (N'Facturacion', N'Inventario, ventas y facturacion')
    ) v(NombreRol, Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Roles r WHERE r.NombreRol = v.NombreRol
    );

    INSERT INTO dbo.Tipos_Sangre (Descripcion)
    SELECT v.Descripcion
    FROM (VALUES ('A+'), ('A-'), ('B+'), ('B-'), ('AB+'), ('AB-'), ('O+'), ('O-')) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Tipos_Sangre t WHERE t.Descripcion = v.Descripcion
    );

    INSERT INTO dbo.Sexos_Biologicos (Descripcion)
    SELECT v.Descripcion
    FROM (VALUES (N'Masculino'), (N'Femenino'), (N'Intersexual'), (N'No indicado')) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Sexos_Biologicos s WHERE s.Descripcion = v.Descripcion
    );

    INSERT INTO dbo.Identidades_Genero (Descripcion)
    SELECT v.Descripcion
    FROM (VALUES (N'Masculino'), (N'Femenino'), (N'No binario'), (N'Prefiere no indicar')) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Identidades_Genero i WHERE i.Descripcion = v.Descripcion
    );

    INSERT INTO dbo.Tipos_Examen (Descripcion, Activo)
    SELECT v.Descripcion, CAST(1 AS BIT)
    FROM (VALUES
        (N'Hemograma completo'),
        (N'Examen general de orina'),
        (N'Glucosa en sangre'),
        (N'Rayos X de torax'),
        (N'Ultrasonido abdominal')
    ) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Tipos_Examen t WHERE t.Descripcion = v.Descripcion
    );

    INSERT INTO dbo.Estados_Examen (Descripcion)
    SELECT v.Descripcion
    FROM (VALUES (N'Solicitado'), (N'En proceso'), (N'Completado'), (N'Cancelado')) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Estados_Examen e WHERE e.Descripcion = v.Descripcion
    );

    INSERT INTO dbo.Tipos_Habitacion (Descripcion, Activo)
    SELECT v.Descripcion, CAST(1 AS BIT)
    FROM (VALUES (N'General'), (N'Individual'), (N'Cuidados intensivos'), (N'Maternidad')) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Tipos_Habitacion t WHERE t.Descripcion = v.Descripcion
    );

    INSERT INTO dbo.Estados_Habitacion (Descripcion)
    SELECT v.Descripcion
    FROM (VALUES (N'Disponible'), (N'Ocupada'), (N'Mantenimiento'), (N'Limpieza')) v(Descripcion)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Estados_Habitacion e WHERE e.Descripcion = v.Descripcion
    );

    /* ================================================================
       2. MATRIZ DE PERMISOS
       La matriz refleja los Authorize(Roles) actuales del proyecto Web.
       ================================================================ */

    DECLARE @PermisosDemo TABLE
    (
        NombreRol NVARCHAR(50),
        Modulo NVARCHAR(50),
        PuedeVer BIT,
        PuedeCrear BIT,
        PuedeEditar BIT,
        PuedeEliminar BIT
    );

    INSERT INTO @PermisosDemo VALUES
        (N'Administrador', N'Usuarios',             1, 1, 1, 1),
        (N'Administrador', N'Empleados',            1, 1, 1, 1),
        (N'Administrador', N'Pacientes',            1, 1, 1, 1),
        (N'Administrador', N'Citas',                1, 1, 1, 1),
        (N'Administrador', N'Medicamentos',         1, 1, 1, 1),
        (N'Administrador', N'Enfermedades',         1, 1, 1, 1),
        (N'Administrador', N'ExamenesMedicos',      1, 1, 1, 1),
        (N'Administrador', N'Habitaciones',         1, 1, 1, 1),
        (N'Administrador', N'Nacimientos',          1, 1, 1, 1),
        (N'Administrador', N'Inventario',           1, 1, 1, 1),
        (N'Administrador', N'Ventas',               1, 1, 1, 1),
        (N'Administrador', N'Visitantes',           1, 1, 1, 1),
        (N'Administrador', N'Bitacora',             1, 0, 0, 0),
        (N'Medico',        N'Pacientes',            1, 1, 1, 0),
        (N'Medico',        N'Citas',                1, 1, 1, 1),
        (N'Medico',        N'Medicamentos',         1, 1, 1, 1),
        (N'Medico',        N'Enfermedades',         1, 1, 1, 1),
        (N'Medico',        N'ExamenesMedicos',      1, 1, 1, 0),
        (N'Medico',        N'Habitaciones',         1, 0, 0, 0),
        (N'Medico',        N'Nacimientos',          1, 1, 1, 0),
        (N'Enfermeria',    N'Pacientes',            1, 1, 1, 0),
        (N'Enfermeria',    N'Citas',                1, 1, 1, 1),
        (N'Enfermeria',    N'Medicamentos',         1, 1, 1, 1),
        (N'Enfermeria',    N'Enfermedades',         1, 1, 1, 1),
        (N'Enfermeria',    N'ExamenesMedicos',      1, 0, 0, 0),
        (N'Enfermeria',    N'Habitaciones',         1, 1, 1, 0),
        (N'Enfermeria',    N'Nacimientos',          1, 1, 1, 0),
        (N'Recepcionista', N'Pacientes',            1, 1, 1, 0),
        (N'Recepcionista', N'Citas',                1, 1, 1, 1),
        (N'Recepcionista', N'Visitantes',           1, 1, 1, 1),
        (N'Facturacion',   N'Inventario',           1, 1, 1, 1),
        (N'Facturacion',   N'Ventas',               1, 1, 1, 1);

    INSERT INTO dbo.Roles_Permisos
        (IdRol, Modulo, PuedeVer, PuedeCrear, PuedeEditar, PuedeEliminar)
    SELECT r.IdRol, p.Modulo, p.PuedeVer, p.PuedeCrear, p.PuedeEditar, p.PuedeEliminar
    FROM @PermisosDemo p
    INNER JOIN dbo.Roles r ON r.NombreRol = p.NombreRol
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Roles_Permisos rp
        WHERE rp.IdRol = r.IdRol AND rp.Modulo = p.Modulo
    );

    /* ================================================================
       3. USUARIOS DE PRUEBA
       El hash usa el mismo algoritmo de sp_Usuario_Crear y PasswordVerifier.
       ================================================================ */

    DECLARE @ClaveDemo NVARCHAR(100) = N'DevMed2026*';
    DECLARE @UsuariosDemo TABLE
    (
        Nombre NVARCHAR(100),
        Username NVARCHAR(50),
        NombreRol NVARCHAR(50)
    );

    INSERT INTO @UsuariosDemo VALUES
        (N'Administrador Demo', N'demo.admin',       N'Administrador'),
        (N'Medico Demo',        N'demo.medico',      N'Medico'),
        (N'Enfermeria Demo',    N'demo.enfermeria',  N'Enfermeria'),
        (N'Recepcion Demo',      N'demo.recepcion',   N'Recepcionista'),
        (N'Facturacion Demo',    N'demo.facturacion', N'Facturacion');

    DECLARE @NombreUsuario NVARCHAR(100);
    DECLARE @Username NVARCHAR(50);
    DECLARE @NombreRol NVARCHAR(50);
    DECLARE @Salt VARBINARY(64);

    DECLARE UsuariosDemoCursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT Nombre, Username, NombreRol FROM @UsuariosDemo;

    OPEN UsuariosDemoCursor;
    FETCH NEXT FROM UsuariosDemoCursor INTO @NombreUsuario, @Username, @NombreRol;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Username = @Username)
        BEGIN
            SET @Salt = CRYPT_GEN_RANDOM(64);

            INSERT INTO dbo.Usuarios
                (Nombre, Username, PasswordHash, PasswordSalt, IdRol, Activo)
            SELECT
                @NombreUsuario,
                @Username,
                HASHBYTES('SHA2_512', @Salt + CONVERT(VARBINARY(200), @ClaveDemo)),
                @Salt,
                r.IdRol,
                1
            FROM dbo.Roles r
            WHERE r.NombreRol = @NombreRol;
        END;

        FETCH NEXT FROM UsuariosDemoCursor INTO @NombreUsuario, @Username, @NombreRol;
    END;

    CLOSE UsuariosDemoCursor;
    DEALLOCATE UsuariosDemoCursor;

    /* ================================================================
       4. EMPLEADOS
       ================================================================ */

    INSERT INTO dbo.Empleados
        (Identificacion, Nombre, Apellidos, Especialidad, SalarioPorHora, IdUsuario, IdPacienteVinculado)
    SELECT N'DEMO-EMP-001', N'Carlos', N'Vargas Mora', N'Medicina general', 18500.00, u.IdUsuario, NULL
    FROM dbo.Usuarios u
    WHERE u.Username = N'demo.medico'
      AND NOT EXISTS (SELECT 1 FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-001');

    INSERT INTO dbo.Empleados
        (Identificacion, Nombre, Apellidos, Especialidad, SalarioPorHora, IdUsuario, IdPacienteVinculado)
    SELECT N'DEMO-EMP-002', N'Laura', N'Jimenez Soto', N'Enfermeria general', 9800.00, u.IdUsuario, NULL
    FROM dbo.Usuarios u
    WHERE u.Username = N'demo.enfermeria'
      AND NOT EXISTS (SELECT 1 FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-002');

    INSERT INTO dbo.Empleados
        (Identificacion, Nombre, Apellidos, Especialidad, SalarioPorHora, IdUsuario, IdPacienteVinculado)
    SELECT N'DEMO-EMP-003', N'Andrea', N'Rojas Solano', N'Recepcion', 6500.00, u.IdUsuario, NULL
    FROM dbo.Usuarios u
    WHERE u.Username = N'demo.recepcion'
      AND NOT EXISTS (SELECT 1 FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-003');

    INSERT INTO dbo.Empleados
        (Identificacion, Nombre, Apellidos, Especialidad, SalarioPorHora, IdUsuario, IdPacienteVinculado)
    SELECT N'DEMO-EMP-004', N'Marco', N'Castro Ruiz', N'Facturacion', 7200.00, u.IdUsuario, NULL
    FROM dbo.Usuarios u
    WHERE u.Username = N'demo.facturacion'
      AND NOT EXISTS (SELECT 1 FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-004');

    /* ================================================================
       5. PACIENTES Y NACIMIENTO
       ================================================================ */

    INSERT INTO dbo.Pacientes
        (Identificacion, Nombre, Apellidos, FechaNacimiento, AntecedentesMedicos,
         EsRecienNacido, Peso, Estatura, Alergias, IdTipoSangre,
         IdSexoBiologico, IdIdentidadGenero, IdMadre)
    SELECT N'DEMO-PAC-001', N'Maria', N'Fernandez Lopez', '1992-05-14',
           N'Asma leve controlada', 0, 62.50, 165.00, 'Penicilina',
           ts.IdTipoSangre, sb.IdSexoBiologico, ig.IdIdentidadGenero, NULL
    FROM dbo.Tipos_Sangre ts
    CROSS JOIN dbo.Sexos_Biologicos sb
    CROSS JOIN dbo.Identidades_Genero ig
    WHERE ts.Descripcion = 'O+'
      AND sb.Descripcion = N'Femenino'
      AND ig.Descripcion = N'Femenino'
      AND NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE Identificacion = N'DEMO-PAC-001');

    INSERT INTO dbo.Pacientes
        (Identificacion, Nombre, Apellidos, FechaNacimiento, AntecedentesMedicos,
         EsRecienNacido, Peso, Estatura, Alergias, IdTipoSangre,
         IdSexoBiologico, IdIdentidadGenero, IdMadre)
    SELECT N'DEMO-PAC-002', N'Jose', N'Ramirez Arias', '1978-11-23',
           N'Hipertension arterial en seguimiento', 0, 84.30, 174.00, 'Ninguna conocida',
           ts.IdTipoSangre, sb.IdSexoBiologico, ig.IdIdentidadGenero, NULL
    FROM dbo.Tipos_Sangre ts
    CROSS JOIN dbo.Sexos_Biologicos sb
    CROSS JOIN dbo.Identidades_Genero ig
    WHERE ts.Descripcion = 'A+'
      AND sb.Descripcion = N'Masculino'
      AND ig.Descripcion = N'Masculino'
      AND NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE Identificacion = N'DEMO-PAC-002');

    INSERT INTO dbo.Pacientes
        (Identificacion, Nombre, Apellidos, FechaNacimiento, AntecedentesMedicos,
         EsRecienNacido, Peso, Estatura, Alergias, IdTipoSangre,
         IdSexoBiologico, IdIdentidadGenero, IdMadre)
    SELECT N'DEMO-PAC-003', N'Sofia', N'Fernandez Lopez', '2026-07-30',
           N'Recien nacida sin complicaciones', 1, 3.25, 49.50, 'Sin alergias conocidas',
           ts.IdTipoSangre, sb.IdSexoBiologico, ig.IdIdentidadGenero, madre.IdPaciente
    FROM dbo.Tipos_Sangre ts
    CROSS JOIN dbo.Sexos_Biologicos sb
    CROSS JOIN dbo.Identidades_Genero ig
    CROSS JOIN dbo.Pacientes madre
    WHERE ts.Descripcion = 'O+'
      AND sb.Descripcion = N'Femenino'
      AND ig.Descripcion = N'Femenino'
      AND madre.Identificacion = N'DEMO-PAC-001'
      AND NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE Identificacion = N'DEMO-PAC-003');

    DECLARE @IdRecienNacido INT =
        (SELECT IdPaciente FROM dbo.Pacientes WHERE Identificacion = N'DEMO-PAC-003');

    IF @IdRecienNacido IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Nacimientos_Apgar WHERE IdPaciente = @IdRecienNacido AND Minuto = 1)
        INSERT INTO dbo.Nacimientos_Apgar
            (IdPaciente, Minuto, Apariencia, Pulso, GestoRespuesta, Actividad, Respiracion)
        VALUES (@IdRecienNacido, 1, 2, 2, 2, 2, 1);

    IF @IdRecienNacido IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Nacimientos_Apgar WHERE IdPaciente = @IdRecienNacido AND Minuto = 5)
        INSERT INTO dbo.Nacimientos_Apgar
            (IdPaciente, Minuto, Apariencia, Pulso, GestoRespuesta, Actividad, Respiracion)
        VALUES (@IdRecienNacido, 5, 2, 2, 2, 2, 2);

    IF @IdRecienNacido IS NOT NULL
       AND NOT EXISTS (SELECT 1 FROM dbo.Nacimientos_ExamenFisico WHERE IdPaciente = @IdRecienNacido)
        INSERT INTO dbo.Nacimientos_ExamenFisico
            (IdPaciente, PerimetroCefalico, PerimetroToracico, Temperatura,
             FrecuenciaCardiaca, FrecuenciaRespiratoria, Reflejos, ColoracionPiel,
             ObservacionesGenerales)
        VALUES
            (@IdRecienNacido, 34.50, 33.00, 36.7, 138, 42,
             N'Reflejos neonatales presentes', N'Rosada', N'Examen fisico normal');

    IF @IdRecienNacido IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM dbo.Nacimientos_Tamizajes
           WHERE IdPaciente = @IdRecienNacido AND TipoTamizaje = N'Tamizaje metabolico')
        INSERT INTO dbo.Nacimientos_Tamizajes
            (IdPaciente, TipoTamizaje, Realizado, Resultado, FechaTamizaje)
        VALUES
            (@IdRecienNacido, N'Tamizaje metabolico', 1, N'Resultado dentro de parametros normales', '2026-07-31T08:30:00');

    IF @IdRecienNacido IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM dbo.Nacimientos_Tamizajes
           WHERE IdPaciente = @IdRecienNacido AND TipoTamizaje = N'Tamizaje auditivo')
        INSERT INTO dbo.Nacimientos_Tamizajes
            (IdPaciente, TipoTamizaje, Realizado, Resultado, FechaTamizaje)
        VALUES
            (@IdRecienNacido, N'Tamizaje auditivo', 1, N'Respuesta bilateral satisfactoria', '2026-07-31T09:00:00');

    /* ================================================================
       6. MEDICAMENTOS, ENFERMEDADES Y TRATAMIENTOS
       ================================================================ */

    INSERT INTO dbo.Medicamentos
        (Nombre, IndicacionesUso, Restricciones, HorasAplicacionRecomendada)
    SELECT v.Nombre, v.IndicacionesUso, v.Restricciones, v.Horas
    FROM (VALUES
        (N'Paracetamol DEMO', N'Dolor leve y fiebre', N'No exceder la dosis indicada', N'Cada 8 horas'),
        (N'Losartan DEMO', N'Control de presion arterial', N'Usar bajo supervision medica', N'08:00'),
        (N'Amoxicilina DEMO', N'Infecciones bacterianas sensibles', N'No usar en pacientes alergicos a penicilina', N'Cada 8 horas')
    ) v(Nombre, IndicacionesUso, Restricciones, Horas)
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Medicamentos m WHERE m.Nombre = v.Nombre);

    INSERT INTO dbo.Enfermedades
        (Nombre, Descripcion, RecomendacionesGenerales)
    SELECT v.Nombre, v.Descripcion, v.Recomendaciones
    FROM (VALUES
        (N'Hipertension arterial DEMO', N'Elevacion persistente de la presion arterial', N'Control periodico, dieta baja en sodio y actividad fisica'),
        (N'Infeccion respiratoria DEMO', N'Infeccion aguda de vias respiratorias', N'Hidratacion, reposo y seguimiento medico')
    ) v(Nombre, Descripcion, Recomendaciones)
    WHERE NOT EXISTS (SELECT 1 FROM dbo.Enfermedades e WHERE e.Nombre = v.Nombre);

    DECLARE @IdHipertension INT =
        (SELECT IdEnfermedad FROM dbo.Enfermedades WHERE Nombre = N'Hipertension arterial DEMO');
    DECLARE @IdLosartan INT =
        (SELECT IdMedicamento FROM dbo.Medicamentos WHERE Nombre = N'Losartan DEMO');

    IF @IdHipertension IS NOT NULL AND @IdLosartan IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM dbo.Tratamiento_Enfermedad
           WHERE IdEnfermedad = @IdHipertension AND IdMedicamento = @IdLosartan)
        INSERT INTO dbo.Tratamiento_Enfermedad
            (IdEnfermedad, IdMedicamento, ObservacionEspecifica)
        VALUES
            (@IdHipertension, @IdLosartan, N'Controlar presion arterial antes de cada ajuste');

    /* ================================================================
       7. CITAS Y EXPEDIENTE
       ================================================================ */

    DECLARE @IdPacienteMaria INT =
        (SELECT IdPaciente FROM dbo.Pacientes WHERE Identificacion = N'DEMO-PAC-001');
    DECLARE @IdPacienteJose INT =
        (SELECT IdPaciente FROM dbo.Pacientes WHERE Identificacion = N'DEMO-PAC-002');
    DECLARE @IdMedico INT =
        (SELECT IdEmpleado FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-001');
    DECLARE @IdEnfermeria INT =
        (SELECT IdEmpleado FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-002');
    DECLARE @IdFacturacion INT =
        (SELECT IdEmpleado FROM dbo.Empleados WHERE Identificacion = N'DEMO-EMP-004');

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Citas_Medicas
        WHERE IdPaciente = @IdPacienteMaria AND IdEmpleado_Medico = @IdMedico
          AND FechaHoraCita = '2026-08-03T09:00:00')
        INSERT INTO dbo.Citas_Medicas
            (IdPaciente, IdEmpleado_Medico, FechaHoraCita, EstadoCita,
             ResultadoConsulta, RequiereControl, PrioridadCita, EsCitaControl,
             FueReagendadaPorEmergencia)
        VALUES
            (@IdPacienteMaria, @IdMedico, '2026-08-03T09:00:00', N'Agendada',
             NULL, 0, N'Normal', 0, 0);

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Citas_Medicas
        WHERE IdPaciente = @IdPacienteJose AND IdEmpleado_Medico = @IdMedico
          AND FechaHoraCita = '2026-08-02T08:00:00')
        INSERT INTO dbo.Citas_Medicas
            (IdPaciente, IdEmpleado_Medico, FechaHoraCita, FechaHoraLlegada,
             TiempoEsperaMinutos, EstadoCita, ResultadoConsulta, RequiereControl,
             PrioridadCita, FechaHoraInicioAtencion, EsCitaControl,
             FueReagendadaPorEmergencia)
        VALUES
            (@IdPacienteJose, @IdMedico, '2026-08-02T08:00:00', '2026-08-02T07:55:00',
             10, N'Finalizada', N'Paciente estable. Se mantiene control de presion arterial.',
             1, N'Normal', '2026-08-02T08:05:00', 0, 0);

    DECLARE @IdCitaFinalizada INT =
        (SELECT IdCita FROM dbo.Citas_Medicas
         WHERE IdPaciente = @IdPacienteJose AND IdEmpleado_Medico = @IdMedico
           AND FechaHoraCita = '2026-08-02T08:00:00');

    IF @IdCitaFinalizada IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM dbo.Paciente_Enfermedades
           WHERE IdPaciente = @IdPacienteJose AND IdEnfermedad = @IdHipertension
             AND IdCita = @IdCitaFinalizada)
        INSERT INTO dbo.Paciente_Enfermedades
            (IdPaciente, IdEnfermedad, IdCita, FechaDiagnostico, Observaciones)
        VALUES
            (@IdPacienteJose, @IdHipertension, @IdCitaFinalizada,
             '2026-08-02T08:20:00', N'Diagnostico de control para demostracion');

    DECLARE @IdDiagnosticoDemo INT =
        (SELECT IdDiagnostico FROM dbo.Paciente_Enfermedades
         WHERE IdPaciente = @IdPacienteJose AND IdEnfermedad = @IdHipertension
           AND IdCita = @IdCitaFinalizada);

    IF @IdDiagnosticoDemo IS NOT NULL
       AND NOT EXISTS (
           SELECT 1 FROM dbo.Paciente_Medicamentos
           WHERE IdPaciente = @IdPacienteJose AND IdMedicamento = @IdLosartan
             AND IdDiagnostico = @IdDiagnosticoDemo)
        INSERT INTO dbo.Paciente_Medicamentos
            (IdPaciente, IdMedicamento, IdDiagnostico, FechaInicio, FechaFin,
             DosisIndicada, Observaciones)
        VALUES
            (@IdPacienteJose, @IdLosartan, @IdDiagnosticoDemo,
             '2026-08-02T08:30:00', NULL, N'50 mg una vez al dia',
             N'Controlar presion y asistir a cita de seguimiento');

    /* ================================================================
       8. EXAMENES MEDICOS
       ================================================================ */

    DECLARE @IdHemograma INT =
        (SELECT IdTipoExamen FROM dbo.Tipos_Examen WHERE Descripcion = N'Hemograma completo');
    DECLARE @IdRayosX INT =
        (SELECT IdTipoExamen FROM dbo.Tipos_Examen WHERE Descripcion = N'Rayos X de torax');
    DECLARE @EstadoSolicitado INT =
        (SELECT IdEstadoExamen FROM dbo.Estados_Examen WHERE Descripcion = N'Solicitado');
    DECLARE @EstadoEnProceso INT =
        (SELECT IdEstadoExamen FROM dbo.Estados_Examen WHERE Descripcion = N'En proceso');
    DECLARE @EstadoCompletado INT =
        (SELECT IdEstadoExamen FROM dbo.Estados_Examen WHERE Descripcion = N'Completado');

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Examenes_Medicos
        WHERE IdPaciente = @IdPacienteMaria AND IdTipoExamen = @IdHemograma
          AND FechaSolicitud = '2026-08-01T10:00:00')
        INSERT INTO dbo.Examenes_Medicos
            (IdPaciente, IdEmpleadoMedico, IdTipoExamen, FechaSolicitud,
             IdEstadoExamen, Resultado, Observaciones, FechaResultado)
        VALUES
            (@IdPacienteMaria, @IdMedico, @IdHemograma, '2026-08-01T10:00:00',
             @EstadoSolicitado, NULL, N'Muestra en ayunas', NULL);

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Examenes_Medicos
        WHERE IdPaciente = @IdPacienteJose AND IdTipoExamen = @IdRayosX
          AND FechaSolicitud = '2026-07-31T14:00:00')
        INSERT INTO dbo.Examenes_Medicos
            (IdPaciente, IdEmpleadoMedico, IdTipoExamen, FechaSolicitud,
             IdEstadoExamen, Resultado, Observaciones, FechaResultado)
        VALUES
            (@IdPacienteJose, @IdMedico, @IdRayosX, '2026-07-31T14:00:00',
             @EstadoEnProceso, NULL, N'Valorar molestias respiratorias', NULL);

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Examenes_Medicos
        WHERE IdPaciente = @IdPacienteJose AND IdTipoExamen = @IdHemograma
          AND FechaSolicitud = '2026-07-28T09:15:00')
        INSERT INTO dbo.Examenes_Medicos
            (IdPaciente, IdEmpleadoMedico, IdTipoExamen, FechaSolicitud,
             IdEstadoExamen, Resultado, Observaciones, FechaResultado)
        VALUES
            (@IdPacienteJose, @IdMedico, @IdHemograma, '2026-07-28T09:15:00',
             @EstadoCompletado,
             N'Valores hematologicos dentro de los rangos esperados.',
             N'Sin hallazgos relevantes', '2026-07-28T15:30:00');

    /* ================================================================
       9. HABITACIONES Y VISITANTES
       ================================================================ */

    DECLARE @TipoGeneral INT =
        (SELECT IdTipoHabitacion FROM dbo.Tipos_Habitacion WHERE Descripcion = N'General');
    DECLARE @TipoIndividual INT =
        (SELECT IdTipoHabitacion FROM dbo.Tipos_Habitacion WHERE Descripcion = N'Individual');
    DECLARE @TipoUci INT =
        (SELECT IdTipoHabitacion FROM dbo.Tipos_Habitacion WHERE Descripcion = N'Cuidados intensivos');
    DECLARE @HabitacionDisponible INT =
        (SELECT IdEstadoHabitacion FROM dbo.Estados_Habitacion WHERE Descripcion = N'Disponible');
    DECLARE @HabitacionOcupada INT =
        (SELECT IdEstadoHabitacion FROM dbo.Estados_Habitacion WHERE Descripcion = N'Ocupada');
    DECLARE @HabitacionMantenimiento INT =
        (SELECT IdEstadoHabitacion FROM dbo.Estados_Habitacion WHERE Descripcion = N'Mantenimiento');

    IF NOT EXISTS (SELECT 1 FROM dbo.Habitaciones WHERE NumeroHabitacion = N'DEMO-101')
        INSERT INTO dbo.Habitaciones
            (NumeroHabitacion, IdTipoHabitacion, IdEstadoHabitacion, IdPaciente,
             FechaIngreso, FechaSalida, IdEmpleadoResponsable)
        VALUES
            (N'DEMO-101', @TipoGeneral, @HabitacionDisponible, NULL, NULL, NULL, NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.Habitaciones WHERE NumeroHabitacion = N'DEMO-102')
        INSERT INTO dbo.Habitaciones
            (NumeroHabitacion, IdTipoHabitacion, IdEstadoHabitacion, IdPaciente,
             FechaIngreso, FechaSalida, IdEmpleadoResponsable)
        VALUES
            (N'DEMO-102', @TipoIndividual, @HabitacionOcupada, @IdPacienteMaria,
             '2026-08-01T16:00:00', NULL, @IdEnfermeria);

    IF NOT EXISTS (SELECT 1 FROM dbo.Habitaciones WHERE NumeroHabitacion = N'DEMO-103')
        INSERT INTO dbo.Habitaciones
            (NumeroHabitacion, IdTipoHabitacion, IdEstadoHabitacion, IdPaciente,
             FechaIngreso, FechaSalida, IdEmpleadoResponsable)
        VALUES
            (N'DEMO-103', @TipoUci, @HabitacionMantenimiento, NULL, NULL, NULL, NULL);

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Control_Visitantes
        WHERE Identificacion = N'DEMO-VIS-001'
          AND FechaHoraEntrada = '2026-08-02T14:00:00')
        INSERT INTO dbo.Control_Visitantes
            (Identificacion, NombreCompleto, IdPacienteAVisitar,
             FechaHoraEntrada, FechaHoraSalida)
        VALUES
            (N'DEMO-VIS-001', N'Roberto Fernandez Demo', @IdPacienteMaria,
             '2026-08-02T14:00:00', '2026-08-02T14:45:00');

    /* ================================================================
       10. INVENTARIO Y FACTURACION
       ================================================================ */

    INSERT INTO dbo.Inventario
        (NombreProducto, Descripcion, CantidadStock, PrecioUnitario, EsInsumoMedico)
    SELECT v.Nombre, v.Descripcion, v.Stock, v.Precio, v.EsInsumo
    FROM (VALUES
        (N'Guantes descartables DEMO', N'Caja de 100 unidades', 40, CAST(4500.00 AS DECIMAL(10,2)), CAST(1 AS BIT)),
        (N'Mascarilla quirurgica DEMO', N'Paquete de 50 unidades', 55, CAST(3200.00 AS DECIMAL(10,2)), CAST(1 AS BIT)),
        (N'Termometro digital DEMO', N'Termometro para uso general', 18, CAST(7800.00 AS DECIMAL(10,2)), CAST(1 AS BIT)),
        (N'Agua embotellada DEMO', N'Botella de 600 ml', 80, CAST(900.00 AS DECIMAL(10,2)), CAST(0 AS BIT))
    ) v(Nombre, Descripcion, Stock, Precio, EsInsumo)
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Inventario i WHERE i.NombreProducto = v.Nombre
    );

    DECLARE @IdProductoGuantes INT =
        (SELECT TOP (1) IdProducto FROM dbo.Inventario WHERE NombreProducto = N'Guantes descartables DEMO');
    DECLARE @IdProductoTermometro INT =
        (SELECT TOP (1) IdProducto FROM dbo.Inventario WHERE NombreProducto = N'Termometro digital DEMO');
    DECLARE @IdFacturaDemo INT;

    SELECT @IdFacturaDemo = IdFactura
    FROM dbo.Ventas_Facturacion
    WHERE IdPaciente = @IdPacienteJose
      AND IdEmpleadoAtiende = @IdFacturacion
      AND FechaFactura = '2026-08-02T11:30:00';

    IF @IdFacturaDemo IS NULL
    BEGIN
        INSERT INTO dbo.Ventas_Facturacion
            (IdPaciente, FechaFactura, Subtotal, Impuestos, Total, Descuento,
             MetodoPago, IdEmpleadoAtiende)
        VALUES
            (@IdPacienteJose, '2026-08-02T11:30:00', 12300.00, 1599.00,
             13899.00, 0.00, N'Tarjeta', @IdFacturacion);

        SET @IdFacturaDemo = SCOPE_IDENTITY();

        INSERT INTO dbo.Detalles_Factura
            (IdFactura, IdProducto, Cantidad, PrecioUnitario, SubtotalLinea)
        VALUES
            (@IdFacturaDemo, @IdProductoGuantes, 1, 4500.00, 4500.00),
            (@IdFacturaDemo, @IdProductoTermometro, 1, 7800.00, 7800.00);
    END;

    COMMIT TRANSACTION;

    PRINT N'Datos de prueba cargados correctamente.';
    PRINT N'Usuarios: demo.admin, demo.medico, demo.enfermeria, demo.recepcion, demo.facturacion';
    PRINT N'Clave comun: DevMed2026*';

    SELECT N'Usuarios demo' AS Grupo, COUNT(*) AS Cantidad
    FROM dbo.Usuarios WHERE Username LIKE N'demo.%'
    UNION ALL
    SELECT N'Pacientes demo', COUNT(*)
    FROM dbo.Pacientes WHERE Identificacion LIKE N'DEMO-PAC-%'
    UNION ALL
    SELECT N'Empleados demo', COUNT(*)
    FROM dbo.Empleados WHERE Identificacion LIKE N'DEMO-EMP-%'
    UNION ALL
    SELECT N'Examenes demo', COUNT(*)
    FROM dbo.Examenes_Medicos
    WHERE FechaSolicitud IN ('2026-08-01T10:00:00', '2026-07-31T14:00:00', '2026-07-28T09:15:00')
    UNION ALL
    SELECT N'Habitaciones demo', COUNT(*)
    FROM dbo.Habitaciones WHERE NumeroHabitacion LIKE N'DEMO-%';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    IF OBJECT_ID(N'dbo.Bitacora_Errores', N'U') IS NOT NULL
    BEGIN
        INSERT INTO dbo.Bitacora_Errores
            (Procedimiento_Trigger, NumeroError, MensajeError, LineaError)
        VALUES
            (N'DatosPrueba_DevMed', ERROR_NUMBER(), ERROR_MESSAGE(), ERROR_LINE());
    END;

    THROW;
END CATCH;
GO
