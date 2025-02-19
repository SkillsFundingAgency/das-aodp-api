CREATE VIEW [dbo].[View_PagesSectionsAssociatedWithRouting] AS
SELECT DISTINCT 
    sourceQuestions.Id AS sourceQuestionId, 
    sourcePage.Id AS sourcePageId, 
    sourcePage.SectionId AS SourceSectionId, 
    r.NextPageId, 
    nextPage.SectionId AS nextPageSectionId, 
    nextSections.Id AS nextSectionId
FROM Routes AS r
INNER JOIN Questions AS sourceQuestions ON sourceQuestions.Id = r.SourceQuestionId
INNER JOIN Pages AS sourcePage ON sourcePage.Id = sourceQuestions.PageId
LEFT JOIN Pages AS nextPage ON nextPage.Id = r.NextPageId
LEFT JOIN Sections AS nextSections ON nextSections.Id = r.NextSectionId;