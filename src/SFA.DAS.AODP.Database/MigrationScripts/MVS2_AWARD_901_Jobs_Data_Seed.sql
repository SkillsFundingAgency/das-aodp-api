BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS2_AWARD_901_Jobs_Data_Seed NVARCHAR(100) = 'MVS2_AWARD_901_Jobs_Data_Seed'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS2_AWARD_901_Jobs_Data_Seed)
        BEGIN

            PRINT @ScriptName_MVS2_AWARD_901_Jobs_Data_Seed + ' has already been run.'

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
           ('00000000-0000-0000-0000-000000000003'
           ,'Pldns'
           ,1
           ,'Initial'
           ,null)

        INSERT INTO [dbo].[Jobs]
           ([Id]
           ,[Name]
           ,[Enabled]
           ,[Status]
           ,[LastRunTime])
        VALUES
           ('00000000-0000-0000-0000-000000000004'
           ,'DefundingList'
           ,1
           ,'Initial'
           ,null)
                    
        INSERT INTO [dbo].[JobConfigurations]
           ([Id]
           ,[Name]
           ,[Value]
           ,[JobId])
        VALUES
           ('00000000-0000-0000-0001-000000000005'
           ,'ImportPldns'
           ,'true'
           ,'00000000-0000-0000-0000-000000000003')

        INSERT INTO [dbo].[JobConfigurations]
           ([Id]
           ,[Name]
           ,[Value]
           ,[JobId])
        VALUES
           ('00000000-0000-0000-0001-000000000006'
           ,'DefundingList'
           ,'true'
           ,'00000000-0000-0000-0000-000000000004')


        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS2_AWARD_901_Jobs_Data_Seed,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH