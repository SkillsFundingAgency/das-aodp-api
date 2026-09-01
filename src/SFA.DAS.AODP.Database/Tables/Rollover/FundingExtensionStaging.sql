CREATE TABLE [dbo].[FundingExtensionStaging]
(
    [OperationId]               UNIQUEIDENTIFIER NOT NULL,
    [RolloverCandidateId]       UNIQUEIDENTIFIER NOT NULL,
    [QualificationFundingId]    UNIQUEIDENTIFIER NULL,
    [RolloverStatus]            NVARCHAR(255) NOT NULL,
    [ExclusionReason]           NVARCHAR(255) NULL,
    [NewFundingEndDate]         DATETIME2(7) NULL,
    [FundingEndDate]            DATE NULL,
    [FundingComments]           NVARCHAR(MAX) NULL,
    [CreatedAt]                 DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_FundingExtensionStaging]
        PRIMARY KEY CLUSTERED ([OperationId], [RolloverCandidateId]),
    CONSTRAINT [FK_FundingExtensionStaging_RolloverCandidates]
        FOREIGN KEY ([RolloverCandidateId]) REFERENCES [dbo].[RolloverCandidates]([Id]),
    CONSTRAINT [FK_FundingExtensionStaging_QualificationFundings]
        FOREIGN KEY ([QualificationFundingId]) REFERENCES [funded].[QualificationFundings]([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_FundingExtensionStaging_QualificationFundingId]
    ON [dbo].[FundingExtensionStaging]([OperationId], [QualificationFundingId])
    WHERE [QualificationFundingId] IS NOT NULL;
