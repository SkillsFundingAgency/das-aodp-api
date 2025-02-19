CREATE TABLE [dbo].[MigrationScripts] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [RunDate]      DATETIME       NOT NULL,  
    CONSTRAINT [PK_MigrationScripts] PRIMARY KEY CLUSTERED ([Id] ASC)
    );