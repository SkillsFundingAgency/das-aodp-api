CREATE TABLE [dbo].[action_type](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[description] [varchar](250) NULL, 
    CONSTRAINT [PK_action_type] PRIMARY KEY ([id])
) ON [PRIMARY]
GO
