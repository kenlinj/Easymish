using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NeatCode.Library.Data;
using System.Data;
using Easymish.Business;

namespace ConsoleApplication1
{
    class Methods
    {
        public static void SeperateFile()
        {
            string file = @"C:\Users\Ken\Desktop\zetian.txt";
            string folder = @"C:\Users\Ken\Desktop\1";

            FileHelper.SeperateFile(file, folder);
        }

        public static void CodeAnalysis(string codeRootPath)
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

            i=0;
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
    }
}
