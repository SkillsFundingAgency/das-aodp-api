CREATE TABLE [dbo].[FundedQualificationOffers]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[FundedQualificationId] Int NOT NULL,
	[Name] varchar(255)  NULL,
	[Notes] varchar(1000)  NULL,
	[FundingAvailable] BIT NULL,
	[FundingApprovalStartDate] DATETIME NULL,
	[FundingApprovalEndDate] DATETIME NULL,
		
	CONSTRAINT [PK_FundedQualificationOffers] PRIMARY KEY NONCLUSTERED ( [Id] ASC ),
	CONSTRAINT [FK_Offers_Qualificats_QualificationId] FOREIGN KEY ([FundedQualificationId]) REFERENCES [FundedQualifications] ([Id]) ON DELETE CASCADE);
