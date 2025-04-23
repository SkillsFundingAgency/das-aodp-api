BEGIN TRY
    BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_Qual_Ref_Data NVARCHAR(100) = 'MVS1_Qual_Ref_Data'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_Qual_Ref_Data)
        BEGIN

            PRINT @ScriptName_MVS1_Qual_Ref_Data + ' has already been run.'

        END
    ELSE

    BEGIN        
       
       DELETE FROM [regulated].[QualificationVersions]
       DELETE FROM [regulated].[ProcessStatus]
       INSERT INTO [regulated].[ProcessStatus]
                   ([Id]
                   ,[Name]
                   ,[IsOutcomeDecision])
       VALUES ('00000000-0000-0000-0000-000000000001' ,'Decision Needed' ,0),
	          ('00000000-0000-0000-0000-000000000002' ,'No Action Required' ,1),
	          ('00000000-0000-0000-0000-000000000003' ,'Hold' ,0),
	          ('00000000-0000-0000-0000-000000000004' ,'Approved' ,1),
	          ('00000000-0000-0000-0000-000000000005' ,'Rejected' ,1)
	  
       DELETE FROM [regulated].[LifecycleStage]
       INSERT INTO [regulated].[LifecycleStage]
                   ([Id]
                   ,[Name])
             VALUES
                   ('00000000-0000-0000-0000-000000000001','New'),
		           ('00000000-0000-0000-0000-000000000002','Changed')
		   
       DELETE FROM [dbo].[QualificationDiscussionHistory]
       DELETE FROM [dbo].[ActionType]
       INSERT INTO [dbo].[ActionType]
                   ([Id]
                   ,[Description])
       VALUES ('00000000-0000-0000-0000-000000000001','No Action Required'),
	          ('00000000-0000-0000-0000-000000000002','Action Required'),
	          ('00000000-0000-0000-0000-000000000003','Ignore')


        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_Qual_Ref_Data,
            GETDATE()
        )
    END
    
    COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH