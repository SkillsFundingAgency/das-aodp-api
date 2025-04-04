CREATE View [dbo].[v_Anomaly_Import_Scenario_5] as

/*##################################################################################################
	-Name:				Anomaly_Import_Scenario_5
	-Description:		View for Funded Quals Import Scenario #5

						User Need:		As a member of the support team, I want to be able to 
						see qualifications not marked a Funded in AOdP that do not appear on the 
						OFQUAL register but are in the Funded qualifications file so that I can 
						manually investigate and ascertain the true status of that qualification 
						and validate the information held in AoDP.
						
						Acceptance Criteria:	Given that all records are selected from the 
						view then all columns of those qualifications held by AOdP with an 
						outcome status of “Not Approved” that are present in the latest funded 
						qualification import and are not current OFQUAL regulated qualifications 
						are returned
						
	-Date of Creation:	03/04/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		03/04/2025			Original
##################################################################################################*/

/*List of current funded qualifications*/
With CTE_CurrFundedQualifications AS (
Select QualificationId

From funded.Qualifications
),
/*List of latest regualted qualifications*/
CTE_LatestRegulatedVersion AS (

SELECT  QV.QualificationId
		,PS.Name
		,QV.OperationalEndDate
		,RANK() Over (Partition By QV.Version Order By QV.Version Desc) as R_K
FROM regulated.QualificationVersions QV
Inner Join regulated.ProcessStatus PS on PS.Id = QV.ProcessStatusId
),
/*List of Qualification Ids that are the latest regulated version, have not approved status
and have an Operational end date in the past*/
CTE_LRV_PastOpEndDate As (
Select QualificationId

From CTE_LatestRegulatedVersion
Where R_K = 1
AND	Name = 'Rejected'
AND OperationalEndDate <= GetDate()
)

/*List of latest regualted qualifications which dont appear in the funded based on criteria above*/
Select REG.QualificationId from CTE_LRV_PastOpEndDate REG
Inner Join CTE_CurrFundedQualifications FUN ON FUN.QualificationId = REG.QualificationId