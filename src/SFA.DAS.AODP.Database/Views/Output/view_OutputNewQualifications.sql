CREATE VIEW [dbo].[view_OutputNewQualifications] AS

/*##################################################################################################
	-Name:				Output New Qualifications
	-Description:		All new qualifications that have been funded during the current review cycle
	-Date of Creation:	10/04/2025
	-Created By:		Robert Rybnikar
####################################################################################################*/

SELECT 
    fq.ImportDate AS DateOfOfqualDataSnapshot,
	q.QualificationName AS Title,
	ao.NameOfqual AS OrganisationName,
    q.Qan AS QualificationNumber,
	qv.Level AS Level,
	qv.Type as QualificationType,
	qv.SubLevel AS Subcategory,
    qv.SSA AS SectorSubjectArea
	--foreach offer:
	--<Name>_FundingAvailable
	--<Name>_FundingApprovalStartDate
	--<Name>_FundingApprovalEndDate
	--<Name>_Notes

FROM dbo.Qualification q
inner join regulated.QualificationVersions qv ON q.Id = qv.QualificationId
inner join dbo.AwardingOrganisation ao ON qv.AwardingOrganisationId = ao.Id
inner join funded.Qualifications fq ON q.Id = fq.QualificationId
inner join funded.QualificationFundings qualfunding ON qualfunding.QualificationVersionId = qv.Id
inner join dbo.FundingOffers offertype ON offertype.Id = qualfunding.FundingOfferId
Left Outer Join regulated.LifecycleStage ls ON qv.LifecycleStageId = ls.Id
 
WHERE ls.Name = 'New'
	and qv.ProcessStatusId = '00000000-0000-0000-0000-000000000004' --Approved
GO
