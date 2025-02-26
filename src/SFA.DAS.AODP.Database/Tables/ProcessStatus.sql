CREATE TABLE [regulated].[ProcessStatus](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NULL,
	[IsOutcomeDecision] [int] NULL,
	CONSTRAINT PK_ProcessStatus PRIMARY KEY CLUSTERED (ID ASC)
) ON [PRIMARY]
GO