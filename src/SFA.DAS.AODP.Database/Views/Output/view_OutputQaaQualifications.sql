CREATE VIEW [dbo].[view_OutputQaaQualifications] AS

/*##################################################################################################
	-Name:				Output QAA Qualifications
	-Description:		A view to output the QAA qualifications data.
	-Date of Creation:	03/03/2026	
	-Created By:		Hamzah Shakeel
####################################################################################################*/

SELECT
	  AimCode
	, DateOfDataSnapshot
	, QualificationTitle
	, AwardingBody
	, Level
	, Type
	, Status
	, StartDate
	, LastDateForRegistration
	, LastFundingApprovalEndDate
	, SectorSubjectArea
FROM regulated.QaaQualification

GO