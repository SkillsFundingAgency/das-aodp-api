create view [regulated].[v_QualificationNewReviewRequired] as

/*##################################################################################################
	-Name:				Qualification New Review Required
	-Description:		Shows newly created qualifications which have not been seen by the system
						previously, these are flagged in the Lifecycle Stage as 'New'
	-Date of Creation:	31/01/2025
	-Created By:		Adam Leaver (Fujitsu)
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		31/12/2025			Original
##################################################################################################*/

select	qual.Qan as QualificationReference
		,ao.NameOfqual as AwardingOrganisation
		,qual.QualificationName as QualificationTitle
		,ver.Type as QualificationType
		,ver.Level as Level
		,case 
					when ver.PreSixteen = 1 or ver.SixteenToEighteen = 1 or ver.EighteenPlus = 1 or ver.NineteenPlus = 1 then
						LEFT(
							case when ver.PreSixteen = 1 then 'Pre 16, ' else '' end +
							case when ver.SixteenToEighteen = 1 then '16 - 18, ' else '' end +
							case when ver.EighteenPlus = 1 then '18+, ' else '' end +
							case when ver.NineteenPlus = 1 then '19+, ' else '' end,
							LEN(
								case when ver.PreSixteen = 1 then 'Pre 16, ' else '' end +
								case when ver.SixteenToEighteen = 1 then '16 - 18, ' else '' end +
								case when ver.EighteenPlus = 1 then '18+, ' else '' end +
								case when ver.NineteenPlus = 1 then '19+, ' else '' end
							) - 1
						)
					else null
				end as AgeGroup
		,ver.Specialism as Subject
		,ver.Ssa as SectorSubjectArea
		,ver.ProcessStatusId as ProcessStatusId
		

from dbo.Qualification qual

inner join		regulated.QualificationVersions ver  on qual.id = ver.QualificationId
inner join		regulated.VersionFieldChanges vfc on vfc.id = ver.VersionFieldChangesId
inner join		dbo.AwardingOrganisation ao on ao.Id = ver.AwardingOrganisationId
left outer join regulated.LifecycleStage lcs on lcs.id = ver.LifecycleStageId

where lcs.name = 'New'
and ver.Type != 'Alternative Academic Qualification'
and ver.Type != 'Technical Occupation Qualification'
and ver.EligibleForFunding = 1

GO
