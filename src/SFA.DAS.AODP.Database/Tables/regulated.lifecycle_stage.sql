CREATE TABLE [regulated].[lifecycle_stage](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](250) NULL, 
    CONSTRAINT [PK_lifecycle_stage] PRIMARY KEY ([id])
) ON [PRIMARY]
GO
