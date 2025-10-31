CREATE TABLE [dbo].[QualificationOutputFileLog]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[UserDisplayName] [varchar](250) NOT NULL,
	[Timestamp] [datetime] NOT NULL, 
    [ApprovedFileName] VARCHAR(250) NOT NULL, 
    [ArchivedFilename] VARCHAR(250) NOT NULL,

)
