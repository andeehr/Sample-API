USE [Sample]
GO

DECLARE @now DATETIME = GETDATE();

-- add example roles
INSERT INTO [dbo].[Role]
           ([Description]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[DeletedAt])
     VALUES
           ('SuperUser'
           ,@now
           ,NULL
           ,NULL),
		   ('Administrator'
           ,@now
           ,NULL
           ,NULL)

-- add example permissions
INSERT INTO [dbo].[Permission]
           ([Description]
           ,[CreatedAt]
           ,[UpdatedAt]
           ,[DeletedAt])
     VALUES
           ('user.list'
           ,@now
           ,NULL
           ,NULL),
		   ('user.manage'
           ,@now
           ,NULL
           ,NULL)

-- add example relationship role/permission
DECLARE @SuperUserId INT;
DECLARE @AdminId INT;

SELECT @SuperUserId = Id
FROM [dbo].[Role]
WHERE Description = 'SuperUser';

SELECT @AdminId = Id
FROM [dbo].[Role]
WHERE Description = 'Administrator';

INSERT INTO [dbo].[PermissionRole] (PermissionsId, RolesId)
SELECT p.Id, @SuperUserId
FROM [dbo].[Permission] p;

INSERT INTO [dbo].[PermissionRole] (PermissionsId, RolesId)
SELECT p.Id, @AdminId
FROM [dbo].[Permission] p
WHERE p.Description = 'user.manage'

GO


