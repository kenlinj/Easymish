using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NeatCode.Library.Data;
using System.Data;
using System.Text.RegularExpressions;

namespace ConsoleApplication1.SCI
{
    class InitDatabase
    {
        public static void InitDb()
        {
            //RemoveFileNameTail(_RootPath);
            //FormatScript(_RootPath, "ResponseDriver_OLD");
            //CombineAllTalbeScripts(_RootPath, _ConnectionString);

            //CombineAllStoreProcedureScripts(_RootPath);
            //CombineAllFunctions(_RootPath);
            //CombineAllViews(_RootPath);
            CombineAllInit(_RootPath);

            //RemoveFileNameTail(_RootPath_I);
            //FormatScript(_RootPath_I, "ResponseDriver_I");

            //CombineAllTalbeScripts(_RootPath_I, _ConnectionString_I);
            //CombineAllStoreProcedureScripts(_RootPath_I);
            //CombineAllFunctions(_RootPath_I);
            //CombineAllViews(_RootPath_I);
        }

        public static void GenerateAllRDScripts(string codeRootPath)
        {
            string conn = "server=10.22.12.43;uid=SCIToolTeam;pwd=SciTool!;database=ResponseDriver;";
            string script = "";
            string type = "";
            DataTable results = new DataTable();
            DataTable dt;
            DataTable dtTemp;

            results.Columns.Add("Type");
            results.Columns.Add("Name");
            results.Columns.Add("Script");

            DataAccess da = new DataAccess(conn, ProviderType.Sql);

            #region scripts except foreign key
            da.CommandType = System.Data.CommandType.Text;
            da.CommandText = "SELECT * FROM sys.objects ORDER BY [type], [name]";
            dt = da.GetDataTable();
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                i++;
                Console.WriteLine("Processing {0:d4}: {1}.{2}", i, row["Type"], row["Name"]);
                switch (row["Type"].ToString().Trim())
                {
                    case "D":   //default value is set in table script     
                    case "F":   //FOREIGN KEY
                    case "PK":  //primary key is set in table script
                    case "S":   //SYSTEM_TABLE
                        type = "";
                        break;
                    case "FN":  //SQL_SCALAR_FUNCTION
                    case "IF":  //SQL_INLINE_TABLE_VALUED_FUNCTION
                    case "TF":  //SQL_TABLE_VALUED_FUNCTION
                        type = "Functions";
                        break;
                    case "P":   //SQL_STORED_PROCEDURE
                        type = "Stored Procedures";
                        break;
                    case "TR":  //SQL_TRIGGER
                        type = "Triggers";
                        break;
                    case "V":   //VIEW
                        type = "Views";
                        break;
                    case "FS":  //CLR_SCALAR_FUNCTION
                        type = "CLR Functions";
                        break;
                    case "PC":  //CLR_STORED_PROCEDURE
                        type = "CLR Stored Procedures";
                        break;
                    case "U":
                        type = "Tables";
                        break;
                    case "UQ":
                        type = "Unique Index";
                        break;
                }

                switch (row["Type"].ToString().Trim())
                {
                    case "D":   //default value is set in table script                        
                    case "F":   //FOREIGN KEY
                    case "PK":  //primary key is set in table script
                    case "S":   //SYSTEM_TABLE
                    case "IT":
                    case "SQ":
                        script = null;
                        break;
                    case "FS":  //CLR_SCALAR_FUNCTION
                    case "PC":  //CLR_STORED_PROCEDURE
                        script = "Encrypted";
                        break;
                    case "FN":  //SQL_SCALAR_FUNCTION
                    case "IF":  //SQL_INLINE_TABLE_VALUED_FUNCTION
                    case "P":   //SQL_STORED_PROCEDURE
                    case "TF":  //SQL_TABLE_VALUED_FUNCTION
                    case "TR":  //SQL_TRIGGER
                    case "V":   //VIEW
                        //script = null;
                        script = "sp_helptext " + row["Name"].ToString();

                        da.CommandText = script;
                        dtTemp = da.GetDataTable();

                        script = "";
                        foreach (DataRow rowTemp in dtTemp.Rows)
                        {
                            script += rowTemp[0];
                        }
                        break;
                    case "U":
                        script =
                        #region generate create table script
 "declare @table sysname                                                                 " +

                            "set @table = '{0}'                                                    " +

                            "declare @sql table(s varchar(1000), id int identity)                                   " +


                            "insert into  @sql(s) values ('create table [' + @table + '] (')                        " +


                            "insert into @sql(s)                                                                    " +
                            "select                                                                                 " +
                            "    '  ['+column_name+'] ' +                                                           " +
                            "    data_type + coalesce('('+cast(character_maximum_length as varchar)+')','') + ' ' + " +
                            "    case when exists (                                                                 " +
                            "        select id from syscolumns                                                      " +
                            "        where object_name(id)=@table                                                   " +
                            "        and name=column_name                                                           " +
                            "        and columnproperty(id,name,'IsIdentity') = 1                                   " +
                            "    ) then                                                                             " +
                            "        'IDENTITY(' +                                                                  " +
                            "        cast(ident_seed(@table) as varchar) + ',' +                                    " +
                            "        cast(ident_incr(@table) as varchar) + ')'                                      " +
                            "    else ''                                                                            " +
                            "    end + ' ' +                                                                        " +
                            "    ( case when IS_NULLABLE = 'No' then 'NOT ' else '' end ) + 'NULL ' +               " +
                            "    coalesce('DEFAULT '+COLUMN_DEFAULT,'') + ','                                       " +

                            " from information_schema.columns where table_name = @table                             " +
                            " order by ordinal_position                                                             " +


                            "declare @pkname varchar(100)                                                           " +
                            "select @pkname = constraint_name from information_schema.table_constraints             " +
                            "where table_name = @table and constraint_type='PRIMARY KEY'                            " +

                            "if ( @pkname is not null ) begin                                                       " +
                            "    insert into @sql(s) values('  PRIMARY KEY (')                                      " +
                            "    insert into @sql(s)                                                                " +
                            "        select '   ['+COLUMN_NAME+'],' from information_schema.key_column_usage        " +
                            "        where constraint_name = @pkname                                                " +
                            "        order by ordinal_position                                                      " +

                            "    update @sql set s=left(s,len(s)-1) where id=@@identity                             " +
                            "    insert into @sql(s) values ('  )')                                                 " +
                            "end                                                                                    " +
                            "else begin                                                                             " +

                            "    update @sql set s=left(s,len(s)-1) where id=@@identity                             " +
                            "end                                                                                    " +

                            "insert into @sql(s) values( ')' )                                                      " +

                            "select s from @sql order by id                                                         "

                        #endregion
;
                        da.CommandText = string.Format(script, row["Name"]);
                        dtTemp = da.GetDataTable();

                        script = "";
                        foreach (DataRow rowTemp in dtTemp.Rows)
                        {
                            script += rowTemp["s"] + "\r\n";
                        }
                        break;
                    case "UQ":
                        script = null;
                        //script = string.Format("select * from information_schema.constraint_column_usage where CONSTRAINT_NAME = '{0}'", row["Name"].ToString());

                        //da.CommandText = script;
                        //dtTemp = da.GetDataTable();

                        //script = string.Format("CREATE TABLE [dbo].[{0}] ADD  CONSTRAINT [{1}] UNIQUE NONCLUSTERED (\r\n", dtTemp.Rows[0]["TABLE_NAME"], row["Name"]);
                        //foreach (DataRow rowTemp in dtTemp.Rows)
                        //{
                        //    script += rowTemp["COLUMN_NAME"] + " ASC,\r\n";
                        //}

                        //script = script.Remove(script.Length - 4, 3);
                        //script += ")";
                        break;
                    default:
                        throw new Exception("Invalid sys.objects type");
                }

                if (!string.IsNullOrEmpty(script))
                {
                    results.Rows.Add(type, row["Name"].ToString(), script);
                }
            }

            #endregion

            #region foreign key
            script =
            #region generate foreign key script
 "declare @sql table(id int identity, name varchar(1000), script varchar(1000))                                                                              " +
                               "                                                                                                                                  " +
                               "DECLARE @schema_name sysname;                                                                                                     " +
                               "                                                                                                                                  " +
                               "DECLARE @table_name sysname;                                                                                                      " +
                               "                                                                                                                                  " +
                               "DECLARE @constraint_name sysname;                                                                                                 " +
                               "                                                                                                                                  " +
                               "DECLARE @constraint_object_id int;                                                                                                " +
                               "                                                                                                                                  " +
                               "DECLARE @referenced_object_name sysname;                                                                                          " +
                               "                                                                                                                                  " +
                               "DECLARE @is_disabled bit;                                                                                                         " +
                               "                                                                                                                                  " +
                               "DECLARE @is_not_for_replication bit;                                                                                              " +
                               "                                                                                                                                  " +
                               "DECLARE @is_not_trusted bit;                                                                                                      " +
                               "                                                                                                                                  " +
                               "DECLARE @delete_referential_action tinyint;                                                                                       " +
                               "                                                                                                                                  " +
                               "DECLARE @update_referential_action tinyint;                                                                                       " +
                               "                                                                                                                                  " +
                               "DECLARE @tsql nvarchar(4000);                                                                                                     " +
                               "                                                                                                                                  " +
                               "DECLARE @tsql2 nvarchar(4000);                                                                                                    " +
                               "                                                                                                                                  " +
                               "DECLARE @fkCol sysname;                                                                                                           " +
                               "                                                                                                                                  " +
                               "DECLARE @pkCol sysname;                                                                                                           " +
                               "                                                                                                                                  " +
                               "DECLARE @col1 bit;                                                                                                                " +
                               "                                                                                                                                  " +
                               "DECLARE @action char(6);                                                                                                          " +
                               "                                                                                                                                  " +
                               "DECLARE @referenced_schema_name sysname;                                                                                          " +
                               "                                                                                                                                  " +
                               "                                                                                                                                  " +
                               "                                                                                                                                  " +
                               "DECLARE FKcursor CURSOR FOR                                                                                                       " +
                               "                                                                                                                                  " +
                               "     select OBJECT_SCHEMA_NAME(parent_object_id)                                                                                  " +
                               "                                                                                                                                  " +
                               "         , OBJECT_NAME(parent_object_id), name, OBJECT_NAME(referenced_object_id)                                                 " +
                               "                                                                                                                                  " +
                               "         , object_id                                                                                                              " +
                               "                                                                                                                                  " +
                               "         , is_disabled, is_not_for_replication, is_not_trusted                                                                    " +
                               "                                                                                                                                  " +
                               "         , delete_referential_action, update_referential_action, OBJECT_SCHEMA_NAME(referenced_object_id)                         " +
                               "                                                                                                                                  " +
                               "    from sys.foreign_keys                                                                                                         " +
                               "                                                                                                                                  " +
                               "    order by 1,2;                                                                                                                 " +
                               "                                                                                                                                  " +
                               "OPEN FKcursor;                                                                                                                    " +
                               "                                                                                                                                  " +
                               "FETCH NEXT FROM FKcursor INTO @schema_name, @table_name, @constraint_name                                                         " +
                               "                                                                                                                                  " +
                               "    , @referenced_object_name, @constraint_object_id                                                                              " +
                               "                                                                                                                                  " +
                               "    , @is_disabled, @is_not_for_replication, @is_not_trusted                                                                      " +
                               "                                                                                                                                  " +
                               "    , @delete_referential_action, @update_referential_action, @referenced_schema_name;                                            " +
                               "                                                                                                                                  " +
                               "WHILE @@FETCH_STATUS = 0                                                                                                          " +
                               "                                                                                                                                  " +
                               "BEGIN                                                                                                                             " +
                               "                                                                                                                                  " +
                               "                                                                                                                                  " +
                               "                                                                                                                                  " +
                               "      IF @action <> 'CREATE'                                                                                                      " +
                               "                                                                                                                                  " +
                               "        SET @tsql = 'ALTER TABLE '                                                                                                " +
                               "                                                                                                                                  " +
                               "                  + QUOTENAME(@schema_name) + '.' + QUOTENAME(@table_name)                                                        " +
                               "                                                                                                                                  " +
                               "                  + ' DROP CONSTRAINT ' + QUOTENAME(@constraint_name) + ';';                                                      " +
                               "                                                                                                                                  " +
                               "    ELSE                                                                                                                          " +
                               "                                                                                                                                  " +
                               "        BEGIN                                                                                                                     " +
                               "                                                                                                                                  " +
                               "        SET @tsql = 'ALTER TABLE '                                                                                                " +
                               "                                                                                                                                  " +
                               "                  + QUOTENAME(@schema_name) + '.' + QUOTENAME(@table_name)                                                        " +
                               "                                                                                                                                  " +
                               "                  + CASE @is_not_trusted                                                                                          " +
                               "                                                                                                                                  " +
                               "                        WHEN 0 THEN ' WITH CHECK '                                                                                " +
                               "                                                                                                                                  " +
                               "                        ELSE ' WITH NOCHECK '                                                                                     " +
                               "                                                                                                                                  " +
                               "                    END                                                                                                           " +
                               "                                                                                                                                  " +
                               "                  + ' ADD CONSTRAINT ' + QUOTENAME(@constraint_name)                                                              " +
                               "                                                                                                                                  " +
                               "                  + ' FOREIGN KEY (';                                                                                             " +
                               "                                                                                                                                  " +
                               "        SET @tsql2 = '';                                                                                                          " +
                               "                                                                                                                                  " +
                               "        DECLARE ColumnCursor CURSOR FOR                                                                                           " +
                               "                                                                                                                                  " +
                               "            select COL_NAME(fk.parent_object_id, fkc.parent_column_id)                                                            " +
                               "                                                                                                                                  " +
                               "                 , COL_NAME(fk.referenced_object_id, fkc.referenced_column_id)                                                    " +
                               "                                                                                                                                  " +
                               "            from sys.foreign_keys fk                                                                                              " +
                               "                                                                                                                                  " +
                               "            inner join sys.foreign_key_columns fkc                                                                                " +
                               "                                                                                                                                  " +
                               "            on fk.object_id = fkc.constraint_object_id                                                                            " +
                               "                                                                                                                                  " +
                               "            where fkc.constraint_object_id = @constraint_object_id                                                                " +
                               "                                                                                                                                  " +
                               "            order by fkc.constraint_column_id;                                                                                    " +
                               "                                                                                                                                  " +
                               "        OPEN ColumnCursor;                                                                                                        " +
                               "                                                                                                                                  " +
                               "        SET @col1 = 1;                                                                                                            " +
                               "                                                                                                                                  " +
                               "        FETCH NEXT FROM ColumnCursor INTO @fkCol, @pkCol;                                                                         " +
                               "                                                                                                                                  " +
                               "        WHILE @@FETCH_STATUS = 0                                                                                                  " +
                               "                                                                                                                                  " +
                               "        BEGIN                                                                                                                     " +
                               "                                                                                                                                  " +
                               "            IF (@col1 = 1)                                                                                                        " +
                               "                                                                                                                                  " +
                               "                SET @col1 = 0;                                                                                                    " +
                               "                                                                                                                                  " +
                               "            ELSE                                                                                                                  " +
                               "                                                                                                                                  " +
                               "            BEGIN                                                                                                                 " +
                               "                                                                                                                                  " +
                               "                SET @tsql = @tsql + ',';                                                                                          " +
                               "                                                                                                                                  " +
                               "                SET @tsql2 = @tsql2 + ',';                                                                                        " +
                               "                                                                                                                                  " +
                               "            END;                                                                                                                  " +
                               "                                                                                                                                  " +
                               "            SET @tsql = @tsql + QUOTENAME(@fkCol);                                                                                " +
                               "                                                                                                                                  " +
                               "            SET @tsql2 = @tsql2 + QUOTENAME(@pkCol);                                                                              " +
                               "                                                                                                                                  " +
                               "            FETCH NEXT FROM ColumnCursor INTO @fkCol, @pkCol;                                                                     " +
                               "                                                                                                                                  " +
                               "        END;                                                                                                                      " +
                               "                                                                                                                                  " +
                               "        CLOSE ColumnCursor;                                                                                                       " +
                               "                                                                                                                                  " +
                               "        DEALLOCATE ColumnCursor;                                                                                                  " +
                               "                                                                                                                                  " +
                               "       SET @tsql = @tsql + ' ) REFERENCES ' + QUOTENAME(@referenced_schema_name) + '.' + QUOTENAME(@referenced_object_name)       " +
                               "                                                                                                                                  " +
                               "                  + ' (' + @tsql2 + ')';                                                                                          " +
                               "                                                                                                                                  " +
                               "        SET @tsql = @tsql                                                                                                         " +
                               "                                                                                                                                  " +
                               "                  + ' ON UPDATE ' + CASE @update_referential_action                                                               " +
                               "                                                                                                                                  " +
                               "                                        WHEN 0 THEN 'NO ACTION '                                                                  " +
                               "                                                                                                                                  " +
                               "                                        WHEN 1 THEN 'CASCADE '                                                                    " +
                               "                                                                                                                                  " +
                               "                                        WHEN 2 THEN 'SET NULL '                                                                   " +
                               "                                                                                                                                  " +
                               "                                        ELSE 'SET DEFAULT '                                                                       " +
                               "                                                                                                                                  " +
                               "                                    END                                                                                           " +
                               "                                                                                                                                  " +
                               "                  + ' ON DELETE ' + CASE @delete_referential_action                                                               " +
                               "                                                                                                                                  " +
                               "                                        WHEN 0 THEN 'NO ACTION '                                                                  " +
                               "                                                                                                                                  " +
                               "                                        WHEN 1 THEN 'CASCADE '                                                                    " +
                               "                                                                                                                                  " +
                               "                                        WHEN 2 THEN 'SET NULL '                                                                   " +
                               "                                                                                                                                  " +
                               "                                        ELSE 'SET DEFAULT '                                                                       " +
                               "                                                                                                                                  " +
                               "                                    END                                                                                           " +
                               "                                                                                                                                  " +
                               "                  + CASE @is_not_for_replication                                                                                  " +
                               "                                                                                                                                  " +
                               "                        WHEN 1 THEN ' NOT FOR REPLICATION '                                                                       " +
                               "                                                                                                                                  " +
                               "                        ELSE ''                                                                                                   " +
                               "                                                                                                                                  " +
                               "                    END                                                                                                           " +
                               "                                                                                                                                  " +
                               "                  + ';';                                                                                                          " +
                               "                                                                                                                                  " +
                               "        END;                                                                                                                      " +
                               "                                                                                                                                  " +
                               "    insert into @sql(name, script) values (@constraint_name, @tsql)                                                               " +
                               "                                                                                                                                  " +
                               "    IF @action = 'CREATE'                                                                                                         " +
                               "                                                                                                                                  " +
                               "        BEGIN                                                                                                                     " +
                               "                                                                                                                                  " +
                               "        SET @tsql = 'ALTER TABLE '                                                                                                " +
                               "                                                                                                                                  " +
                               "                  + QUOTENAME(@schema_name) + '.' + QUOTENAME(@table_name)                                                        " +
                               "                                                                                                                                  " +
                               "                  + CASE @is_disabled                                                                                             " +
                               "                                                                                                                                  " +
                               "                        WHEN 0 THEN ' CHECK '                                                                                     " +
                               "                                                                                                                                  " +
                               "                        ELSE ' NOCHECK '                                                                                          " +
                               "                                                                                                                                  " +
                               "                    END                                                                                                           " +
                               "                                                                                                                                  " +
                               "                  + 'CONSTRAINT ' + QUOTENAME(@constraint_name)                                                                   " +
                               "                                                                                                                                  " +
                               "                  + ';';                                                                                                          " +
                               "                                                                                                                                  " +
                               "        insert into @sql(name, script) values (@constraint_name, @tsql)                                                           " +
                               "                                                                                                                                  " +
                               "        END;                                                                                                                      " +
                               "                                                                                                                                  " +
                               "    FETCH NEXT FROM FKcursor INTO @schema_name, @table_name, @constraint_name                                                     " +
                               "                                                                                                                                  " +
                               "        , @referenced_object_name, @constraint_object_id                                                                          " +
                               "                                                                                                                                  " +
                               "        , @is_disabled, @is_not_for_replication, @is_not_trusted                                                                  " +
                               "                                                                                                                                  " +
                               "        , @delete_referential_action, @update_referential_action, @referenced_schema_name;                                        " +
                               "                                                                                                                                  " +
                               "END;                                                                                                                              " +
                               "                                                                                                                                  " +
                               "CLOSE FKcursor;                                                                                                                   " +
                               "                                                                                                                                  " +
                               "DEALLOCATE FKcursor;                                                                                                              " +
                               "                                                                                                                                  " +
                               "SELECT * FROM @sql                                                                                                                "
            #endregion
;

            da.CommandType = System.Data.CommandType.Text;
            da.CommandText = script;

            dt = da.GetDataTable();

            foreach (DataRow row in dt.Rows)
            {
                results.Rows.Add("Foreign Keys", row["Name"], row["script"]);
            }

            #endregion

            i = 0;
            Console.WriteLine("removing old data......");

            if (Directory.Exists(codeRootPath))
            {
                Directory.Delete(codeRootPath, true);
            }

            foreach (DataRow row in results.Rows)
            {
                i++;
                Console.Write("Writing {0:d4}: {1}.{2}...", i, row["Type"], row["Name"]);
                string path = Path.Combine(codeRootPath, row["Type"].ToString());

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                path = Path.Combine(path, row["Name"].ToString() + ".sql");

                File.WriteAllText(path, row["Script"].ToString());

                Console.WriteLine("done");
            }

            Console.Read();
        }

        public const string _RootPath = @"C:\Development\_Code Check\Extract Scripts";
        public const string _RootPath_I = @"C:\Development\_Code Check\Extract Script _RDI";
        public const string _ConnectionString = "server=10.22.12.43;uid=SCIToolTeam;pwd=SciTool!;database=ResponseDriver;";
        public const string _ConnectionString_I = "server=10.22.12.43;uid=SCIToolTeam;pwd=SciTool!;database=ResponseDriver_I;";

        public static void RemoveFileNameTail(string codeRootPath)
        {
            string path;

            path = Path.Combine(codeRootPath, "Tables");
            RemoveFileNameTail(path, ".table");

            path = Path.Combine(codeRootPath, "Stored Procedures");
            RemoveFileNameTail(path, ".StoredProcedure");

            path = Path.Combine(codeRootPath, "Functions");
            RemoveFileNameTail(path, ".UserDefinedFunction");

            path = Path.Combine(codeRootPath, "Views");
            RemoveFileNameTail(path, ".View");

            path = Path.Combine(codeRootPath, "Triggers");
            RemoveFileNameTail(path, ".Trigger");

            path = Path.Combine(codeRootPath, "Assemblies");
            RemoveFileNameTail(path, ".SqlAssembly");
        }

        public static void RemoveFileNameTail(string codeRootPath, string tail)
        {
            if (Directory.Exists(codeRootPath))
            {
                string[] fileNames = Directory.GetFiles(codeRootPath);

                foreach (string file in fileNames)
                {
                    string temp = file;
                    string ext = Path.GetExtension(temp);

                    string path = Path.GetDirectoryName(file);
                    string fileName = Path.GetFileName(file);

                    if (fileName.ToLower().StartsWith("dbo."))
                    {
                        fileName = fileName.Substring(4);
                        temp = Path.Combine(path, fileName);
                    }

                    if (temp.ToLower().EndsWith((tail + ext).ToLower()))
                    {
                        temp = temp.Remove(temp.Length - tail.Length - ext.Length) + ext;                        
                    }

                    File.Move(file, temp);
                }
            }
        }

