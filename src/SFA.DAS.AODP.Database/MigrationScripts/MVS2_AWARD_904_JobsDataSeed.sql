BEGIN TRY
BEGIN TRANSACTION

    DECLARE @ScriptName_MVS2_AWARD_904_JobsDataSeed NVARCHAR(100) = 'MVS2_AWARD_904_JobsDataSeed'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS2_AWARD_904_JobsDataSeed)
BEGIN

            PRINT @ScriptName_MVS2_AWARD_904_JobsDataSeed + ' has already been run.'

END
ELSE

BEGIN

INSERT INTO [dbo].[Jobs]
([Id]
    ,[Name]
    ,[Enabled]
    ,[Status]
    ,[LastRunTime])
VALUES
    ('00000000-0000-0000-0000-000000000005'
        ,'QaaQualifications'
        ,1
        ,'Initial'
        ,null)

INSERT INTO [dbo].[JobConfigurations]
([Id]
    ,[Name]
    ,[Value]
    ,[JobId])
VALUES
    ('00000000-0000-0000-0000-000000000005'
        ,'ApiImport'
        ,'true'
        ,'00000000-0000-0000-0000-000000000005')

-- Record the script as run
INSERT INTO [dbo].[MigrationScripts]
(
    [Name],
[RunDate]
)
VALUES
    (
    @ScriptName_MVS2_AWARD_904_JobsDataSeed,
    GETDATE()
    )
END

COMMIT TRANSACTION

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH