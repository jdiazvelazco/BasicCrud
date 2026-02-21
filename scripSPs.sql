

------------------------------------------------------------------
-- spAgregar on tblExamen
-- Creación: JDV 20.02.2026
-- Modificación: JDV 20.02.2026
------------------------------------------------------------------
CREATE PROCEDURE dbo.spAgregar
	@Id int,
	@Nombre varchar(255),
	@Descripcion varchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		INSERT INTO tblExamen
				(IdExamen, 
				Nombre, 
				Descripcion)
				
		VALUES (@Id, 
				@Nombre, 
				@Descripcion);
				
        SELECT 0 AS CodigoRetorno, 
			'Registro insertado satisfactoriamente' AS DescripconRetorno;
			
    END TRY
    BEGIN CATCH
        SELECT ERROR_NUMBER() AS CodigoRetorno, 
               ERROR_MESSAGE() AS DescripconRetorno;
    END CATCH
 
END;

GO
------------------------------------------------------------------
-- spActualizar on tblExamen
-- Creación: JDV 20.02.2026
-- Modificación: JDV 20.02.2026
------------------------------------------------------------------
CREATE PROCEDURE dbo.spActualizar
    @Id INT,
    @Nombre VARCHAR(255),
    @Descripcion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE tblExamen
        SET Nombre = @Nombre,
            Descripcion = @Descripcion
        WHERE idExamen = @Id;

        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 1 AS CodigoRetorno, 
				'No se encontró el registro con el Id proporcionado' AS DescripconRetorno;
        END
        ELSE
        BEGIN
            SELECT 0 AS CodigoRetorno, 
				'Registro actualizado satisfactoriamente' AS DescripconRetorno;
        END
    END TRY
    BEGIN CATCH
        SELECT ERROR_NUMBER() AS CodigoRetorno, 
               ERROR_MESSAGE() AS DescripconRetorno;
    END CATCH
END;
GO

------------------------------------------------------------------
-- spEliminar on tblExamen
-- Creación: JDV 20.02.2026
-- Modificación: JDV 20.02.2026
------------------------------------------------------------------
CREATE PROCEDURE dbo.spEliminar
	@Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
		DELETE FROM tblExamen
		WHERE idExamen = @Id;
		IF @@ROWCOUNT = 0
		BEGIN
			SELECT 1 AS CodigoRetorno,
				'No se encontró el registro' AS DescripconRetorno
		END
		ELSE
		BEGIN
			SELECT 0 AS CodigoRetorno,
				'Registro eliminado correctamente' AS DescripconRetorno
		END
	END TRY
    BEGIN CATCH
        SELECT ERROR_NUMBER() AS CodigoRetorno, 
               ERROR_MESSAGE() AS DescripconRetorno;
    END CATCH
END;

GO

------------------------------------------------------------------
-- spConsultar on tblExamen
-- Creación: JDV 20.02.2026
-- Modificación: JDV 20.02.2026
------------------------------------------------------------------
CREATE PROCEDURE dbo.spConsultar
	@Id int = NULL,
    @Nombre VARCHAR(255) = NULL,
    @Descripcion VARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
	SELECT idExamen, Nombre, DEscripcion
	FROM tblExamen
	WHERE (@Id is null Or idExamen = @Id)
		AND	(@Nombre is null Or Nombre Like '%' + @Nombre + '%')
		AND (@Descripcion is null Or Descripcion Like  '%' + @Descripcion + '%' );   
END;

GO