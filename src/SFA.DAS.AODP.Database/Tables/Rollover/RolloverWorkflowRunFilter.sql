CREATE TABLE [dbo].[RolloverWorkflowRunFilter] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [RolloverWorkflowRunId] UNIQUEIDENTIFIER NOT NULL,
    [FilterKey]             NVARCHAR (255)   NOT NULL,
    [CreatedAt]             DATETIME2 (7)    NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([RolloverWorkflowRunId]) REFERENCES [dbo].[RolloverWorkflowRun] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_WorkflowRunFilter_Run_FilterKey]
    ON [dbo].[RolloverWorkflowRunFilter]([RolloverWorkflowRunId] ASC, [FilterKey] ASC);

