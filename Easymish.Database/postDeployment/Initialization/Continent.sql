DECLARE @idoc int 
DECLARE @data varchar(MAX)

SET @data = '
<items>
  <item>
    <ID>1</ID>
    <Name>Africa</Name>
  </item>
  <item>
    <ID>2</ID>
    <Name>Antarctica</Name>
  </item>
  <item>
    <ID>3</ID>
    <Name>Asia</Name>
  </item>
  <item>
    <ID>4</ID>
    <Name>Europe</Name>
  </item>
  <item>
    <ID>5</ID>
    <Name>North America</Name>
  </item>
  <item>
    <ID>6</ID>
    <Name>Oceania</Name>
  </item>
  <item>
    <ID>7</ID>
    <Name>South America</Name>
  </item>
</items>
'

EXEC sp_xml_preparedocument @idoc OUTPUT, @data

INSERT INTO Continent(ID, Name)
SELECT   * 
FROM     OPENXML (@idoc, 'items/item',2) 
            WITH (
            ID tinyint,
			Name VARCHAR(50))

GO
