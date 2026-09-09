CREATE TABLE [dbo].[FileRecords]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [FileName] VARCHAR(260) NOT NULL
        CONSTRAINT DF_FileRecords_FileName DEFAULT (''),
    [ContentType] VARCHAR(260) NOT NULL,
    [BlobPath] VARCHAR(500) NOT NULL,
    [BlobContainer] VARCHAR(50) NOT NULL,
    [FileCategory] VARCHAR(50) NOT NULL
        CONSTRAINT DF_FileRecords_FileCategory DEFAULT ('Unknown'),
    [ApplicationId] UNIQUEIDENTIFIER NULL,
    [MessageId]     UNIQUEIDENTIFIER NULL,
    [QuestionId]    UNIQUEIDENTIFIER NULL,
    [UploadedAt] DATETIME NOT NULL 
        CONSTRAINT DF_FileRecords_UploadedAt DEFAULT (GETUTCDATE()),
    [UploadedByDisplayName] VARCHAR(200) NOT NULL,
    [ScanResult] VARCHAR(25) NOT NULL
        CONSTRAINT DF_FileRecords_ScanResult DEFAULT ('Unknown'),
    [LastScanAt] DATETIME NULL
);
GO

CREATE NONCLUSTERED INDEX IX_FileRecords_BlobPath
ON dbo.FileRecords (BlobPath);
GO

CREATE NONCLUSTERED INDEX IX_FileRecords_Category_Application
ON dbo.FileRecords (FileCategory, ApplicationId);
GO

CREATE NONCLUSTERED INDEX IX_FileRecords_Category_Message
ON dbo.FileRecords (FileCategory, MessageId);
GO

CREATE NONCLUSTERED INDEX IX_FileRecords_Category_Question
ON dbo.FileRecords (FileCategory, QuestionId);
GO