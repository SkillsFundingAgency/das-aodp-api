CREATE TABLE [dbo].[QualificationImportStaging](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [JsonData] NVARCHAR(MAX) NULL, 
    [CreatedDate] DATETIME NULL DEFAULT GETDATE()
    CONSTRAINT [PK_AwardingOrganisation] PRIMARY KEY ([Id])
) ON [PRIMARY]
