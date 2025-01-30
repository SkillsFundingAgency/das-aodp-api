CREATE VIEW [dbo].[View_QuestionRoutingDetails] AS
SELECT
SourceSection.FormVersionId,
Questions.Id QuestionId,
Questions.[Order] QuestionOrder,
SourceSection.Id SectionId,
SourceSection.[Order] SectionOrder,
SourceSection.Title SectionTitle,
SourcePage.id PageId,
SourcePage.Title PageTitle,
SourcePage.[Order] PageOrder,
QuestionOptions.Id OptionId,
QuestionOptions.Value OptionValue,
QuestionOptions.[Order] OptionOrder,

NextPage.Id NextPageId,
NextPage.Title NextPageTitle,
NextPage.[Order] NextPageOrder,

NextSection.Id NextSectionId,
NextSection.Title NextSectionTitle,
NextSection.[Order] NextSectionOrder,

Routes.EndForm,
Routes.EndSection
FROM 
	Routes
	INNER JOIN Questions ON Questions.Id = Routes.SourceQuestionId
	INNER JOIN Pages SourcePage ON SourcePage.Id = Questions.PageId
	INNER JOIN Sections SourceSection ON SourceSection.Id = SourcePage.SectionId
	INNER JOIN QuestionOptions ON QuestionOptions.Id = Routes.SourceOptionId
	LEFT JOIN Pages NextPage ON NextPage.Id = Routes.NextPageId
	LEFT JOIN Sections NextSection ON NextSection.Id = Routes.NextSectionId