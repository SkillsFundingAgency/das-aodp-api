CREATE TABLE [dbo].[awarding_organisation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ukprn] [int] NULL,
	[recognition_number] [varchar](250) NULL,
	[name_legal] [varchar](250) NULL,
	[name_ofqual] [varchar](250) NULL,
	[name_govuk] [varchar](250) NULL,
	[name_dsi] [varchar](250) NULL,
	[acronym] [varchar](100) NULL, 
    CONSTRAINT [PK_awarding_organisation] PRIMARY KEY ([id])
) ON [PRIMARY]
GO
