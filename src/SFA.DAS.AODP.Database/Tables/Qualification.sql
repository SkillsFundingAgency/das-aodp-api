CREATE TABLE [dbo].[Qualification](
	[Id] [uniqueidentifier] NOT NULL,
	[Qan] [varchar](10) NOT NULL,
	[QualificationName] [varchar](250) NULL,
	CONSTRAINT PK_Qualification PRIMARY KEY CLUSTERED (ID ASC)
) ON [PRIMARY]
GO

CREATE INDEX [IX_Qualification_Qan] ON [dbo].[Qualification] ([Qan])
