CREATE TABLE [dbo].[Facility] (
    [ID]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [FullName]       NVARCHAR (100) NULL,
    [HeadquarterID]  INT            NULL,
    [Type]           INT            CONSTRAINT [DF_Facility_Type] DEFAULT ((0)) NOT NULL,
    [Website]        VARCHAR (100)  NULL,
    [Comment]        NTEXT          NULL,
    [CreateDate]                DATETIME       CONSTRAINT [DF_Facility_CreateDate] DEFAULT (getutcdate()) NOT NULL,
	[CreateBy]					INT NOT NULL,
    [LModifyDate]				DATETIME       CONSTRAINT [DF_Facility_LastUpdateDate] DEFAULT (getutcdate()) NOT NULL,
	[ModifyBy]					INT NOT NULL,
    CONSTRAINT [PK_Facility] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_Facility_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [FK_Facility_CreateBy] FOREIGN KEY ([CreateBy]) REFERENCES [Users](ID), 
    CONSTRAINT [FK_Facility_ModifyBy] FOREIGN KEY ([ModifyBy]) REFERENCES [Users](ID)
);

