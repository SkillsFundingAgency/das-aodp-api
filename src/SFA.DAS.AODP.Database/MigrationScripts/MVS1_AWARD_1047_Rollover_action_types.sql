BEGIN TRY
BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_AWARD_1047_Rollover_action_types NVARCHAR(100) = 'MVS1_AWARD_1047_Rollover_action_types'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_AWARD_1047_Rollover_action_types)
BEGIN

            PRINT @ScriptName_MVS1_AWARD_1047_Rollover_action_types + ' has already been run.'

END
ELSE

BEGIN

    INSERT INTO [dbo].[ActionType]
        ([Id]
        ,[Description])
    VALUES
    ('00000000-0000-0000-0000-000000000004','Rollover - Funding Extended'),
    ('00000000-0000-0000-0000-000000000005','Rollover - Funding Not Extended')



-- Record the script as run
INSERT INTO [dbo].[MigrationScripts]
(
    [Name],
    [RunDate]
)
VALUES
    (
    @ScriptName_MVS1_AWARD_1047_Rollover_action_types,
    GETDATE()
    )
END

COMMIT TRANSACTION

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH