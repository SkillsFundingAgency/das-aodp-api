CREATE procedure [regulated].[proc_MS_KPI_RQR_CR_CT_001] 

/*##################################################################################################
	-Name:				proc_MS_KPI_RQR_CR_CT_001
	-Description:		Decision Outcome within 4-week review cycle (excluding ‘hold’ status)

						KPI Indicator Type - Completion Rate
						Baseline - 60%
						Target - > 70%
						
	-Date of Creation:	26/02/2025
	-Created By:		Adam Leaver (Fujitsu)
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		06/03/2025			Original
##################################################################################################*/
As

DECLARE @QualificationId AS UNIQUEIDENTIFIER
Declare @maxVersion as int
Declare @maxVersionLatest as Int
Declare @temp table (
	QualificationID uniqueidentifier
	,Qan varchar(50)
	,QualificationName Varchar(max)
	,QualificationVersion int
	,InsertedTimeStamp datetime
	,Duration int
	,IsOutcomeDecision int
	,ProcessStatus varchar(20)
	,CurrentlyUnderReview varchar(20)
	,LatestVersion int
)

--Removed exisiting data from table
truncate table regulated.MS_KPI_RQR_CR_CT_001;

DECLARE cur_qual CURSOR READ_ONLY
FOR
		(
			SELECT distinct Q.Id AS QualificationId
			FROM dbo.Qualification Q
			INNER JOIN regulated.QualificationVersions QV ON QV.QualificationId = Q.Id
		)

OPEN cur_qual

FETCH NEXT FROM cur_qual INTO @QualificationId

WHILE @@FETCH_STATUS = 0
BEGIN
	--Loop through each qualification and get the duration between each outcome status

			--Find Max latest version with outcome
			select @maxVersion = max(version) from regulated.QualificationVersions QV
			Inner Join regulated.ProcessStatus PS on PS.Id = QV.ProcessStatusId
			Where QualificationId = @QualificationId and PS.IsOutcomeDecision = 1

			--Find Max latest version no exclusions
			select @maxVersionLatest = max(version) from regulated.QualificationVersions QV
			Where QualificationId = @QualificationId
			
			--Process current qualification id
			Insert into @temp
			SELECT Q.Id AS QualificationId
				,Q.Qan
				,Q.QualificationName
				,QV.Version AS QualificationVersion
				,QV.InsertTimestamp
				,DATEDIFF(HOUR,LAG(QV.InsertTimestamp) over(ORDER BY QV.Version ASC),QV.InsertTimestamp) AS Duration
				,PS.IsOutcomeDecision
				,PS.Name as ProcessStatus
				,Case When LEAD(PS.IsOutcomeDecision) Over (Partition By Q.Id Order By QV.Version ASC) =0
					  AND  PS.IsOutcomeDecision = 1
					  AND  QV.Version = @maxVersion
					  THEN 'On-Going'	  
				 Else ''	
				 End as CurrentlyUnderReview
				,Case when QV.Version = @maxVersionLatest Then 1
					  Else 0

				 End as LatestVersion
			FROM dbo.Qualification Q
			INNER JOIN regulated.QualificationVersions QV ON QV.QualificationId = Q.Id
			INNER JOIN regulated.ProcessStatus PS ON PS.Id = QV.ProcessStatusId
			WHERE Q.Id = @QualificationId
			ORDER BY QV.Version ASC

	FETCH NEXT FROM cur_qual INTO @QualificationId
END

Close cur_qual
Deallocate cur_qual

Insert Into regulated.MS_KPI_RQR_CR_CT_001 (
		 Qan
		,QualificationName
		,VersionNumber_StartReviewCycle
		,VersionNumber_EndReviewCycle
		,Status
		,IsOutcomeDecision
		,Duration_Hours
		,InsertedTimestamp
		)
select Qan
		,QualificationName
		
		,Case 
			When IsOutcomeDecision = 1 and QualificationVersion = 1 then 1
			When IsOutcomeDecision = 1 and QualificationVersion <> 1 Then 
			Lag(QualificationVersion) Over (Partition By QualificationId Order By QualificationVersion ASC) + 1
			When IsOutcomeDecision = 0 then
			Lag(QualificationVersion) Over (Partition By QualificationId Order By QualificationVersion ASC) + 1
			
			
		 End as VersionNumber_StartReviewCycle
		 ,QualificationVersion as VersionNumber_EndReviewCycle
		,ProcessStatus as Status
		,IsOutcomeDecision
		,Duration as Duration_Hours
		,InsertedTimestamp

from @temp 
Where IsOutcomeDecision = 1 or LatestVersion = 1
order by QualificationID, QualificationVersion asc 



GO