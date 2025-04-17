﻿CREATE View [dbo].[v_Anomaly_Scenario_5_Dashboard] as

/*##################################################################################################
	-Name:				v_Anomaly_Scenario_5_Dashboard
	-Description:		View for Funded Quals Import Scenario #5

						
	-Date of Creation:	17/04/2025
	-Created By:		Adam Leaver
####################################################################################################
	Version No.			Updated By		Updated Date		Description of Change
####################################################################################################
	1					Adam Leaver		17/04/2025			Original
##################################################################################################*/

SELECT 		q.Qan, 
			q.QualificationName, 
			qv.OperationalEndDate,
			qv.CertificationEndDate,
			ls.Name as LifecycleStatus, 
			ps.Name as ProcessStatus
FROM [dbo].[v_Anomaly_Import_Scenario_5] ai
INNER JOIN dbo.Qualification q ON q.Id = ai.QualificationId
INNER JOIN regulated.v_QualificationLastestVersion lqv ON q.Qan = lqv.QualificationReference
INNER JOIN regulated.QualificationVersions qv ON q.Id = qv.QualificationId
INNER JOIN regulated.LifecycleStage ls On ls.id = qv.LifecycleStageId 
INNER JOIN regulated.ProcessStatus ps On ps.id = qv.ProcessStatusId
GO