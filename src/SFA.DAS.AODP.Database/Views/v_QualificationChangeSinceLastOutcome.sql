CREATE view [regulated].[v_QualificationChangeSinceLastOutcome] as

/*##################################################################################################
	-Name:				Qualification Change Since Last Outcome
	-Description:		Shows columns which have changed since outcome for each qualifications

	-Date of Creation:	10/02/2025
	-Created By:		Adam Leaver (Fujitsu)
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		10/02/2025			Original
##################################################################################################*/

With cte_lastOutcome
as (

select 
		 agg.QualificationReference_curr
		,agg.AwardingOrganisation_curr
		,agg.QualificationTitle_curr
		,agg.QualificationType_curr
		,agg.Level_curr
		,agg.AgeGroup_curr
		,agg.Subject_curr
		,agg.SectorSubjectArea_curr

from (

		select	qual.Qan as QualificationReference_curr
				,ao.NameLegal as AwardingOrganisation_curr
				,qual.QualificationName as QualificationTitle_curr
				,ver.Type as QualificationType_curr
				,ver.Level as Level_curr
				,case when ver.PreSixteen = 1 then '< 16'
					  when ver.SixteenToEighteen = 1 then '16 - 18'
					  when ver.EighteenPlus = 1 then '18+'
					  When ver.NineteenPlus = 1 then '19+'
					  else null
				end as AgeGroup_curr
				,ver.Specialism as Subject_curr
				,ver.Ssa as SectorSubjectArea_curr
				,Rank() OVER (PARTITION BY qual.Qan Order by vfc.QualificationVersionNumber desc) as r_n
		

		from dbo.Qualification qual

		inner join		regulated.QualificationVersions ver  on qual.id = ver.QualificationId
		inner join		regulated.VersionFieldChanges vfc on vfc.id = ver.VersionFieldChangesId
		inner join		dbo.AwardingOrganisation ao on ao.Id = ver.AwardingOrganisationId
		inner join		regulated.ProcessStatus ps on ps.Id = ver.ProcessStatusId
		left outer join regulated.LifecycleStage lcs on lcs.id = ver.LifecycleStageId

		where ps.Name in ('No Action Required','Approved','Not Approved')
		
	) agg

Where agg.r_n = 1

),
cte_propentVersion
as
(
select 
		 agg.QualificationReference_prop
		,agg.AwardingOrganisation_prop
		,agg.QualificationTitle_prop
		,agg.QualificationType_prop
		,agg.Level_prop
		,agg.AgeGroup_prop
		,agg.Subject_prop
		,agg.SectorSubjectArea_prop
		,agg.QualificationVersionNumber_prop

from (

		select	qual.Qan as QualificationReference_prop
				,ao.NameLegal as AwardingOrganisation_prop
				,qual.QualificationName as QualificationTitle_prop
				,ver.Type as QualificationType_prop
				,ver.Level as Level_prop
				,case when ver.PreSixteen = 1 then '< 16'
					  when ver.SixteenToEighteen = 1 then '16 - 18'
					  when ver.EighteenPlus = 1 then '18+'
					  When ver.NineteenPlus = 1 then '19+'
					  else null
				end as AgeGroup_prop
				,ver.Specialism as Subject_prop
				,ver.Ssa as SectorSubjectArea_prop
				,vfc.QualificationVersionNumber as QualificationVersionNumber_prop
				,Rank() OVER (PARTITION BY qual.Qan Order by vfc.QualificationVersionNumber desc) as r_n
		

		from dbo.Qualification qual

		inner join		regulated.QualificationVersions ver  on qual.id = ver.QualificationId
		inner join		regulated.VersionFieldChanges vfc on vfc.id = ver.VersionFieldChangesId
		inner join		dbo.AwardingOrganisation ao on ao.Id = ver.AwardingOrganisationId
		left outer join regulated.LifecycleStage lcs on lcs.id = ver.LifecycleStageId
		
	) agg

Where agg.r_n = 1
)

select 
		 QualificationReference_prop
		,QualificationReference_curr

		,Case	when AwardingOrganisation_prop = AwardingOrganisation_curr 
				then null 
				else 'Current: ' +  AwardingOrganisation_curr + char(10) + 'Proposed: ' + AwardingOrganisation_prop
		 end as AwardingOrganisationChanges

		,Case	when QualificationTitle_prop = QualificationTitle_curr 
				then null 
				else 'Current: ' +  QualificationTitle_curr + char(10) + 'Proposed: ' + QualificationTitle_prop
		 end as QualificationTitleChanges
		 
		,Case	when QualificationType_prop = QualificationType_curr 
				then null 
				else 'Current: ' +  QualificationType_curr + char(10) + 'Proposed: ' + QualificationType_prop
		 end as QualificationTypeChanges

		,Case	when Level_prop = Level_curr 
				then null 
				else 'Current: ' +  Level_curr + char(10) + 'Proposed: ' + Level_prop
		 end as LevelChanges

		,Case	when AgeGroup_prop = AgeGroup_curr 
				then null 
				else 'Current: ' +  AgeGroup_curr + char(10) + 'Proposed: ' + AgeGroup_prop
		 end as AgeGroupChanges

		,Case	when Subject_prop = Subject_curr 
				then null 
				else 'Current: ' +  Subject_curr + char(10) + 'Proposed: ' + Subject_prop
		 end as SubjectChanges

		,Case	when SectorSubjectArea_prop = SectorSubjectArea_curr 
				then null 
				else 'Current: ' +  SectorSubjectArea_curr + char(10) + 'Proposed: ' + SectorSubjectArea_prop
		 end as SectorSubjectAreaChanges

from cte_lastOutcome lo
left outer join cte_propentVersion cv on cv.QualificationReference_prop = lo.QualificationReference_curr

Where lo.QualificationReference_curr is not null

GO
