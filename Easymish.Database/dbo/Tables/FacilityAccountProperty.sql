CREATE TABLE [dbo].[FacilityAccountProperty] (
    [FacilityAccountID] INT            NOT NULL,
    [PropertyName]      NVARCHAR (30)  NOT NULL,
    [Type]              TINYINT        CONSTRAINT [DF_FacilityAccountProperty_Type] DEFAULT ((0)) NOT NULL,
    [Order]             INT            CONSTRAINT [DF_FacilityAccountProperty_Order] DEFAULT ((0)) NOT NULL,
    [PropertyValue]     NVARCHAR (100) NULL,
    [Description]       NVARCHAR (500) NULL,
    [Active]            BIT            CONSTRAINT [DF_FacilityAccountProperty_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_FacilityAccountProperty] PRIMARY KEY CLUSTERED ([FacilityAccountID] ASC, [PropertyName] ASC),
    CONSTRAINT [FK_FacilityAccountProperty_FacilityAccount] FOREIGN KEY ([FacilityAccountID]) REFERENCES [dbo].[FacilityAccount] ([ID])
);

