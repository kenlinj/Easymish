DECLARE @idoc int 
DECLARE @data varchar(MAX)

SET @data = '
<items>
  <item>
    <LoginName>SystemUser</LoginName>
    <PersonID>1</PersonID>
    <Password>Password1</Password>
    <CreateBy>1</CreateBy>
    <ModifyBy>1</ModifyBy>
  </item>
</items>'


EXEC sp_xml_preparedocument @idoc OUTPUT, @data

INSERT INTO [Users](LoginName, PersonID, [Password], CreateBy, ModifyBy)
SELECT   * 
FROM     OPENXML (@idoc, 'items/item', 2) 
            WITH (
            LoginName nvarchar(100),
			PersonID int,
			[Password] nvarchar(256),
			CreateBy int,
			ModifyBy int)

GO
