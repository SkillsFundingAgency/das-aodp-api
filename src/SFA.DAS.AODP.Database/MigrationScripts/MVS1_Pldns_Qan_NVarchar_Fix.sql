IF OBJECT_ID(N'[dbo].[MigrationScripts]', N'U') IS NOT NULL
   AND OBJECT_ID(N'[dbo].[Pldns]', N'U') IS NOT NULL
BEGIN
BEGIN TRY
BEGIN TRANSACTION

    DECLARE @ScriptName_MVS1_Pldns_Qan_NVarchar_Fix NVARCHAR(100) = 'MVS1_Pldns_Qan_NVarchar_Fix'

    IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_MVS1_Pldns_Qan_NVarchar_Fix)
    BEGIN

        PRINT @ScriptName_MVS1_Pldns_Qan_NVarchar_Fix + ' has already been run.'

    END
    ELSE
    BEGIN

        -- Pldns.Qan was VARCHAR(100) while Qualification.Qan (and every other Qan column it is
        -- joined/compared against) is NVARCHAR(10). Comparing the two forces an implicit conversion
        -- of Pldns.Qan, which prevents IX_Pldns_Qan being seeked and causes a full scan on every
        -- comparison. Aligning the type removes the implicit conversion.

        -- Only alter if the column is still VARCHAR — a database created fresh from the dacpac
        -- already has NVARCHAR(10), so there is nothing to fix and the script is just recorded.
        IF EXISTS (
            SELECT 1
            FROM sys.columns c
            JOIN sys.types t ON c.user_type_id = t.user_type_id
            WHERE c.object_id = OBJECT_ID(N'[dbo].[Pldns]')
              AND c.name = N'Qan'
              AND t.name = N'varchar')
        BEGIN

            -- Fail fast with a clear message if narrowing the column would truncate existing data,
            -- rather than letting ALTER COLUMN raise a generic truncation error.
            IF EXISTS (SELECT 1 FROM [dbo].[Pldns] WHERE LEN([Qan]) > 10)
            BEGIN
                RAISERROR('MVS1_Pldns_Qan_NVarchar_Fix: cannot proceed - one or more Pldns.Qan values exceed 10 characters.', 16, 1);
            END

            DROP INDEX IF EXISTS [IX_Pldns_Qan] ON [dbo].[Pldns];

            ALTER TABLE [dbo].[Pldns] ALTER COLUMN [Qan] NVARCHAR(10) NOT NULL;

            CREATE NONCLUSTERED INDEX [IX_Pldns_Qan] ON [dbo].[Pldns] ([Qan]);

        END

        -- Record the script as run
        INSERT INTO [dbo].[MigrationScripts]
        (
            [Name],
            [RunDate]
        )
        VALUES
        (
            @ScriptName_MVS1_Pldns_Qan_NVarchar_Fix,
            GETDATE()
        )
    END

COMMIT TRANSACTION

END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

    THROW

END CATCH
END