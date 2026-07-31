BEGIN TRY
BEGIN TRANSACTION

    DECLARE @ScriptName_MVS2_AWARD_RolloverCandidatesJobDataSeed NVARCHAR(100) = 'MVS2_AWARD_RolloverCandidatesJobDataSeed'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS2_AWARD_RolloverCandidatesJobDataSeed)
BEGIN

            PRINT @ScriptName_MVS2_AWARD_RolloverCandidatesJobDataSeed + ' has already been run.'

END
ELSE

BEGIN

IF NOT EXISTS (SELECT 1 FROM [dbo].[Jobs] WHERE [Name] = 'RolloverCandidates')
BEGIN
    INSERT INTO [dbo].[Jobs]
    ([Id]
        ,[Name]
        ,[Enabled]
        ,[Status]
        ,[LastRunTime])
    VALUES
        ('00000000-0000-0000-0000-000000000006'
            ,'RolloverCandidates'
            ,1
            ,'Initial'
            ,null)
END

-- Record the script as run
INSERT INTO [dbo].[MigrationScripts]
(
    [Name],
[RunDate]
)
VALUES
    (
    @ScriptName_MVS2_AWARD_RolloverCandidatesJobDataSeed,
    GETDATE()
    )
END

COMMIT TRANSACTION

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH
