CREATE procedure [dbo].[proc_CompleteOutputProcess] 

/*##################################################################################################
	-Name:				CompleteOutputProcess
	-Description:		Mark quals that are approved/rejected as 'Completed' to close off review cycle 
						
	-Date of Creation:	16/04/2025
	-Created By:		Robert Rybnikar (Fujitsu)
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Robert Rybnikar	16/04/2025			Original
##################################################################################################*/
As
BEGIN

SET NOCOUNT ON;
	DECLARE
	@ErrorMessage NVARCHAR(4000),
	@ErrorSeverity INT,
	@ErrorNumber INT,
	@ProcedureName SYSNAME = '[dbo].[proc_CompleteOutputProcess]'

BEGIN TRY

	BEGIN TRANSACTION;

	WITH LatestQualificationSet AS (
		SELECT
			ver.Id,
			ver.QualificationId,        
			ROW_NUMBER() OVER (PARTITION BY ver.QualificationId ORDER BY ver.Version DESC) AS rn
		FROM regulated.QualificationVersions ver
		WHERE (ver.ProcessStatusId = '00000000-0000-0000-0000-000000000004' --approved
			OR ver.ProcessStatusId = '00000000-0000-0000-0000-000000000005') --rejected
		  AND ver.LifecycleStageId <> '00000000-0000-0000-0000-000000000003' --Completed
	)
	SELECT *
	INTO #LatestQualifications
	FROM
	(
	SELECT *
	FROM LatestQualificationSet
	WHERE rn = 1) as x;

	Declare @VersionId UniqueIdentifier
	Declare @QualId UniqueIdentifier
	Declare @timestamp DateTime = GETDATE()
	Declare @count INT

	select @count = count(*) from #LatestQualifications;
	IF (@count > 0)
	BEGIN
		PRINT 'FOUND ' + CAST(@count AS VARCHAR) + ' qualifications to mark as completed:'
		WHILE EXISTS(SELECT * FROM #LatestQualifications)
		Begin

			Select Top 1 @VersionId = Id, @QualId = QualificationId From #LatestQualifications;

			UPDATE [regulated].[QualificationVersions] SET LifecycleStageId = '00000000-0000-0000-0000-000000000003'
			WHERE Id = @VersionId;

			INSERT INTO [dbo].[QualificationDiscussionHistory]
				   ([Id]
				   ,[QualificationId]
				   ,[ActionTypeId]
				   ,[UserDisplayName]
				   ,[Notes]
				   ,[Timestamp]
				   ,[Title])
			 VALUES
				   (NEWID()
				   ,@QualId
				   ,'00000000-0000-0000-0000-000000000001' -- No Action Required
				   ,'OfqualExport'
				   ,'Qualification Exported and marked as completed'
				   ,@timestamp
				   ,'Updated status to: Completed')

			PRINT CAST(@VersionId AS nvarchar(36))
    
			Delete #LatestQualifications Where Id = @VersionId;

		End
	END
	ELSE
		PRINT 'No quals found that are approved/rejected to update as completed';

	COMMIT TRANSACTION;

END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
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
