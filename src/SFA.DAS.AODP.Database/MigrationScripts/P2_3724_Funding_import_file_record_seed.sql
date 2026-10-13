BEGIN TRY
BEGIN TRANSACTION

DECLARE @ScriptName_P2_3724_Funding_import_file_record_seed NVARCHAR(100) = 'P2_3724_Funding_import_file_record_seed'

IF EXISTS (SELECT * FROM [MigrationScripts] WHERE [Name] = @ScriptName_P2_3724_Funding_import_file_record_seed)
BEGIN

            PRINT @ScriptName_P2_3724_Funding_import_file_record_seed    + ' has already been run.'

END
ELSE

BEGIN

IF NOT EXISTS (SELECT 1 FROM [dbo].[FileRecords] WHERE [FileCategory] = 'ApprovedFunding')
BEGIN
    INSERT INTO [dbo].[FileRecords]
    ([Id]
        ,[FileCategory]
        ,[FileName]
        ,[ContentType]
        ,[BlobContainer]
        ,[BlobPath]
        ,[UploadedByDisplayName]
        ,[UploadedAt]
        ,[ScanResult]
        ,[LastScanAt])
    VALUES
        (NEWID()
            ,'ApprovedFunding'
            ,'approved.csv'
            ,'text/csv'
            ,'funded-qualifications-import'
            ,'approved.csv'
            ,'ManualSeed'
            ,GETUTCDATE()
            ,'NotScanned'
            ,null)
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[FileRecords] WHERE [FileCategory] = 'ArchivedFunding')
BEGIN
    INSERT INTO [dbo].[FileRecords]
    ([Id]
        ,[FileCategory]
        ,[FileName]
        ,[ContentType]
        ,[BlobContainer]
        ,[BlobPath]
        ,[UploadedByDisplayName]
        ,[UploadedAt]
        ,[ScanResult]
        ,[LastScanAt])
    VALUES
        (NEWID()
            ,'ArchivedFunding'
            ,'archived.csv'
            ,'text/csv'
            ,'funded-qualifications-import'
            ,'archived.csv'
            ,'ManualSeed'
            ,GETUTCDATE()
            ,'NotScanned'
            ,null)
END

-- Record the script as run
INSERT INTO [dbo].[MigrationScripts]
(
    [Name],
    [RunDate]
)
VALUES
    (
    @ScriptName_P2_3724_Funding_import_file_record_seed,
    GETDATE()
    )
END

COMMIT TRANSACTION

END TRY
BEGIN CATCH

IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    
    THROW

END CATCH