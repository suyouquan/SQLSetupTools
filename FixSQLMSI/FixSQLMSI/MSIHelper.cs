using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using System.Runtime.InteropServices;

namespace FixSQLMSI
{
   public static class MSIHelper
    {

        public const uint ERROR_SUCCESS = 0;
        public const int ERROR_MORE_DATA = 0xEA;

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern uint MsiGetProperty(uint hInstall, string szName,out StringBuilder szValueBuf, ref uint pchValueBuf);
        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiOpenPackageEx(string szPackagePath, uint dwOptions, out uint hProduct);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiOpenPackage(string szPackagePath, out uint hProduct);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiCloseHandle(uint hAny);

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        public static extern uint MsiOpenDatabase(string szDatabasePath, IntPtr uiOpenMode, out uint hDatabase);


       


        public static IEnumerable<ProductInstallation> GetInstalledProducts()
        {
            //List<string> products = new List<string>();

            return Microsoft.Deployment.WindowsInstaller.ProductInstallation.GetProducts(null, null, UserContexts.All);
           


        }

        
        public static string Get(string msi, string name)
        {
            using (var db = new QDatabase(msi.ToLower(), DatabaseOpenMode.ReadOnly))
            {
                return db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", name) as string;
            }
        }

        public static string MspGetMetadata(string msp, string name)
        {
            using (var db = new QDatabase(msp.ToLower(), DatabaseOpenMode.ReadOnly))
            {
                return db.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", name) as string;
            }
        }

        public static string GetRevisionNumber(string msimsp)
        {
            try
            {
                using (var db = new QDatabase(msimsp.ToLower(), DatabaseOpenMode.ReadOnly))
                {
                    return db.SummaryInfo.RevisionNumber;
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            return "";
        }


        public static Dictionary<String, String> GetProperties(string msi)
        {
            var t = new Dictionary<string, string>();

            using (var database = new QDatabase(msi.ToLower(), DatabaseOpenMode.ReadOnly))
            {
                var properties = from p in database.Properties
                                 select p;

                foreach (var property in properties)
                {
                    t.Add(property.Property, property.Value);
                }
            }

            return t;
        }

        

        public static Dictionary<String, String> GetComponents(string msi)
        {
            var t = new Dictionary<string, string>();

            using (var database = new QDatabase(msi.ToLower(), DatabaseOpenMode.ReadOnly))
            {
                var comps = from p in database.Components
                            select p;

                foreach (var comp in comps)
                {
                    t.Add(comp.Component, comp.ComponentId);
                }
            }

            return t;
        }




    }
}
