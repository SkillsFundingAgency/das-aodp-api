CREATE TABLE [dbo].[FundingExtensionStaging]
(
    [OperationId]               UNIQUEIDENTIFIER NOT NULL,
    [RolloverCandidateId]       UNIQUEIDENTIFIER NOT NULL,
    [SourceType]                NVARCHAR(50) NULL,
    [SourceFundingRecordId]     UNIQUEIDENTIFIER NULL,
    [RolloverStatus]            NVARCHAR(255) NOT NULL,
    [ExclusionReason]           NVARCHAR(255) NULL,
    [NewFundingEndDate]         DATETIME2(7) NULL,
    [FundingEndDate]            DATE NULL,
    [FundingComments]           NVARCHAR(MAX) NULL,
    [CreatedAt]                 DATETIME2(7) NOT NULL,
    CONSTRAINT [PK_FundingExtensionStaging]
        PRIMARY KEY CLUSTERED ([OperationId], [RolloverCandidateId]),
    CONSTRAINT [FK_FundingExtensionStaging_RolloverCandidates]
        FOREIGN KEY ([RolloverCandidateId]) REFERENCES [dbo].[RolloverCandidates]([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_FundingExtensionStaging_SourceFundingRecordId]
    ON [dbo].[FundingExtensionStaging]([OperationId], [SourceType], [SourceFundingRecordId])
    WHERE [SourceFundingRecordId] IS NOT NULL;

