CREATE TABLE [dbo].Country (
	[Code]         CHAR(2)            NOT NULL,
	Alpha3Code CHAR(3) NOT NULL,
	NumericCode SMALLINT NULL,
    [Name]    NVARCHAR (100) NOT NULL,
    [PhonePrefix] VARCHAR(5) NULL, 
    [DomainExtension] VARCHAR(3) NULL, 
    [ContinentID] TINYINT NULL, 
	[Active] BIT  NOT NULL CONSTRAINT [DF_Country_Active] DEFAULT 0,
    CONSTRAINT [FK_Country_Continent] FOREIGN KEY ([ContinentID]) REFERENCES [Continent]([ID]), 
    CONSTRAINT [PK_Country] PRIMARY KEY ([Code]) 
);


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Alpha-2 Code',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Country',
    @level2type = N'COLUMN',
    @level2name = N'Code'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Alpha-3 Code',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Country',
    @level2type = N'COLUMN',
    @level2name = N'Alpha3Code'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'ISO 3166-1 numeric',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Country',
    @level2type = N'COLUMN',
    @level2name = N'NumericCode'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Country full name',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Country',
    @level2type = N'COLUMN',
    @level2name = N'Name'
GO

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'Phone prefix',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Country',
    @level2type = N'COLUMN',
    @level2name = N'PhonePrefix'
GO

CREATE UNIQUE INDEX [IX_Country_Alpha3Code] ON [dbo].[Country] ([Alpha3Code])
GO


CREATE INDEX [IX_Country_Continent] ON [dbo].[Country] ([ContinentID])
