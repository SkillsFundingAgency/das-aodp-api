CREATE TABLE [dbo].[QuestionValidations](
	[Id] [uniqueidentifier] NOT NULL,
	[QuestionId] [uniqueidentifier] NOT NULL,
	[MinLength] [int] NULL,
	[MaxLength] [int] NULL,
 [MinNumberOfOptions] INT NULL, 
    [MaxNumberOfOptions] INT NULL, 
    [NumberGreaterThanOrEqualTo] INT NULL, 
    [NumberLessThanOrEqualTo] INT NULL, 
    [NumberNotEqualTo] INT NULL, 
    CONSTRAINT [PK_QuestionValidations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[QuestionValidations]  WITH CHECK ADD  CONSTRAINT [FK_QuestionValidations_Questions] FOREIGN KEY([QuestionId])
REFERENCES [dbo].[Questions] ([Id])
GO

ALTER TABLE [dbo].[QuestionValidations] CHECK CONSTRAINT [FK_QuestionValidations_Questions]
GO


