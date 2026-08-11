USE [HospitalUTC_DB];
GO

/* ================================================================
   Bitacora_Auditoria ya no se purga automaticamente: se conserva
   el historial completo de auditoria de forma indefinida. El limite
   de 100 registros ahora es solo de PRESENTACION (la pantalla de
   Auditoria muestra unicamente los 100 mas recientes), no de
   almacenamiento -- todo el historial sigue en la tabla y queda
   disponible via consulta directa si se necesita.
   Script idempotente.
   ================================================================ */
IF EXISTS (SELECT 1 FROM sys.triggers WHERE name = 'trg_Bitacora_Auditoria_Purga')
    DROP TRIGGER dbo.trg_Bitacora_Auditoria_Purga;
GO
