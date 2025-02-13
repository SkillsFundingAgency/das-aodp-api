CREATE VIEW [dbo].[View_SectionSummaryForApplication] AS
SELECT 
[dbo].[Sections].[Id] SectionId,
[dbo].[Applications].[Id] ApplicationId,
[dbo].[View_RemainingPagesBySectionForApplication].PageCount RemainingPages,
[dbo].[View_SkippedPagesBySectionForApplication].SkippedPageCount SkippedPages,
[dbo].[View_SectionPageCount].PageCount TotalPages
FROM [dbo].[Sections]
INNER JOIN [dbo].[Applications] ON [dbo].[Applications].[FormVersionId] = [dbo].[Sections].[FormVersionId]
LEFT JOIN [dbo].[View_SkippedPagesBySectionForApplication] 
	ON [dbo].[View_SkippedPagesBySectionForApplication].[SectionId] = [dbo].[Sections].[Id]
	AND [dbo].[View_SkippedPagesBySectionForApplication].[ApplicationId] = [dbo].[Applications].[Id] 

LEFT JOIN [dbo].[View_RemainingPagesBySectionForApplication] 
	ON [dbo].[View_RemainingPagesBySectionForApplication].[SectionId] = [dbo].[Sections].[Id]
	AND [dbo].[View_RemainingPagesBySectionForApplication].[ApplicationId] = [dbo].[Applications].[Id] 

LEFT JOIN [dbo].[View_SectionPageCount] 
	ON [dbo].[View_SectionPageCount].[SectionId] = [dbo].[Sections].[Id]

