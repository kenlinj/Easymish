CREATE TABLE [dbo].[AccountTransaction] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [FacilityAccountID] INT             NULL,
    [Description]       VARCHAR (100)   NULL,
    [FromDate]          DATETIME        NULL,
    [ToDate]            DATETIME        NULL,
    [Debit]             DECIMAL (12, 4) NULL,
    [Credit]            DECIMAL (12, 4) NULL,
    [CreateDate]        DATETIME        CONSTRAINT [DF_AccountTransaction_CreateDate] DEFAULT (getutcdate()) NOT NULL,
    [LastUpdateDate]    DATETIME        CONSTRAINT [DF_AccountTransaction_LastUpdateDate] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_AccountTransaction] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_AccountTransaction_FacilityAccount] FOREIGN KEY ([FacilityAccountID]) REFERENCES [dbo].[FacilityAccount] ([ID])
);

