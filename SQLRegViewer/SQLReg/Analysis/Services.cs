using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace SQLReg.Analysis
{
    public static class Services
    {
        /*
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MSSQLFDLauncher",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MSSQLSERVER",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\MSSQLServerOLAPService",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SQLBrowser",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SQLSERVERAGENT",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SQLWriter",
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SQLTELEMETRY"

              if (key.StartsWith("MSSQLServerADHelper")
                    || key.StartsWith("RsFx")
                    || key.StartsWith("DTSPipeline")
                    || key.StartsWith("MsDtsServer")

            */
        public static        string[] KnownServices = {
            "MSSQLSERVER","MSSQL$","MSSQLSERVERADHelper","SQLSERVERAGENT",
            "SQLAgent$","MSSQLServerOLAPService","MSOLAP$",
            "SQLBrowser","MSSQLFDLauncher","MsDtsServer","ReportServer",
             "SQLWriter","RsFx","DTSPipeline",
            "MSSQLLaunchpad","SSASTELEMETRY","SQLTELEMETRY",
                "SQL Server Distributed Replay Client",
                "SQL Server Distributed Replay Controller",
                "SSISTELEMETRY",
                "SSISScaleOutMaster",
                "SSISScaleOutWorker"



            };

        public static List<string> serviceCleanupScript = new List<string>();
        public static List<string> serviceRestoreScript = new List<string>();
        public static void Add()
        {
            /****************************************************************************************************/
            //HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services  
            /****************************************************************************************************/

            Controller.UpdateProgress("Add Services key...", true);
            serviceCleanupScript.Clear();
            serviceRestoreScript.Clear();
            serviceCleanupScript.Add("\n\r");
            serviceRestoreScript.Add("\n\r");
            serviceCleanupScript.Add("REM Known SQL Server related Services.");
            serviceRestoreScript.Add("REM Known SQL Server related Services.");

            List<string> knownKeys = new List<string>
            {
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\DatabaseMail",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\MSDMine",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\MSSQLSERVER",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\MSSQLServerOLAPService",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLBrowser",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLCTR",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLDumper",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLSERVERAGENT",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLVDI",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLWEP",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLWriter",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLBackupToUrl",
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\VSS\Diag\SqlServerWriter",

            };
            int cnt = 0;
            foreach (string key in knownKeys)
            {
                if (RegHelper.IsKeyExist(key))
                {
                    if (!Controller.sqlRegKeys.ContainsKey(key))
                    {
                        cnt++;
                        Controller.sqlRegKeys.Add(key,
                        new Reason(SourceType.Service, key, "Known SQL related service",true));



                    }
                }
            }


            //
            //"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Security\MSSQLSERVER$AUDIT",
            //"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Security\MSSQL$SQL2014$AUDIT",

            RegKey rk = new RegKey(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Security", "");
            foreach (string key in rk.subKeys)
            {
                string sub = rk.Path + "\\" + key;
                if (key.StartsWith("MSSQL") && key.EndsWith("$AUDIT"))
                    if (!Controller.sqlRegKeys.ContainsKey(sub))
                    {


                        cnt++;
                        Controller.sqlRegKeys.Add(sub,
                        new Reason(SourceType.Service, sub, "Known SQL related service",true));

                    }
            }

            //"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLISPackage100",
            //"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application\SQLISService100",
            rk = new RegKey(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\Application", "");
            foreach (string key in rk.subKeys)
            {
                if (key.StartsWith("SQLISPackage")
                    || key.EndsWith("SQLISService")
                    || key.EndsWith("SQLNCLI")
                    || key.EndsWith("MSSQLServerADHelper")
                    )

                    if (!Controller.sqlRegKeys.ContainsKey(rk.Path + "\\" + key))
                    {
                        {


                            cnt++;
                            Controller.sqlRegKeys.Add(rk.Path + "\\" + key,
                            new Reason(SourceType.Service, rk.Path + "\\" + key, "Known SQL related service",true));
                        }
                    }
            }
            rk = new RegKey(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\EventLog\System", "");
            foreach (string key in rk.subKeys)
            {
                if (key.StartsWith("RsFx"))
                    if (!Controller.sqlRegKeys.ContainsKey(rk.Path + "\\" + key))
                    {


                        cnt++;
                        Controller.sqlRegKeys.Add(rk.Path + "\\" + key,
                        new Reason(SourceType.Service, rk.Path + "\\" + key, "Known SQL related serivce",true));
                    }
            }

          

 
            rk = new RegKey(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services", "");
             List<string> protectKeys = new List<string>() { /* "sqlwriter" */ };
            foreach (string key in rk.subKeys)
            {

                foreach (string service in KnownServices)
                {
                    if (key.ToLower().StartsWith(service.ToLower()))
                    {
                        string path = rk.Path + "\\" + key;

                        if (!Controller.sqlRegKeys.ContainsKey(path))
                        {
                            cnt++;
                            Controller.sqlRegKeys.Add(path,
                            new Reason(SourceType.Service, path, "Known SQL related serivce",true));
                            if (!protectKeys.Contains(key.ToLower()))
                            {
                                serviceCleanupScript.Add("");
                                serviceRestoreScript.Add("");

                                string filename = Utility.CleanFileName(path) + ".hiv";
                                string save="REG SAVE " + "\"" + path + "\" \"" + filename + "\" /y /reg:64";
                                string del="REG DELETE " + "\"" + path + "\" /f /reg:64";



                                string add="REG ADD " + "\"" + path + "\"  /f ";
                                string restore="REG RESTORE " + "\"" + path + "\" \"" + filename + "\" /f ";



                                serviceCleanupScript.Add("NET STOP \"" + key + "\"");
                                serviceCleanupScript.Add(save);
                                serviceCleanupScript.Add("SC DELETE \"" + key + "\"");
                                serviceCleanupScript.Add(del);

                                serviceRestoreScript.Add(add);
                                serviceRestoreScript.Add(restore);


                            }
                        }
                    }
                }

            }

            Controller.UpdateProgress("Services Add keys:" + cnt, true);



        }
    }
}
