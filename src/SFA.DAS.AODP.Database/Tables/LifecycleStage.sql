CREATE TABLE [regulated].[LifecycleStage](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[Name] [varchar](250) NULL, 
    CONSTRAINT [PK_LifecycleStage] PRIMARY KEY ([Id])
) ON [PRIMARY]
GO
