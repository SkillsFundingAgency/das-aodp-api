CREATE TABLE [dbo].[RolloverWorkflowRunFilterValue] (
    [Id]                          UNIQUEIDENTIFIER NOT NULL,
    [RolloverWorkflowRunFilterId] UNIQUEIDENTIFIER NOT NULL,
    [ValueText]                   NVARCHAR (255)   NULL,
    [ValueGuid]                   UNIQUEIDENTIFIER NULL,
    [DisplayText]                 NVARCHAR (255)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([RolloverWorkflowRunFilterId]) REFERENCES [dbo].[RolloverWorkflowRunFilter] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_WorkflowRunFilterValue_Filter_Guid]
    ON [dbo].[RolloverWorkflowRunFilterValue]([RolloverWorkflowRunFilterId] ASC, [ValueGuid] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_WorkflowRunFilterValue_Filter_Text]
    ON [dbo].[RolloverWorkflowRunFilterValue]([RolloverWorkflowRunFilterId] ASC, [ValueText] ASC);