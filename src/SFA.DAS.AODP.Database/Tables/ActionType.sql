CREATE TABLE [dbo].[ActionType](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Description] [varchar](250) NULL, 
    CONSTRAINT [PK_ActionType] PRIMARY KEY ([Id])
) ON [PRIMARY]
GO
