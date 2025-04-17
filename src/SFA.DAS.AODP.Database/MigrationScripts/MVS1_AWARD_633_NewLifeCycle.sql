BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_AWARD_633_NewLifeCycle NVARCHAR(100) = 'MVS1_AWARD_633_NewLifeCycle'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_AWARD_633_NewLifeCycle)
        BEGIN

            PRINT @ScriptName_MVS1_AWARD_633_NewLifeCycle + ' has already been run.'

        END
    ELSE

    BEGIN        
              
       INSERT INTO [regulated].[LifecycleStage]
                   ([Id]
                   ,[Name])
             VALUES
                   ('00000000-0000-0000-0000-000000000003','Completed')		     		          

        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_AWARD_633_NewLifeCycle,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH