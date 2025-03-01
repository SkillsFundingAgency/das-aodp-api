CREATE view [regulated].[v_QualificationLastestVersion] as

/*##################################################################################################
	-Name:				Qualification Lastest Outcome
	-Description:		Shows latest version for each qualifications

	-Date of Creation:	06/02/2025
	-Created By:		Adam Leaver (Fujitsu)
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		06/02/2025			Original
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
		,agg.QualificationVersionNumber

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
				,vfc.QualificationVersionNumber as QualificationVersionNumber
				,Rank() OVER (PARTITION BY qual.Qan Order by vfc.QualificationVersionNumber desc) as r_n
		

		from dbo.Qualification qual

		inner join		regulated.QualificationVersions ver  on qual.id = ver.QualificationId
		inner join		regulated.VersionFieldChanges vfc on vfc.id = ver.VersionFieldChangesId
		inner join		dbo.AwardingOrganisation ao on ao.Id = ver.AwardingOrganisationId
		left outer join regulated.LifecycleStage lcs on lcs.id = ver.LifecycleStageId
		
	) agg

Where agg.r_n = 1


GO
