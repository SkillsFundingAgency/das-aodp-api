CREATE TABLE [dbo].[QaaQualificationDownloadLog]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [UserDisplayName] [varchar](250) NOT NULL DEFAULT '',
    [DownloadDate] [datetime] NOT NULL DEFAULT GETDATE(),
    [FileName] VARCHAR(250) NOT NULL DEFAULT ''
)
