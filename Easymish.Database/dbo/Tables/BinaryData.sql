CREATE TABLE [dbo].[BinaryData] (
    [ID]   INT             IDENTITY (1, 1) NOT NULL,
    [Data] VARBINARY (MAX) NULL,
    CONSTRAINT [PK_BinaryData] PRIMARY KEY CLUSTERED ([ID] ASC)
);

