CREATE TABLE [dbo].[qualification_discussion_history](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[qualification_id] [int] NOT NULL,
	[action_type_id] [int] NOT NULL,
	[user_display_name] [varchar](250) NULL,
	[notes] [varchar](max) NULL,
	[timestamp] [datetime] NULL, 
    CONSTRAINT [PK_qualification_discussion_history] PRIMARY KEY ([id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[qualification_discussion_history]  WITH CHECK ADD FOREIGN KEY([action_type_id])
REFERENCES [dbo].[action_type] ([id])
GO
ALTER TABLE [dbo].[qualification_discussion_history]  WITH CHECK ADD FOREIGN KEY([qualification_id])
REFERENCES [dbo].[qualification] ([id])
GO
