CREATE TABLE [dbo].[UsersInRoles] (
    [UserID] INT NOT NULL,
    [RoleID] TINYINT NOT NULL,
    CONSTRAINT [PK_UsersInRoles] PRIMARY KEY CLUSTERED ([UserID] ASC, [RoleID] ASC),
    CONSTRAINT [FK_UsersInRoles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Roles] ([ID]),
    CONSTRAINT [FK_UsersInRoles_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([ID])
);

