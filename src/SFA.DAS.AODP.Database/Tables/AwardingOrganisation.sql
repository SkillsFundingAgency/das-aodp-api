CREATE TABLE [dbo].[AwardingOrganisation](
	[Id] [uniqueidentifier] NOT NULL,
	[Ukprn] [int] NULL,
	[RecognitionNumber] [varchar](250) NULL,
	[NameLegal] [varchar](250) NULL,
	[NameOfqual] [varchar](250) NULL,
	[NameGovUk] [varchar](250) NULL,
	[Name_Dsi] [varchar](250) NULL,
	[Acronym] [varchar](100) NULL,
	CONSTRAINT PK_AwardingOrganisation PRIMARY KEY CLUSTERED (ID ASC)
) ON [PRIMARY]
GO
