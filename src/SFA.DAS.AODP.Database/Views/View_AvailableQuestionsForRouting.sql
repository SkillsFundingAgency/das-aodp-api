CREATE VIEW [dbo].[View_AvailableQuestionsForRouting]
AS
SELECT
	Questions.Id QuestionId,
	Pages.Id PageId,
	Sections.Id SectionId,
	Sections.FormVersionId FormVersionId,
	Questions.Title QuestionTitle,
	Questions.[Order] QuestionOrder,
	Pages.Title PageTitle,
	Pages.[Order] PageOrder,
	Sections.Title SectionTitle,
	Sections.[Order] SectionOrder
FROM
	Questions
INNER JOIN Pages ON Pages.Id = Questions.PageId
INNER JOIN Sections ON Sections.Id = Pages.SectionId
WHERE 
	NOT EXISTS( SELECT 1 FROM Routes 
				INNER JOIN Questions Q ON Q.Id = Routes.SourceQuestionId
				WHERE Q.PageId = Pages.Id -- Only 1 routing per page allowed
	           )
	AND Exists( SELECT 1 FROM QuestionOptions 
				WHERE QuestionOptions.QuestionId = Questions.Id
			)