/*
    Actividad: Tablas, procedimientos almacenados y API con Postman
    Alumno:    Kevin Avendaño Ramírez
    Tabla:     Empleado

    El script se puede ejecutar varias veces: crea la base y la tabla solo si
    no existen, recrea los procedimientos y solo inserta los datos de prueba
    si la tabla está vacía.
*/

IF DB_ID('PRACTICA_EMPLEADO') IS NULL
    CREATE DATABASE PRACTICA_EMPLEADO
GO

USE [PRACTICA_EMPLEADO]
GO

/****** 1. Tabla ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Empleado', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Empleado](
        -- Campos comunes
        [IdEmpleado]         INT IDENTITY(1,1) NOT NULL,
        -- Campos específicos
        [NumeroEmpleado]     VARCHAR(20)    NOT NULL,   -- texto: es un código, no se opera
        [Nombre]             NVARCHAR(100)  NOT NULL,   -- NVARCHAR para acentos y ñ
        [Apellidos]          NVARCHAR(150)  NOT NULL,
        [Correo]             VARCHAR(150)   NOT NULL,
        [Puesto]             NVARCHAR(100)  NOT NULL,
        [Sueldo]             DECIMAL(10, 2) NOT NULL,
        -- Campos comunes
        [Activo]             BIT            NOT NULL CONSTRAINT DF_Empleado_Activo        DEFAULT (1),
        [FechaRegistro]      DATETIME2      NOT NULL CONSTRAINT DF_Empleado_FechaRegistro DEFAULT (SYSDATETIME()),
        [FechaActualizacion] DATETIME2      NULL,

        CONSTRAINT [PK_Empleado] PRIMARY KEY CLUSTERED ([IdEmpleado] ASC),
        CONSTRAINT [UQ_Empleado_NumeroEmpleado] UNIQUE ([NumeroEmpleado]),
        CONSTRAINT [CK_Empleado_Sueldo] CHECK ([Sueldo] > 0)
    ) ON [PRIMARY]
END
GO

/****** 2. Procedimientos almacenados ******/

/****** Object:  StoredProcedure [dbo].[Empleado_Insertar] ******/
CREATE OR ALTER PROCEDURE [dbo].[Empleado_Insertar](
@NumeroEmpleado varchar(20),
@Nombre nvarchar(100),
@Apellidos nvarchar(150),
@Correo varchar(150),
@Puesto nvarchar(100),
@Sueldo decimal(10, 2)
)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO dbo.Empleado
    (
        NumeroEmpleado,
        Nombre,
        Apellidos,
        Correo,
        Puesto,
        Sueldo
    )
    VALUES
    (
        @NumeroEmpleado,
        @Nombre,
        @Apellidos,
        @Correo,
        @Puesto,
        @Sueldo
    )
    -- Activo y FechaRegistro los pone la tabla con sus valores por defecto

    -- Regresa el Id generado para que la API lo devuelva
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdEmpleado
END
GO

/****** Object:  StoredProcedure [dbo].[Empleado_Actualizar] ******/
CREATE OR ALTER PROCEDURE [dbo].[Empleado_Actualizar](
@IdEmpleado int,
@NumeroEmpleado varchar(20),
@Nombre nvarchar(100),
@Apellidos nvarchar(150),
@Correo varchar(150),
@Puesto nvarchar(100),
@Sueldo decimal(10, 2)
)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE dbo.Empleado
    SET
        NumeroEmpleado     = @NumeroEmpleado,
        Nombre             = @Nombre,
        Apellidos          = @Apellidos,
        Correo             = @Correo,
        Puesto             = @Puesto,
        Sueldo             = @Sueldo,
        FechaActualizacion = SYSDATETIME()
    WHERE IdEmpleado = @IdEmpleado
      AND Activo = 1          -- un empleado dado de baja ya no se edita

    -- 1 = se actualizó, 0 = no existe o está dado de baja
    SELECT @@ROWCOUNT AS FilasAfectadas
END
GO

/****** Object:  StoredProcedure [dbo].[Empleado_Eliminar] ******/
-- Baja lógica: no borra la fila, solo la marca como inactiva
CREATE OR ALTER PROCEDURE [dbo].[Empleado_Eliminar](
@IdEmpleado int
)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE dbo.Empleado
    SET
        Activo             = 0,
        FechaActualizacion = SYSDATETIME()
    WHERE IdEmpleado = @IdEmpleado
      AND Activo = 1

    SELECT @@ROWCOUNT AS FilasAfectadas
END
GO

/****** Object:  StoredProcedure [dbo].[Empleado_ConsultarPorId] ******/
-- Devuelve el registro aunque esté dado de baja: así se ve Activo = 0
CREATE OR ALTER PROCEDURE [dbo].[Empleado_ConsultarPorId](
@IdEmpleado int
)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        IdEmpleado,
        NumeroEmpleado,
        Nombre,
        Apellidos,
        Correo,
        Puesto,
        Sueldo,
        Activo,
        FechaRegistro,
        FechaActualizacion
    FROM dbo.Empleado
    WHERE IdEmpleado = @IdEmpleado
END
GO

/****** Object:  StoredProcedure [dbo].[Empleado_ConsultarTodos] ******/
CREATE OR ALTER PROCEDURE [dbo].[Empleado_ConsultarTodos]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        IdEmpleado,
        NumeroEmpleado,
        Nombre,
        Apellidos,
        Correo,
        Puesto,
        Sueldo,
        Activo,
        FechaRegistro,
        FechaActualizacion
    FROM dbo.Empleado
    WHERE Activo = 1
    ORDER BY IdEmpleado
END
GO

/****** 3. Datos de prueba (10 registros) ******/
IF NOT EXISTS (SELECT 1 FROM dbo.Empleado)
BEGIN
    INSERT INTO dbo.Empleado (NumeroEmpleado, Nombre, Apellidos, Correo, Puesto, Sueldo) VALUES
    ('EMP-0001', N'María',     N'Méndez Ruiz',       'maria.mendez@empresa.com',     N'Gerente de Ventas',        28500.00),
    ('EMP-0002', N'José',      N'Hernández López',   'jose.hernandez@empresa.com',   N'Desarrollador Backend',    24000.00),
    ('EMP-0003', N'Ana',       N'García Torres',     'ana.garcia@empresa.com',       N'Diseñadora UX',            21000.00),
    ('EMP-0004', N'Luis',      N'Martínez Pérez',    'luis.martinez@empresa.com',    N'Contador',                 19500.00),
    ('EMP-0005', N'Fernanda',  N'Núñez Castillo',    'fernanda.nunez@empresa.com',   N'Recursos Humanos',         18000.00),
    ('EMP-0006', N'Carlos',    N'Ramírez Ortega',    'carlos.ramirez@empresa.com',   N'Soporte Técnico',          14500.00),
    ('EMP-0007', N'Sofía',     N'Jiménez Vargas',    'sofia.jimenez@empresa.com',    N'Analista de Datos',        23000.00),
    ('EMP-0008', N'Diego',     N'Peña Morales',      'diego.pena@empresa.com',       N'Almacenista',              11500.00),
    ('EMP-0009', N'Valeria',   N'Cruz Domínguez',    'valeria.cruz@empresa.com',     N'Ejecutiva de Cuenta',      17500.00),
    ('EMP-0010', N'Ricardo',   N'Flores Sánchez',    'ricardo.flores@empresa.com',   N'Administrador de Sistemas', 26000.00)
END
GO