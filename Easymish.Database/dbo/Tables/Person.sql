CREATE TABLE [dbo].[Person] (
    [ID]             INT IDENTITY (1, 1) NOT NULL,
    [FirstName]      NVARCHAR (100) NULL,
    [LastName]       NVARCHAR (100) NULL,
	[Title]			nvarchar(20) NULL,
	[Phone]			nvarchar(20) NULL,
	[Email]			nvarchar(256) NULL,
    [AddressID]		int,
    [FacilityID]     INT            NULL,
    [CreateDate]     DATETIME       CONSTRAINT [DF_Person_CreateDate] DEFAULT (getutcdate()) NOT NULL,
    [LastUpdateDate] DATETIME       CONSTRAINT [DF_Person_LastUpdateDate] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [CK_Person] CHECK ([FirstName] IS NOT NULL OR [LastName] IS NOT NULL),
    CONSTRAINT [FK_Person_FacilityID] FOREIGN KEY ([FacilityID]) REFERENCES [dbo].[Facility] ([ID]), 
    CONSTRAINT [FK_Person_Address] FOREIGN KEY ([AddressID]) REFERENCES [Address]([ID]) 
);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'first name or last name cannot be empty at the same time', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'Person', @level2type = N'CONSTRAINT', @level2name = N'CK_Person';


GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'The title to call this person. (e.g) Sir, Madam...',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Person',
    @level2type = N'COLUMN',
    @level2name = N'Title'