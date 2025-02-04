CREATE TABLE [regulated].[version_field_changes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[qualification_version_number] [int] NULL,
	[changed_field_names] [varchar](250) NULL, 
    CONSTRAINT [PK_version_field_changes] PRIMARY KEY ([id])
) ON [PRIMARY]
GO
