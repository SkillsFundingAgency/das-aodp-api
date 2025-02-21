BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_Jobs_Data_Seed NVARCHAR(100) = 'MVS1_Jobs_Data_Seed'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_Jobs_Data_Seed)
        BEGIN

            PRINT @ScriptName_MVS1_Jobs_Data_Seed + ' has already been run.'

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
           ('00000000-0000-0000-0000-000000000001'
           ,'RegulatedQualifications'
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
           ('00000000-0000-0000-0000-000000000002'
           ,'FundedQualifications'
           ,1
           ,'Initial'
           ,null)
                    
        INSERT INTO [dbo].[JobConfigurations]
           ([Id]
           ,[Name]
           ,[Value]
           ,[JobId])
        VALUES
           ('00000000-0000-0000-0001-000000000001'
           ,'ApiImport'
           ,'true'
           ,'00000000-0000-0000-0000-000000000001')

        INSERT INTO [dbo].[JobConfigurations]
           ([Id]
           ,[Name]
           ,[Value]
           ,[JobId])
        VALUES
           ('00000000-0000-0000-0001-000000000002'
           ,'ProcessStagingData'
           ,'true'
           ,'00000000-0000-0000-0000-000000000001')


        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_Jobs_Data_Seed,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH