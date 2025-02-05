CREATE VIEW [dbo].[View_SectionPageCount] AS 
SELECT 
[dbo].[Sections].[Id] SectionId,
(SELECT COUNT(*) FROM [dbo].[Pages] WHERE [dbo].[Pages].[SectionId] = [dbo].[Sections].[Id]) PageCount
FROM [dbo].[Sections]
