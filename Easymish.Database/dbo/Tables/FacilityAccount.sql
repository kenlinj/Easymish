CREATE TABLE [dbo].[FacilityAccount] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [FacilityID]     INT           NOT NULL,
    [UserName]       NVARCHAR (50) NOT NULL,
    [Password]       VARCHAR (128) NULL,
    [UserID]         INT           NULL,
    [Title]          NVARCHAR (50) NULL,
    [StartDate]      DATETIME      NULL,
    [EndDate]        DATETIME      NULL,
    [CreateDate]                DATETIME      CONSTRAINT [DF_FacilityAccount_CreateDate] DEFAULT (getutcdate()) NOT NULL,
	[CreateBy]					INT NOT NULL,
    [LModifyDate]				DATETIME       CONSTRAINT [DF_FacilityAccount_LastUpdateDate] DEFAULT (getutcdate()) NOT NULL,
	[ModifyBy]					INT NOT NULL,
    CONSTRAINT [UQ_FacilityAccount] PRIMARY KEY CLUSTERED ([FacilityID] ASC, [UserName] ASC),
    CONSTRAINT [FK_FacilityAccount_Facility] FOREIGN KEY ([FacilityID]) REFERENCES [dbo].[Facility] ([ID]),
    CONSTRAINT [FK_FacilityAccount_Users] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Person] ([ID]),
    CONSTRAINT [PK_FacilityAccount] UNIQUE NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_FacilityAccount_CreateBy] FOREIGN KEY ([CreateBy]) REFERENCES [Users](ID), 
    CONSTRAINT [FK_FacilityAccount_ModifyBy] FOREIGN KEY ([ModifyBy]) REFERENCES [Users](ID)
);

