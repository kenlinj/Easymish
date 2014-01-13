using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Data;
using NeatCode.Library.Data;

namespace ConsoleApplication1.SCI
{
    class CheckCodeUse
    {

        public const string _SourceCodeRootFolder = @"C:\Development\DealerDevelopment\ResponseDriver\Trunk";
        public const string _ExtractScriptRootFolder = @"C:\Development\_Code Check\Extract Scripts";
        
        public static void CheckSp(string scriptType)
        {
            //string resultFile = @"C:\Development\_Code Check\result.txt";
            string folderName = scriptType;// "Functions";
            List<string> toCheckFiles = new List<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("FileName");
            dt.Columns.Add("UsedFile");
            dt.Columns.Add("UsedFileLine", typeof(int));
            dt.Columns.Add("Content");

            StringBuilder sb = new StringBuilder();
            string[] tempArray = Directory.GetFiles(_SourceCodeRootFolder, "*.*", SearchOption.AllDirectories);
            string[] optOutFolders = new string[] {"ResponseDriver.Database", "Reference", "SCI.BizTalk.SharedSchema", "SCI_GPL_BizTalk", "LeadsManagement.SCI" };

            for (int i = 0; i < optOutFolders.Length; i++)
            {
                optOutFolders[i] = Path.Combine(_SourceCodeRootFolder, optOutFolders[i]);
            }

            foreach (string arr in tempArray)
            {
                if (!optOutFolders.Any(f => arr.ToLower().StartsWith(f.ToLower())))
                {
                    toCheckFiles.Add(arr);
                }
            }

            tempArray = Directory.GetFiles(_ExtractScriptRootFolder, "*.*", SearchOption.AllDirectories);
            //tempArray = new string[]{@"C:\Development\DealerDevelopment\ResponseDriver\Trunk\ResponseDriver.Framework\ResponseDriver.Framework.DAL\AuditLog.cs"};

            toCheckFiles.AddRange(tempArray);

            string scriptFoler = Path.Combine(_ExtractScriptRootFolder, folderName);

            string[] scriptFiles = Directory.GetFiles(scriptFoler, "*.*", SearchOption.AllDirectories);

            foreach (string toCheckFile in toCheckFiles)
            {
                string toCheckFileName = Path.GetFileNameWithoutExtension(toCheckFile);
                string toCheckFileType = Path.GetExtension(toCheckFile);
               
                Console.Write("checking " + toCheckFile + "...");
                string[] optoutExts = new string[] { ".dll", ".exe", ".dic", ".jpg", ".sln", ".suo", ".vssscc", ".vdproj", ".csproj", ".pdb", ".js", ".css" };
                if (!optoutExts.Contains(toCheckFileType) && !toCheckFileName.ToLower().StartsWith("del_"))
                {
                    byte[] fileBytes = File.ReadAllBytes(toCheckFile);

                    if (fileBytes.Length < 300000)
                    {
                        string[] toCheckFileLines = File.ReadAllLines(toCheckFile);
                        int count = 0;

                        foreach (string scriptFile in scriptFiles)
                        {
                            string scriptFileName = Path.GetFileNameWithoutExtension(scriptFile);
                            for (int i = 0; i < toCheckFileLines.Length; i++)
                            {
                                string toCheckFileLine = toCheckFileLines[i];

                                if (scriptFileName.ToLower() != toCheckFileName.ToLower() && !scriptFileName.ToLower().StartsWith("del_"))
                                {
                                    if (Regex.IsMatch(toCheckFileLine, scriptFileName, RegexOptions.IgnoreCase))
                                    {
                                        dt.Rows.Add(scriptFileName, toCheckFile, i + 1, toCheckFileLine);
                                        count++;
                                    }
                                }
                            }
                        }

                        Console.WriteLine("done. " + count.ToString() + " matches found!");
                    }
                    else
                    {
                        Console.WriteLine("ignore");
                    }
                }
                else
                {
                    Console.WriteLine("ignore");
                }
            }

            DataAccess da = new DataAccess(@"Initial Catalog=MyDb;Data Source=.;Integrated Security=SSPI;", ProviderType.Sql);
            da.CommandType = CommandType.Text;

            foreach (string scriptFile in scriptFiles)
            {
                string ex = Path.GetFileNameWithoutExtension(scriptFile);

                DataRow[] rows = dt.Select("FileName='" + ex + "'");

                string fileName = ex;
                string usedFile = null;
                int usedFileLine = 0;
                string content = null;

                if (rows != null && rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        try
                        {
                            fileName = row["FileName"].ToString();
                            usedFile = row["UsedFile"].ToString();
                            usedFileLine = (int)row["UsedFileLine"];
                            content = row["Content"].ToString().Replace("'", "''");
                            if (content.Length > 1000)
                            {
                                content = content.Substring(0, 900) + "...";
                            }

                            da.CommandText = string.Format("INSERT INTO ScriptUseStatus (FileName, UsedFile, UsedFileLine, Content, ScriptType) VALUES ('{0}', '{1}', {2}, '{3}', '{4}')", fileName, usedFile, usedFileLine, content, scriptType);

                            da.Execute();
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    da.CommandText = string.Format("INSERT INTO ScriptUseStatus (FileName, UsedFile, UsedFileLine, Content, ScriptType) VALUES ('{0}', '{1}', {2}, '{3}', '{4}')", fileName, usedFile, usedFileLine, content, scriptType);

                    da.Execute();
                }
            }

            Console.WriteLine("all done");
            //Console.Read();
        }

        public static void CheckTablesAndViews(string scriptType)
        {
            //string resultFile = @"C:\Development\_Code Check\result.txt";
            string folderName = scriptType;// "Functions";
            List<string> toCheckFiles = new List<string>();
            DataTable dt = new DataTable();
            dt.Columns.Add("FileName");
            dt.Columns.Add("UsedFile");
            dt.Columns.Add("UsedFileLine", typeof(int));
            dt.Columns.Add("Content");

            StringBuilder sb = new StringBuilder();
            string[] tempArray = Directory.GetFiles(_SourceCodeRootFolder, "*.*", SearchOption.AllDirectories);
            string[] optOutFolders = new string[] { "ResponseDriver.Database", "Reference", "SCI.BizTalk.SharedSchema", "SCI_GPL_BizTalk", "LeadsManagement.SCI" };

            for (int i = 0; i < optOutFolders.Length; i++)
            {
                optOutFolders[i] = Path.Combine(_SourceCodeRootFolder, optOutFolders[i]);
            }

            foreach (string arr in tempArray)
            {
                if (!optOutFolders.Any(f => arr.ToLower().StartsWith(f.ToLower())))
                {
                    toCheckFiles.Add(arr);
                }
            }

            tempArray = Directory.GetFiles(_ExtractScriptRootFolder, "*.*", SearchOption.AllDirectories);
            //tempArray = new string[]{@"C:\Development\DealerDevelopment\ResponseDriver\Trunk\ResponseDriver.Framework\ResponseDriver.Framework.DAL\AuditLog.cs"};

            toCheckFiles.AddRange(tempArray);

            string scriptFoler = Path.Combine(_ExtractScriptRootFolder, folderName);

            string[] scriptFiles = Directory.GetFiles(scriptFoler, "*.*", SearchOption.AllDirectories);

            foreach (string toCheckFile in toCheckFiles)
            {
                string toCheckFileName = Path.GetFileNameWithoutExtension(toCheckFile);
                string toCheckFileType = Path.GetExtension(toCheckFile);

                Console.Write("checking " + toCheckFile + "...");
                string[] optoutExts = new string[] { ".dll", ".exe", ".dic", ".jpg", ".sln", ".suo", ".vssscc", ".vdproj", ".csproj", ".pdb", ".js", ".css" };
                if (!optoutExts.Contains(toCheckFileType) && !toCheckFileName.ToLower().StartsWith("del_"))
                {
                    byte[] fileBytes = File.ReadAllBytes(toCheckFile);

                    if (fileBytes.Length < 300000)
                    {
                        string[] toCheckFileLines = File.ReadAllLines(toCheckFile);
                        int count = 0;

                        foreach (string scriptFile in scriptFiles)
                        {
                            string scriptFileName = Path.GetFileNameWithoutExtension(scriptFile);
                            for (int i = 0; i < toCheckFileLines.Length; i++)
                            {
                                string toCheckFileLine = toCheckFileLines[i];

                                if (scriptFileName.ToLower() != toCheckFileName.ToLower() && !scriptFileName.ToLower().StartsWith("del_"))
                                {
                                    string ex = scriptFileName;
                                    if (Regex.IsMatch(toCheckFileLine, ex, RegexOptions.IgnoreCase))
                                    {
                                        dt.Rows.Add(scriptFileName, toCheckFile, i + 1, toCheckFileLine);
                                        count++;
                                    }
                                }
                            }
                        }

                        Console.WriteLine("done. " + count.ToString() + " matches found!");
                    }
                    else
                    {
                        Console.WriteLine("ignore");
                    }
                }
                else
                {
                    Console.WriteLine("ignore");
                }
            }

            DataAccess da = new DataAccess(@"Initial Catalog=MyDb;Data Source=.;Integrated Security=SSPI;", ProviderType.Sql);
            da.CommandType = CommandType.Text;

            foreach (string scriptFile in scriptFiles)
            {
                string ex = Path.GetFileNameWithoutExtension(scriptFile);

                DataRow[] rows = dt.Select("FileName='" + ex + "'");

                string fileName = ex;
                string usedFile = null;
                int usedFileLine = 0;
                string content = null;

                if (rows != null && rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        try
                        {
                            fileName = row["FileName"].ToString();
                            usedFile = row["UsedFile"].ToString();
                            usedFileLine = (int)row["UsedFileLine"];
                            content = row["Content"].ToString().Replace("'", "''");
                            if (content.Length > 1000)
                            {
                                content = content.Substring(0, 900) + "...";
                            }

                            da.CommandText = string.Format("INSERT INTO ScriptUseStatus (FileName, UsedFile, UsedFileLine, Content, ScriptType) VALUES ('{0}', '{1}', {2}, '{3}', '{4}')", fileName, usedFile, usedFileLine, content, scriptType);

                            da.Execute();
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    da.CommandText = string.Format("INSERT INTO ScriptUseStatus (FileName, UsedFile, UsedFileLine, Content, ScriptType) VALUES ('{0}', '{1}', {2}, '{3}', '{4}')", fileName, usedFile, usedFileLine, content, scriptType);

                    da.Execute();
                }
            }

            Console.WriteLine("all done");
            //Console.Read();
        }
    }
}
