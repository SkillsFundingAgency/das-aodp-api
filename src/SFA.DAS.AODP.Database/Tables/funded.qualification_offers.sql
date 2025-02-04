CREATE TABLE [funded].[qualification_offers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[qualification_id] [int] NULL,
	[name] [varchar](max) NULL,
	[notes] [varchar](max) NULL,
	[funding_available] [bit] NULL,
	[funding_approval_start_date] [datetime] NULL,
	[funding_approval_end_date] [datetime] NULL, 
    CONSTRAINT [PK_qualification_offers] PRIMARY KEY ([id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [funded].[qualification_offers]  WITH CHECK ADD FOREIGN KEY([qualification_id])
REFERENCES [funded].[qualifications] ([id])
GO
