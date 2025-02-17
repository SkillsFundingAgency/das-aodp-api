CREATE TABLE [dbo].[ActionType](
	[Id] [uniqueidentifier] NOT NULL,
	[Description] [varchar](250) NULL,
	CONSTRAINT PK_ActionType PRIMARY KEY CLUSTERED (ID ASC)
) ON [PRIMARY]
GO