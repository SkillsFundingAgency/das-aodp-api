CREATE TABLE [regulated].[VersionFieldChanges](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[QualificationVersionNumber] [int] NULL,
	[ChangedFieldNames] [varchar](250) NULL, 
    CONSTRAINT [PK_VersionFieldChanges] PRIMARY KEY ([Id])
) ON [PRIMARY]
GO