        public static void FormatScript(string codeRootPath, string databaseName)
        {
            string[] fileNames = Directory.GetFiles(codeRootPath, "*.*", SearchOption.AllDirectories);
            foreach (string file in fileNames)
            {
                Console.Write("processing {0}...", file);
                //string file = @"C:\Development\DealerDevelopment\ResponseDriver\Trunk\ResponseDriver.Database\Extract Scripts\Stored Procedures\dbo.Admin_Get_GlobalAttrValue.sql";
                string content = File.ReadAllText(file);
                string scriptType = "";
                string scriptName = Path.GetFileNameWithoutExtension(file);
                //scriptName = scriptName.Substring(4);

                if (content.ToLower().Contains("create procedure "))
                {
                    content = content.Substring(content.ToLower().IndexOf("create procedure "));
                    scriptType = "proc";
                }
                else if (content.ToLower().Contains("create proc "))
                {
                    content = content.Substring(content.ToLower().IndexOf("create proc "));
                    scriptType = "proc";
                }
                else if (content.ToLower().Contains("create  procedure "))
                {
                    content = content.Substring(content.ToLower().IndexOf("create  procedure "));
                    scriptType = "proc";
                }
                else if (content.ToLower().Contains("create function "))
                {
                    content = content.Substring(content.ToLower().IndexOf("create function "));
                    scriptType = "func";
                }
                else
                {
                    content = Regex.Replace(content, @"/[*][\s\S]+?[*]/", "");
                    content = content.Replace(string.Format("USE [{0}]\r\nGO\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\n", databaseName), "");
                    content = content.Replace(string.Format("USE [{0}]\r\nGO\r\n", databaseName), "");
                    content = content.Replace(string.Format("USE [{0}]\r\nGO\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\n", "ResponseDriver"), "");
                    content = content.Replace(string.Format("USE [{0}]\r\nGO\r\n", "ResponseDriver"), "");
                    content = content.Replace("\r\n\r\n", "\r\n");
                    content = content.Replace("TEXTIMAGE_ON [FileGroup1]", "");
                    content = content.Replace("ON [FileGroup1]", "");
                    content = content.Replace("ON [IndexGroup01]", "");
                }

                //content = Regex.Replace(content, @"/[*][\s\S]+?[*]/", "");
                //content = content.Replace(string.Format("USE [{0}]\r\nGO\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\n", databaseName), "");
                //content = content.Replace(string.Format("USE [{0}]\r\nGO\r\n", databaseName), "");
                //content = content.Replace(string.Format("USE [{0}]\r\nGO\r\nSET ANSI_NULLS ON\r\nGO\r\nSET QUOTED_IDENTIFIER ON\r\nGO\r\n", "ResponseDriver"), "");
                //content = content.Replace(string.Format("USE [{0}]\r\nGO\r\n", "ResponseDriver"), "");
                //content = content.Replace("\r\n\r\n", "\r\n");
                //content = content.Replace("TEXTIMAGE_ON [FileGroup1]", "");
                //content = content.Replace("ON [FileGroup1]", "");
                //content = content.Replace("ON [IndexGroup01]", "");

                //if (scriptName == "allsps" || scriptName == "allfunctions")
                //{
                string usescript = "USE [ResponseDriver]\r\n" +
                "GO" + "\r\n\r\n";
                //}
                
                string comment =
                "/*******************************************************************************" + "\r\n" +
                "**		Name: {0}                                                                " + "\r\n" +
                "**		Desc: {0}                                                                " + "\r\n" +
                "**                                                                              " + "\r\n" +
                "**                                                                              " + "\r\n" +
                "**		Author: Extract from SQL Server                                          " + "\r\n" +
                "**		Date: {1}                                                                " + "\r\n" +
                "********************************************************************************" + "\r\n" +
                "**		Change History                                                           " + "\r\n" +
                "********************************************************************************" + "\r\n" +
                "**		Date:		Author:			Description:                             " + "\r\n" +
                "**		--------	--------		-----------------------------------------" + "\r\n" +
                "**		{1}	Unknown			Script from SQL server" + "\r\n" +
                "*******************************************************************************/" + "\r\n\r\n";

                comment = string.Format(comment, Path.GetFileNameWithoutExtension(file), DateTime.Now.ToString("yyyy-MM-dd"));

                string dropScript = "";

                if (scriptType == "proc")
                {
                    dropScript = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'P', N'PC'))\r\n";
                    dropScript += "DROP PROCEDURE [dbo].[{0}]\r\n";
                    dropScript += "GO\r\n";
                }
                else if (scriptType == "func")
                {
                    dropScript = "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{0}]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))\r\n";
                    dropScript += "DROP FUNCTION [dbo].[{0}]\r\n";
                    dropScript += "GO\r\n";
                }

