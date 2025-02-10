CREATE VIEW [dbo].[View_SkippedPagesBySectionForApplication] AS
SELECT 
[dbo].[Sections].[Id] SectionId,
[dbo].[Applications].[Id] ApplicationId,
COUNT([dbo].[Pages].[Id]) AS SkippedPageCount
FROM [dbo].[Sections]
INNER JOIN [dbo].[Applications] ON [dbo].[Applications].[FormVersionId] = [dbo].[Sections].[FormVersionId]
LEFT JOIN [dbo].[Pages] ON [dbo].[Sections].[Id] = [dbo].[Pages].[SectionId]
LEFT JOIN [dbo].[ApplicationPages] ON [dbo].[Pages].[Id] = [dbo].[ApplicationPages].[PageId] AND [dbo].[ApplicationPages].[ApplicationId] = [dbo].[Applications].[Id]
WHERE [dbo].[ApplicationPages].[Status] = 'Skipped'
GROUP BY   [dbo].[Sections].[Id], [dbo].[Applications].[Id]
