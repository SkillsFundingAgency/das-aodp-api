-- =============================================
-- Author:		Robert Rybnikar
-- Create date: 07/03/2025
-- Description:	Truncate data from QualificationImportStaging table
-- =============================================
CREATE PROCEDURE [dbo].[Truncate_QualificationImportStaging]	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE
	@ErrorMessage NVARCHAR(4000),
	@ErrorSeverity INT,
	@ErrorNumber INT,
	@ProcedureName SYSNAME = '[dbo].[Truncate_QualificationImportStaging]'

	BEGIN TRY
		truncate table dbo.QualificationImportStaging;
	END TRY
	BEGIN CATCH
		SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorNumber = ERROR_NUMBER()
		SELECT @ErrorMessage = '[' + @ProcedureName + '] - ' + @ErrorMessage
		GOTO ErrorHandler
	END CATCH

	GOTO CleanExit

	ErrorHandler:
		BEGIN
			SET @ErrorMessage = 'ERROR: ' + CONVERT(varchar, @ErrorNumber) + '-' + @ErrorMessage

			RAISERROR
			(
				@ErrorMessage,
				@ErrorSeverity,
				1
			)

			RETURN
		END

	CleanExit:
		BEGIN
			RETURN
		END
END
GO
