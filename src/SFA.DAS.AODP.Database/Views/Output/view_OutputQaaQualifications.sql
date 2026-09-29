CREATE VIEW [dbo].[view_OutputQaaQualifications] AS

/*##################################################################################################
	-Name:				Output QAA Qualifications
	-Description:		A view to output the QAA qualifications data.
	-Date of Creation:	03/03/2026	
	-Created By:		Hamzah Shakeel
####################################################################################################*/

SELECT
	  qaa.AimCode
	, qaa.DateOfDataSnapshot
	, qaa.QualificationTitle
	, qaa.AwardingBody
	, qaa.Level
	, qaa.Type
	, qaa.Status
	, qaa.StartDate
	, qaa.LastDateForRegistration
	, qaaFunding.Age1619FundingApprovalEndDate
    , qaaFunding.AdvancedLearnerLoansFundingApprovalEndDate
    , qaaFunding.LegalEntitlementL2L3FundingApprovalEndDate
	, qaa.SectorSubjectArea
FROM regulated.QaaQualification qaa
LEFT JOIN (
    SELECT
          QaaQualificationId
        , MAX(CASE WHEN FundingOfferId = '00000000-0000-0000-0000-000000000007' THEN EndDate END) AS Age1619FundingApprovalEndDate
        , MAX(CASE WHEN FundingOfferId = '00000000-0000-0000-0000-000000000006' THEN EndDate END) AS AdvancedLearnerLoansFundingApprovalEndDate
        , MAX(CASE WHEN FundingOfferId = '00000000-0000-0000-0000-000000000005' THEN EndDate END) AS LegalEntitlementL2L3FundingApprovalEndDate
    FROM funded.QaaQualificationFundings
    GROUP BY QaaQualificationId
) qaaFunding
    ON qaaFunding.QaaQualificationId = qaa.Id

GO