                if (!string.IsNullOrEmpty(dropScript))
                {
                    dropScript = string.Format(dropScript, scriptName);
                }

                content = usescript + dropScript + comment + content;

                File.WriteAllText(file, content);

                Console.WriteLine("done");
                //foreach (string file in fileNames)
                //{
                //    string content = File.ReadAllText(file);
                //    content = Regex.Replace(content, "///*.*/*//", "");
                //}
            }
        }

        public static void CombineAllTalbeScripts(string codeRootPath, string connectionString)
        {
            string scriptPath = Path.Combine(codeRootPath, "tables");

            string script =
  "  DECLARE @totalTableCount int                                                                                                            " +
  "  DECLARE @tableListCount int                                                                                                             " +
  "                                                                                                                                          " +
  "  DECLARE @tableList TABLE                                                                                                                " +
  "  (tableID int)                                                                                                                           " +
  "                                                                                                                                          " +
  "  SELECT @totalTableCount = COUNT(1) FROM sys.tables                                                                                      " +
  "                                                                                                                                          " +
  "  INSERT INTO @tableList                                                                                                                  " +
  "  SELECT object_id FROM sys.tables                                                                                                        " +
  "  	WHERE object_id NOT IN                                                                                                               " +
  "  		(SELECT parent_object_id FROM sys.foreign_keys)                                                                                  " +
  "  	ORDER BY [name]	                                                                                                                     " +
  "  	                                                                                                                                     " +
  "  WHILE (EXISTS(                                                                                                                          " +
  "  select * from sys.foreign_keys                                                                                                          " +
  "  	WHERE parent_object_id NOT in                                                                                                        " +
  "  		(SELECT parent_object_id FROM sys.foreign_keys keys LEFT JOIN @tableList tbl ON keys.referenced_object_id = tbl.tableID          " +
  "  				WHERE tbl.tableID is null)                                                                                               " +
  "  	AND referenced_object_id IN (SELECT tableID FROM @tableList)                                                                         " +
  "  	AND parent_object_id NOT IN (SELECT tableID FROM @tableList)                                                                         " +
  "  ))                                                                                                                                      " +
  "  BEGIN                                                                                                                                   " +
  "  	INSERT INTO @tableList                                                                                                               " +
  "  	SELECT distinct parent_object_id FROM sys.foreign_keys                                                                               " +
  "  	WHERE parent_object_id NOT in                                                                                                        " +
  "  		(SELECT parent_object_id FROM sys.foreign_keys keys LEFT JOIN @tableList tbl ON keys.referenced_object_id = tbl.tableID          " +
  "  				WHERE tbl.tableID is null)                                                                                               " +
  "  	AND referenced_object_id IN (SELECT tableID FROM @tableList)                                                                         " +
  "  	AND parent_object_id NOT IN (SELECT tableID FROM @tableList)                                                                         " +
  " END                                                                                                                                      " +
  "                                                                                                                                          " +
  "  INSERT INTO @tableList                                                                                                                  " +
  "  SELECT parent_object_id FROM sys.foreign_keys                                                                                           " +
  "  	WHERE parent_object_id NOT IN                                                                                                        " +
  "  		(SELECT tableID FROM @tableList)                                                                                                 " +
  "  	ORDER BY [name]                                                                                                                      " +
  "                                                                                                                                          " +
  "  		                                                                                                                                 " +
  "  SELECT tableID, name FROM @tableList l INNER JOIN sys.tables t ON l.tableID=t.object_id and t.name not like 'DEL_%'                     "; 

            DataAccess da = new DataAccess(connectionString, ProviderType.Sql);
            da.CommandText = script;
            da.CommandType = CommandType.Text;
            DataTable dt = da.GetDataTable();

            StringBuilder sb = new StringBuilder();

            foreach (DataRow row in dt.Rows)
            {
                string path = string.Format("{0}.sql", row["name"]);
                path = Path.Combine(scriptPath, path);

                if (File.Exists(path))
                {
                    sb.AppendFormat("PRINT '**************** {0} *******************'", row["name"]).AppendLine();
                    sb.AppendLine(File.ReadAllText(path));
                }
            }

            string dest = Path.Combine(codeRootPath, "alltables.sql");
            File.WriteAllText(dest, sb.ToString());
        }

        public static void CombineAllStoreProcedureScripts(string codeRootPath)
        {
            string scriptPath = Path.Combine(codeRootPath, "Stored Procedures");

            string[] fileNames = Directory.GetFiles(scriptPath);

            StringBuilder sb = new StringBuilder();
            foreach (string file in fileNames)
            {
                string scriptName = Path.GetFileNameWithoutExtension(file);
                //scriptName = scriptName.Substring(4);
                sb.AppendFormat("PRINT '**************** {0} *******************'", scriptName).AppendLine();
                sb.AppendLine(File.ReadAllText(file));
            }

            string dest = Path.Combine(codeRootPath, "allsps.sql");
            File.WriteAllText(dest, sb.ToString());
        }

        public static void CombineAllFunctions(string codeRootPath)
        {
            string scriptPath = Path.Combine(codeRootPath, "Functions");

            string[] fileNames = Directory.GetFiles(scriptPath);

            StringBuilder sb = new StringBuilder();
            foreach (string file in fileNames)
            {
                string scriptName = Path.GetFileNameWithoutExtension(file);
                //scriptName = scriptName.Substring(4);
                sb.AppendFormat("PRINT '**************** {0} *******************'", scriptName).AppendLine();
                sb.AppendLine(File.ReadAllText(file));
            }

            string dest = Path.Combine(codeRootPath, "allfunctions.sql");
            File.WriteAllText(dest, sb.ToString());
        }

        public static void CombineAllViews(string codeRootPath)
        {
            string scriptPath = Path.Combine(codeRootPath, "Views");

            string[] fileNames = Directory.GetFiles(scriptPath);

            StringBuilder sb = new StringBuilder();
            foreach (string file in fileNames)
            {
                string scriptName = Path.GetFileNameWithoutExtension(file);
                //scriptName = scriptName.Substring(4);
                sb.AppendFormat("PRINT '**************** {0} *******************'", scriptName).AppendLine();
                sb.AppendLine(File.ReadAllText(file));
            }

            string dest = Path.Combine(codeRootPath, "allviews.sql");
            File.WriteAllText(dest, sb.ToString());
        }

        public static void CombineAllInit(string codeRootPath)
        {
            string scriptPath = Path.Combine(codeRootPath, "Initialization");

            string[] fileNames = Directory.GetFiles(scriptPath);

            StringBuilder sb = new StringBuilder();
            foreach (string file in fileNames)
            {
                string scriptName = Path.GetFileNameWithoutExtension(file);
                //scriptName = scriptName.Substring(4);
                sb.AppendFormat("PRINT '**************** {0} *******************'", scriptName).AppendLine();
                sb.AppendLine(File.ReadAllText(file));
            }

            string dest = Path.Combine(codeRootPath, "allinit.sql");
            File.WriteAllText(dest, sb.ToString());
        }
    }
}
