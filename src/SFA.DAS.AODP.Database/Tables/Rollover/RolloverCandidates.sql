CREATE TABLE [dbo].[RolloverCandidates] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [QualificationVersionId] UNIQUEIDENTIFIER NOT NULL,
    [FundingOfferId]         UNIQUEIDENTIFIER NOT NULL,
    [AcademicYear]           NVARCHAR (255)   NOT NULL,
    [RolloverRound]          INT              NOT NULL,
    [RolloverDecisionRunId]  UNIQUEIDENTIFIER NULL,
    [RolloverStatus]         NVARCHAR (255)   NOT NULL,
    [ExclusionReason]        NVARCHAR (255)   NULL,
    [PreviousFundingEndDate] DATETIME2 (7)    NULL,
    [NewFundingEndDate]      DATETIME2 (7)    NULL,
    [ReviewedAt]             DATETIME2 (7)    NULL,
    [ReviewedByUsername]     NVARCHAR (255)   NULL,
    [IsActive]               BIT              NOT NULL,
    [CreatedAt]              DATETIME2 (7)    NOT NULL,
    [UpdatedAt]              DATETIME2 (7)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([RolloverDecisionRunId]) REFERENCES [dbo].[RolloverDecisionRun] ([Id]),
    FOREIGN KEY ([QualificationVersionId]) REFERENCES [regulated].[QualificationVersions] ([Id]),
    FOREIGN KEY ([FundingOfferId]) REFERENCES [dbo].[FundingOffers] ([Id]),
);

GO
ALTER TABLE dbo.RolloverCandidates
    ADD CONSTRAINT UQ_RolloverCandidates_QualOfferYearRound
        UNIQUE (QualificationVersionId, FundingOfferId, AcademicYear, RolloverRound);

GO
CREATE NONCLUSTERED INDEX [IX_RolloverCandidates_DecisionRunId]
    ON [dbo].[RolloverCandidates]([RolloverDecisionRunId] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_RolloverCandidates_ByCandidateKey]
    ON [dbo].[RolloverCandidates]([QualificationVersionId] ASC, [FundingOfferId] ASC, [AcademicYear] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_RolloverCandidates_Totals]
    ON [dbo].[RolloverCandidates]([AcademicYear] ASC, [RolloverStatus] ASC, [IsActive] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_RolloverCandidates_QualOfferYearRound]
    ON [dbo].[RolloverCandidates]([QualificationVersionId] ASC, [FundingOfferId] ASC, [AcademicYear] ASC, [RolloverRound] ASC);