CREATE TABLE [dbo].[RolloverWorkflowCandidate] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [RolloverWorkflowRunId]  UNIQUEIDENTIFIER NOT NULL,
    [QualificationVersionId] UNIQUEIDENTIFIER NOT NULL,
    [FundingOfferId]         UNIQUEIDENTIFIER NOT NULL,
    [AcademicYear]           NVARCHAR (255)   NOT NULL,
    [RolloverRound]          INT              NOT NULL,
    [PassP1]                 INT              NOT NULL,
    [P1FailureReason]        NVARCHAR (255)   NULL,
    [IncludedInP1Export]     INT              NOT NULL,
    [IncludedInFinalUpload]  INT              NOT NULL,
    [CurrentFundingEndDate]  DATETIME2 (7)    NOT NULL,
    [ProposedFundingEndDate] DATETIME2 (7)    NULL,
    [CreatedAt]              DATETIME2 (7)    NOT NULL,
    [UpdatedAt]              DATETIME2 (7)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([RolloverWorkflowRunId]) REFERENCES [dbo].[RolloverWorkflowRun] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_WorkflowCandidate_Run_PassP1]
    ON [dbo].[RolloverWorkflowCandidate]([RolloverWorkflowRunId] ASC, [PassP1] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_WorkflowCandidate_Run_NaturalKey]
    ON [dbo].[RolloverWorkflowCandidate]([RolloverWorkflowRunId] ASC, [QualificationVersionId] ASC, [FundingOfferId] ASC, [AcademicYear] ASC, [RolloverRound] ASC);