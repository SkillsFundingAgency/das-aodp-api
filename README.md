## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# AODP API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## Requirements

- A clone of this repository
- SQL Server database
- Azurite

## About

das-aodp-api represents the inner api definition for AODP.

## Local running
### Config
You can find the latest config file in [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-aodp-api/SFA.DAS.AODP.Api.json)


* If you are using Azure Storage Emulator for local development purpose, then In your Azure Storage Account, create a table called Configuration and Add the following

ParitionKey: LOCAL  
RowKey: SFA.DAS.AODP.Api_1.0  
Data:  
```json
{
  // content from latest config file
}
```

### SQL Server database
You are able to create the database by doing the following:

* Run the database deployment publish command to create the database ```SFA.DAS.AODP``` or create the database manually and run in the table creation scripts

