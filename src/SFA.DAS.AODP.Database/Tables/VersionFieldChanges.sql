CREATE TABLE [regulated].[VersionFieldChanges](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationVersionNumber] [int] NULL,
	[ChangedFieldNames] [varchar](250) NULL,
	CONSTRAINT PK_VersionFieldChanges PRIMARY KEY CLUSTERED (Id ASC)
) ON [PRIMARY]
GO