CREATE TABLE [dbo].[QualificationOutputFileLog]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserDisplayName] [varchar](250) NOT NULL DEFAULT '',
	[DownloadDate] [datetime] NOT NULL DEFAULT GETDATE(), 
	[PublicationDate] [date] NOT NULL DEFAULT GETDATE(),
    [FileName] VARCHAR(250) NOT NULL DEFAULT ''
)
