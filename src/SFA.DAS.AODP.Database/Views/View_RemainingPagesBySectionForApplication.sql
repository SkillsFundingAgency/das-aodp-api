CREATE VIEW [dbo].[View_RemainingPagesBySectionForApplication] AS
SELECT 
[dbo].[Sections].[Id] SectionId,
[dbo].[Applications].[Id] ApplicationId,
 COUNT([dbo].[Pages].[Id]) AS PageCount
FROM [dbo].[Sections]
INNER JOIN [dbo].[Applications] ON [dbo].[Applications].[FormVersionId] = [dbo].[Sections].[FormVersionId]
LEFT JOIN [dbo].[Pages] ON [dbo].[Sections].[Id] = [dbo].[Pages].[SectionId]
LEFT JOIN [dbo].[ApplicationPages] ON [dbo].[Pages].[Id] = [dbo].[ApplicationPages].[PageId]
WHERE [dbo].[ApplicationPages].[Id] IS NULL OR [dbo].[ApplicationPages].[Status] = 'NotStarted'
GROUP BY   [dbo].[Sections].[Id], [dbo].[Applications].[Id]
