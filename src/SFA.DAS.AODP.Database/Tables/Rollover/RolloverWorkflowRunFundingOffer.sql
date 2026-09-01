CREATE TABLE [dbo].[RolloverWorkflowRunFundingOffer] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [RolloverWorkflowRunId] UNIQUEIDENTIFIER NOT NULL,
    [FundingOfferId]        UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([RolloverWorkflowRunId]) REFERENCES [dbo].[RolloverWorkflowRun] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_WorkflowRunFundingOffer_Offer]
    ON [dbo].[RolloverWorkflowRunFundingOffer]([FundingOfferId] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_WorkflowRunFundingOffer_Run_Offer]
    ON [dbo].[RolloverWorkflowRunFundingOffer]([RolloverWorkflowRunId] ASC, [FundingOfferId] ASC);