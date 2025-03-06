BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_Qual_Ref_Data_award_185 NVARCHAR(100) = 'MVS1_Qual_Ref_Data_award_185'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_Qual_Ref_Data_award_185)
        BEGIN

            PRINT @ScriptName_MVS1_Qual_Ref_Data_award_185 + ' has already been run.'

        END
    ELSE

    BEGIN        
       
        UPDATE [regulated].[ProcessStatus] SET [Name] = 'Decision Required' WHERE [Name] = 'Decision Needed'
        UPDATE [regulated].[ProcessStatus] SET [Name] = 'On Hold' WHERE [Name] = 'Hold'

        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_Qual_Ref_Data_award_185,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH