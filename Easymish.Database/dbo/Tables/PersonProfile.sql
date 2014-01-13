CREATE TABLE [dbo].[PersonProfile] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [PersonID]      INT            NOT NULL,
    [Type]          TINYINT        DEFAULT ((0)) NOT NULL,
    [PropertyName]  NVARCHAR (30)  NOT NULL,
    [PropertyValue] NVARCHAR (256) NULL,
    [Description]   NVARCHAR (500) NULL,
    [Order]         INT            DEFAULT ((0)) NOT NULL,
    [Active]        BIT            CONSTRAINT [DF_PersonProfile_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_PersonProfile] PRIMARY KEY NONCLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PersonProfile_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Person] ([ID])
);


GO
CREATE CLUSTERED INDEX [IDX_PersonProfile]
    ON [dbo].[PersonProfile]([PersonID] ASC, [Type] ASC, [PropertyName] ASC);

