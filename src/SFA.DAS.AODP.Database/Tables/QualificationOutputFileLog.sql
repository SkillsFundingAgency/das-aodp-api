CREATE TABLE [dbo].[QualificationOutputFileLog]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserDisplayName] [varchar](250) NOT NULL,
	[DownloadDate] [datetime] NOT NULL, 
	[PublicationDate] [date] NOT NULL,
    [ApprovedFileName] VARCHAR(250) NOT NULL, 
    [ArchivedFilename] VARCHAR(250) NOT NULL,

)
