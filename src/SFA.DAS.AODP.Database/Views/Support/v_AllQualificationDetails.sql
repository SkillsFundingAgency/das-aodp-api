CREATE View [dbo].[v_AllQualificationDetails] as

/*##################################################################################################
	-Name:				All Qualification Details
	-Description:		View for showing all Qualifications
						
	-Date of Creation:	07/04/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		07/04/2025			Original
##################################################################################################*/

With CTE_QualificationDetails AS (
Select QV.Id as QualificationVersionId
		,QV.QualificationId
		,LS.Name As LifecycleStatus
		,AO.Ukprn AS UKPRN
		,AO.NameLegal AS AOLegalName
		,PS.Name as ProcessStatus
		,QV.Type AS QualificationType
		,QV.Ssa AS SectorSubjectArea
		,QV.PreSixteen
		,QV.SixteenToEighteen
		,QV.EighteenPlus
		,QV.NineteenPlus
		,QV.Level
		,RANK() OVER (Partition By QV.Version Order By Version Desc) as R_K


From regulated.QualificationVersions QV
Inner Join regulated.ProcessStatus PS ON PS.Id = QV.ProcessStatusId
Inner Join regulated.LifecycleStage LS ON LS.Id = QV.LifecycleStageId
Inner Join dbo.AwardingOrganisation AO ON AO.Id = QV.AwardingOrganisationId
Where PS.IsOutcomeDecision = 1
),
CTE_AggregatedOffers AS (


Select QF.QualificationVersionId
	   ,STRING_AGG(FO.Name, ', ') AS OfferList
		
from funded.QualificationFundings QF
Inner Join dbo.FundingOffers FO ON FO.Id = QF.FundingOfferId
Group By QF.QualificationVersionId


)

SELECT Q.Qan
      ,Q.QualificationName
	  ,QD.AOLegalName
	  ,QD.UKPRN
	  ,QD.LifecycleStatus
	  ,QD.ProcessStatus
	  ,QualificationType
	  ,QD.SectorSubjectArea
	  ,PreSixteen
	  ,QD.SixteenToEighteen
	  ,QD.EighteenPlus
	  ,QD.NineteenPlus
	  ,QD.Level
	  ,AGG.OfferList
	  

  FROM dbo.Qualification Q
  Left Outer Join CTE_QualificationDetails QD ON Q.Id = QD.QualificationId
  Left Outer Join CTE_AggregatedOffers AGG ON AGG.QualificationVersionId = QD.QualificationVersionId
  Where QD.R_K = 1