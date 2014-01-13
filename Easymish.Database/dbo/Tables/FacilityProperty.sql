CREATE TABLE [dbo].[FacilityProperty] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [FacilityID]    INT            NOT NULL,
    [PropertyName]  NVARCHAR (30)  NOT NULL,
    [Type]          TINYINT        CONSTRAINT [DF_FacilityProperty_Type] DEFAULT ((0)) NOT NULL,
    [Order]         INT            CONSTRAINT [DF_FacilityProperty_Order] DEFAULT ((0)) NOT NULL,
    [PropertyValue] NVARCHAR (100) NULL,
    [Description]   NVARCHAR (500) NULL,
    [Active]        BIT            CONSTRAINT [DF_FacilityProperty_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_FacilityProperty] PRIMARY KEY CLUSTERED ([FacilityID] ASC, [PropertyName] ASC),
    CONSTRAINT [FK_FacilityProperty_Facility] FOREIGN KEY ([FacilityID]) REFERENCES [dbo].[Facility] ([ID])
);

