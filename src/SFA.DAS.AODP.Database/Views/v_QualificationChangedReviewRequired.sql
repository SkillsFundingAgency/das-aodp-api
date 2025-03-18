CREATE view [regulated].[v_QualificationChangedReviewRequired] as

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
	2					Faisal ashraf   05/03/2025			Added vfc.Changefieldnames
	3					Adam Leaver		05/03/2025			Inclusion of Status column
	4					Faisal Ashraf	16/03/2025			Included statusID column
##################################################################################################*/

select 
		agg.QualificationReference
		,agg.AwardingOrganisation
		,agg.QualificationTitle
		,agg.QualificationType
		,agg.Level
		,agg.AgeGroup
		,agg.Subject
		,agg.SectorSubjectArea
		,agg.ChangedFieldNames
		,agg.Status
		,agg.StatusId
 
from (
 
		select	qual.Qan as QualificationReference
				,ao.NameLegal as AwardingOrganisation
				,qual.QualificationName as QualificationTitle
				,ver.Type as QualificationType
				,ver.Level as Level
				,case when ver.PreSixteen = 1 then '< 16'
					  when ver.SixteenToEighteen = 1 then '16 - 18'
					  when ver.EighteenPlus = 1 then '18+'
					  When ver.NineteenPlus = 1 then '19+'
					  else null
				end as AgeGroup
				,ver.Specialism as Subject
				,ver.Ssa as SectorSubjectArea
				,vfc.ChangedFieldNames as ChangedFieldNames
				
				,Rank() OVER (PARTITION BY qual.Qan Order by vfc.QualificationVersionNumber desc) as r_n
				,PS.Name as Status
				,PS.Id as StatusId
 
		from dbo.Qualification qual
 
		inner join		regulated.QualificationVersions ver  on qual.id = ver.QualificationId
		inner join		regulated.VersionFieldChanges vfc on vfc.id = ver.VersionFieldChangesId
		inner join		dbo.AwardingOrganisation ao on ao.Id = ver.AwardingOrganisationId
		left outer join regulated.LifecycleStage lcs on lcs.id = ver.LifecycleStageId
		left Outer Join regulated.ProcessStatus PS on PS.Id = ver.ProcessStatusId
 
		where lcs.name = 'Changed' ) agg
 
Where agg.r_n = 1

GO
