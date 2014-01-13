CREATE TABLE [dbo].[Roles] (
    [ID]          TINYINT            NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [Description] NVARCHAR (500) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([ID] ASC)
);

