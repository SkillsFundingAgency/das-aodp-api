BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_AWARD452_Jobs NVARCHAR(100) = 'MVS1_AWARD452_Jobs'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_AWARD452_Jobs)
        BEGIN

            PRINT @ScriptName_MVS1_AWARD452_Jobs + ' has already been run.'

        END
    ELSE

    BEGIN                                          
        INSERT INTO [dbo].[JobConfigurations]
           ([Id]
           ,[Name]
           ,[Value]
           ,[JobId])
        VALUES
           ('00000000-0000-0000-0001-000000000003'
           ,'ImportFundedCsv'
           ,'true'
           ,'00000000-0000-0000-0000-000000000002')

        INSERT INTO [dbo].[JobConfigurations]
           ([Id]
           ,[Name]
           ,[Value]
           ,[JobId])
        VALUES
           ('00000000-0000-0000-0001-000000000004'
           ,'ImportArchivedCsv'
           ,'true'
           ,'00000000-0000-0000-0000-000000000002')


        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_AWARD452_Jobs,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH