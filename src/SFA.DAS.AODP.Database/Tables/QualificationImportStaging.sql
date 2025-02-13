CREATE TABLE [dbo].[QualificationImportStaging](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [JsonData] NVARCHAR(MAX) NULL, 
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_QualificationImportStaging] PRIMARY KEY ([Id])
) ON [PRIMARY]
