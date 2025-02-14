IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[regulated].[ProcessStatus]') AND type in (N'U'))
DROP TABLE [regulated].[ProcessStatus]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [regulated].[ProcessStatus](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](250) NULL,
	[IsOutcomeDecision] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
