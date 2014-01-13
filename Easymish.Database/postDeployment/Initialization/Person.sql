DECLARE @idoc int 
DECLARE @data varchar(MAX)

SET @data = '
<items>
  <item>
    <FirstName>User</FirstName>
    <LastName>System</LastName>
  </item>
</items>'


EXEC sp_xml_preparedocument @idoc OUTPUT, @data

INSERT INTO [Person](FirstName, LastName)
SELECT   * 
FROM     OPENXML (@idoc, 'items/item',2) 
            WITH (
            FirstName nvarchar(100),
			LastName VARCHAR(100))

GO
