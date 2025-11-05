CREATE TABLE [funded].[QualificationOffers](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NULL,
	[Notes] [varchar](max) NULL,
	[FundingAvailable] [bit] NULL,
	[FundingApprovalStartDate] [datetime] NULL,
	[FundingApprovalEndDate] [datetime] NULL,
	CONSTRAINT PK_QualificationOffers PRIMARY KEY CLUSTERED (Id asc),
	CONSTRAINT FK_Qualifications FOREIGN KEY (QualificationId) REFERENCES [funded].[Qualifications] (Id)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE INDEX IX_QualificationOffers_QualificationId ON funded.QualificationOffers (QualificationId)      
INCLUDE ([Name], Notes);                                
