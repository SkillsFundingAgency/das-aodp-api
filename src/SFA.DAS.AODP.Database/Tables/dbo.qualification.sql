CREATE TABLE [dbo].[qualification](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[qan] [varchar](10) NOT NULL,
	[qualification_name] [varchar](250) NULL, 
    CONSTRAINT [PK_qualification] PRIMARY KEY ([id])
) ON [PRIMARY]
GO
