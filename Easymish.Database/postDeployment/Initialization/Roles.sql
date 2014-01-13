DECLARE @idoc int 
DECLARE @data varchar(MAX)

SET @data = '
<rows>
  <row ID="1" Name="System" Description="System use" />
  <row ID="2" Name="Administrator" Description="Administrator" />
  <row ID="3" Name="User" Description="General user" />
</rows>
'

EXEC sp_xml_preparedocument @idoc OUTPUT, @data

INSERT INTO Roles(ID, Name, Description)
SELECT   * 
FROM     OPENXML (@idoc, 'rows/row',1) 
            WITH (
            ID tinyint,
			Name VARCHAR(100),
			Description nvarchar(500))

GO
