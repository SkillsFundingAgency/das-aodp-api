CREATE TABLE [funded].[QaaQualificationFundings] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [QaaQualificationId] UNIQUEIDENTIFIER NOT NULL,
    [FundingOfferId]     UNIQUEIDENTIFIER NOT NULL,
    [StartDate]          DATE             NULL,
    [EndDate]            DATE             NULL,
    [FundingStatus]      NVARCHAR (255)   NULL,
    [Comments]           NVARCHAR (MAX)   NULL,
    [CreatedAt]          DATETIME2 (7)    NOT NULL,
    [UpdatedAt]          DATETIME2 (7)    NOT NULL,
    CONSTRAINT [PK_QaaQualificationFundings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QaaQualificationFundings_QaaQualification] FOREIGN KEY ([QaaQualificationId])
        REFERENCES [regulated].[QaaQualification] ([Id]),
    CONSTRAINT [FK_QaaQualificationFundings_FundingOffers] FOREIGN KEY ([FundingOfferId])
        REFERENCES [dbo].[FundingOffers] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_QaaQualificationFundings_QaaQualification]
    ON [funded].[QaaQualificationFundings]([QaaQualificationId] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_QaaQualificationFundings_Qualification_Offer]
    ON [funded].[QaaQualificationFundings]([QaaQualificationId] ASC, [FundingOfferId] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_QaaQualificationFundings_Offer]
    ON [funded].[QaaQualificationFundings]([FundingOfferId] ASC);
GO
