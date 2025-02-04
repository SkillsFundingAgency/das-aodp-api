CREATE TABLE [dbo].[Qualification](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Qan] [varchar](10) NOT NULL,
	[QualificationName] [varchar](250) NULL, 
    CONSTRAINT [PK_Qualification] PRIMARY KEY ([Id])
) ON [PRIMARY]
GO
