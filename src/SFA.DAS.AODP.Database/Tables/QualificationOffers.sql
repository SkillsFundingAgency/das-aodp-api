CREATE TABLE [funded].[QualificationOffers](
	[Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
	[QualificationId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NULL,
	[Notes] [varchar](max) NULL,
	[FundingAvailable] [bit] NULL,
	[FundingApprovalStartDate] [datetime] NULL,
	[FundingApprovalEndDate] [datetime] NULL, 
    CONSTRAINT [PK_QualificationOffers] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Qualifications] FOREIGN KEY (QualificationId) REFERENCES funded.Qualifications(Id),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
