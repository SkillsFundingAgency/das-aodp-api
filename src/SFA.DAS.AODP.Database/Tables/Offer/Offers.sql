﻿CREATE TABLE [dbo].[FundingOffers](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_FundingOffers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
