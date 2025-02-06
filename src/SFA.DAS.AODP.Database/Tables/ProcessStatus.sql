CREATE TABLE [regulated].[ProcessStatus](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Name] [varchar](250) NULL,
	[IsOutcomeDecision] [int] NULL, 
    CONSTRAINT [PK_ProcessStatus] PRIMARY KEY ([Id])
) ON [PRIMARY]
GO
