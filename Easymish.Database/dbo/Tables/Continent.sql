CREATE TABLE [dbo].[Continent]
(
	[ID] TINYINT NOT NULL, 
    [Name] VARCHAR(50) NOT NULL, 
	CONSTRAINT [PK_Continent] PRIMARY KEY NONCLUSTERED ([ID] ASC)
)

GO

CREATE CLUSTERED INDEX [IX_Continent_Name] ON [dbo].[Continent] ([Name])
