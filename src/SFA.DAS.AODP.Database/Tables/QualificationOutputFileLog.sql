CREATE TABLE [dbo].[QualificationOutputFileLog]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserDisplayName] [varchar](250) NOT NULL,
	[DownloadDate] [datetime] NOT NULL, 
	[PublicationDate] [date] NOT NULL,
    [FileName] VARCHAR(250) NOT NULL, 

)
