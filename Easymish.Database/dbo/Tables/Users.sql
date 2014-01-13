CREATE TABLE [dbo].[Users] (
    [ID]                        INT            IDENTITY (1, 1) NOT NULL,
    [LoginName]                 NVARCHAR (100) NOT NULL,
    [PersonID]                  INT            NOT NULL,
    [Password]                  NVARCHAR (256) NOT NULL,
    [IsLockedOut]               BIT            CONSTRAINT [DF_Users_IsLockedOut] DEFAULT 0 NOT NULL,
    [Active]                    BIT            CONSTRAINT [DF_Users_Active] DEFAULT 1 NOT NULL,
    [LastLockedOutDate]         DATETIME       NULL,
    [LastLoginDate]             DATETIME       NULL,
    [LastPasswordChangeDate]    DATETIME       NULL,
    [FailedPasswordAttempCount] INT            CONSTRAINT [DF_Users_FailedPasswordAttempCount] DEFAULT 0 NOT NULL,
    [CreateDate]                DATETIME       CONSTRAINT [DF_Users_CreateDate] DEFAULT (getutcdate()) NOT NULL,
	[CreateBy]					INT NOT NULL,
    [LModifyDate]				DATETIME       CONSTRAINT [DF_Users_ModifyDate] DEFAULT (getutcdate()) NOT NULL,
	[ModifyBy]					INT NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([LoginName] ASC),
    CONSTRAINT [FK_Users_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person] ([ID]),
    CONSTRAINT [UQ_Users] UNIQUE NONCLUSTERED ([ID] ASC), 
    CONSTRAINT [FK_Users_CreateBy] FOREIGN KEY ([CreateBy]) REFERENCES [Users](ID), 
    CONSTRAINT [FK_Users_ModifyBy] FOREIGN KEY ([ModifyBy]) REFERENCES [Users](ID)
);

