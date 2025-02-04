CREATE TABLE [funded].[qualifications](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[date_of_ofqual_data_snapshot] [datetime] NULL,
	[qualification_id] [int] NULL,
	[organisation_id] [int] NULL,
	[level] [nvarchar](255) NULL,
	[qualification_type] [nvarchar](255) NULL,
	[sub_category] [nvarchar](255) NULL,
	[sector_subject_area] [nvarchar](255) NULL,
	[status] [nvarchar](255) NULL,
	[awarding_organisation_url] [nvarchar](255) NULL,
	[import_date] [datetime] NOT NULL, 
    CONSTRAINT [PK_qualifications] PRIMARY KEY ([id]),
) ON [PRIMARY]
GO
ALTER TABLE [funded].[qualifications] ADD  CONSTRAINT [DF_FundedQualifications_CreateDate_GETDATE]  DEFAULT (getdate()) FOR [import_date]
GO
ALTER TABLE [funded].[qualifications]  WITH CHECK ADD FOREIGN KEY([organisation_id])
REFERENCES [dbo].[awarding_organisation] ([id])
GO
ALTER TABLE [funded].[qualifications]  WITH CHECK ADD FOREIGN KEY([qualification_id])
REFERENCES [dbo].[qualification] ([id])
GO
