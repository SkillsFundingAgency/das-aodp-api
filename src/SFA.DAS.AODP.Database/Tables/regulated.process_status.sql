CREATE TABLE [regulated].[process_status](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](250) NULL,
	[is_outcome_decision] [int] NULL, 
    CONSTRAINT [PK_process_status] PRIMARY KEY ([id])
) ON [PRIMARY]
GO
