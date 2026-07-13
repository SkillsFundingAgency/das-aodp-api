BEGIN TRY
BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_Drop_OutputChangedQualifications_View NVARCHAR(100) = 'MVS1_Drop_OutputChangedQualifications_View'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_Drop_OutputChangedQualifications_View)
BEGIN

    PRINT @ScriptName_MVS1_Drop_OutputChangedQualifications_View + ' has already been run.'

END
ELSE

BEGIN

    IF EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[view_OutputChangedQualifications]'))
    
    DROP VIEW [dbo].[view_OutputChangedQualifications];
    
    -- Record the script as run
    INSERT INTO [dbo].[MigrationScripts]
    (
        [Name],
        [RunDate]
    )
    VALUES
    (
        @ScriptName_MVS1_Drop_OutputChangedQualifications_View, 
        GETDATE()
    )

END

COMMIT TRANSACTION

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH