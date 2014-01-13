CREATE TABLE [dbo].[Address] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [Address]    NVARCHAR (100) NULL,
    [City]       NVARCHAR (30)  NULL,
    [Province]   NVARCHAR (40)  NULL,
    [CountryCode]    CHAR (2)       NULL,
    [PostalCode] NVARCHAR (24)  NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_Address_Country] FOREIGN KEY ([CountryCode]) REFERENCES [Country]([Code])
);

