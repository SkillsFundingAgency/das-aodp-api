CREATE TABLE [funded].[QualificationFundingFeedbacks](
	[Id] [uniqueidentifier] NOT NULL,
	[QualificationVersionId] [uniqueidentifier] NOT NULL,
	[Approved] [bit] NULL,
	[Comments] [nvarchar](max) NULL,
 CONSTRAINT [PK_QualificationFundingFeedback] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [funded].[QualificationFundingFeedbacks]  WITH CHECK ADD  CONSTRAINT [FK_QualificationFundingFeedbacks_QualificationVersions] FOREIGN KEY([QualificationVersionId])
REFERENCES [regulated].[QualificationVersions] ([Id])
GO

ALTER TABLE [funded].[QualificationFundingFeedbacks] CHECK CONSTRAINT [FK_QualificationFundingFeedbacks_QualificationVersions]
GO