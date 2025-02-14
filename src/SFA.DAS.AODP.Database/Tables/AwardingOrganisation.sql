IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AwardingOrganisation]') AND type in (N'U'))
DROP TABLE [dbo].[AwardingOrganisation]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AwardingOrganisation](
	[Id] [uniqueidentifier] NOT NULL,
	[Ukprn] [int] NULL,
	[RecognitionNumber] [varchar](250) NULL,
	[NameLegal] [varchar](250) NULL,
	[NameOfqual] [varchar](250) NULL,
	[NameGovUk] [varchar](250) NULL,
	[Name_Dsi] [varchar](250) NULL,
	[Acronym] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
