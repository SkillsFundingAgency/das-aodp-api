-- =============================================
-- Author:		Robert Rybnikar
-- Create date: 07/03/2025
-- Description:	Truncate data from funded.Qualifications table
-- =============================================
CREATE PROCEDURE [dbo].Truncate_Funded_Qualifications	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE
	@ErrorMessage NVARCHAR(4000),
	@ErrorSeverity INT,
	@ErrorNumber INT,
	@ProcedureName SYSNAME = '[dbo].[Truncate_Funded_Qualifications]'

	BEGIN TRY
		
		ALTER TABLE [funded].[Qualifications] DROP CONSTRAINT [FK_Qualification]		
		ALTER TABLE [funded].[Qualifications] DROP CONSTRAINT [FK_AwardingOrganisation]		
		ALTER TABLE [funded].[QualificationOffers] DROP CONSTRAINT [FK_Qualifications]

		TRUNCATE TABLE [funded].[QualificationOffers]
		TRUNCATE TABLE [funded].[Qualifications]

		ALTER TABLE [funded].[QualificationOffers]  WITH CHECK ADD  CONSTRAINT [FK_Qualifications] FOREIGN KEY([QualificationId]) REFERENCES [funded].[Qualifications] ([Id])		
		ALTER TABLE [funded].[QualificationOffers] CHECK CONSTRAINT [FK_Qualifications]
		
		ALTER TABLE [funded].[Qualifications]  WITH CHECK ADD  CONSTRAINT [FK_AwardingOrganisation] FOREIGN KEY([AwardingOrganisationId]) REFERENCES [dbo].[AwardingOrganisation] ([Id])		
		ALTER TABLE [funded].[Qualifications] CHECK CONSTRAINT [FK_AwardingOrganisation]
		
		ALTER TABLE [funded].[Qualifications]  WITH CHECK ADD  CONSTRAINT [FK_Qualification] FOREIGN KEY([QualificationId]) REFERENCES [dbo].[Qualification] ([Id])		
		ALTER TABLE [funded].[Qualifications] CHECK CONSTRAINT [FK_Qualification]
		
		
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

