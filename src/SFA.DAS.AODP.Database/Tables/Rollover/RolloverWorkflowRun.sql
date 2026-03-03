CREATE TABLE [dbo].[RolloverWorkflowRun] (
    [Id]                                     UNIQUEIDENTIFIER NOT NULL,
    [AcademicYear]                           NVARCHAR (255)   NOT NULL,
    [SelectionMethod]                        NVARCHAR (255)   NOT NULL,
    [FundingEndDateEligibilityThreshold]     DATETIME2 (7)    NULL,
    [OperationalEndDateEligibilityThreshold] DATETIME2 (7)    NULL,
    [MaximumApprovalFundingEndDate]          DATETIME2 (7)    NULL,
    [CreatedAt]                              DATETIME2 (7)    NOT NULL,
    [CreatedByUsername]                      NVARCHAR (255)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_RolloverWorkflowRun_YearSelectionMethod]
    ON [dbo].[RolloverWorkflowRun]([AcademicYear] ASC, [SelectionMethod] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RolloverWorkflowRun_YearCreatedAt]
    ON [dbo].[RolloverWorkflowRun]([AcademicYear] ASC, [CreatedAt] ASC);

