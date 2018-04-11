using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using System.Runtime.InteropServices;

namespace SQLReg
{
    public static class MSIHelper
    {

        public const uint ERROR_SUCCESS = 0;
        public const int ERROR_MORE_DATA = 0xEA;

        [DllImport("msi.dll", CharSet = CharSet.Unicode)]
        static extern uint MsiGetProperty(uint hInstall, string szName, out StringBuilder szValueBuf, ref uint pchValueBuf);
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

        public static string[] SqlRuns = new string[]
       {
            ":SqlRun01.mst",":SqlRun02.mst",":SqlRun03.mst",":SqlRun04.mst",":SqlRun05.mst",":SqlRun06.mst",":SqlRun07.mst",":SqlRun08.mst",":SqlRun09.mst",":SqlRun10.mst",
            ":SqlRun11.mst",":SqlRun12.mst",":SqlRun13.mst",":SqlRun14.mst",":SqlRun15.mst",":SqlRun16.mst",":SqlRun17.mst",":SqlRun18.mst",":SqlRun19.mst",":SqlRun20.mst",
            ":SqlRun21.mst",":SqlRun22.mst",":SqlRun23.mst",":SqlRun24.mst",":SqlRun25.mst",":SqlRun26.mst",":SqlRun27.mst",":SqlRun28.mst",":SqlRun29.mst",":SqlRun30.mst",
            ":SqlRun31.mst",":SqlRun32.mst",":SqlRun33.mst",":SqlRun34.mst",":SqlRun35.mst",":SqlRun36.mst",":SqlRun37.mst",":SqlRun38.mst",":SqlRun39.mst",":SqlRun40.mst",
            ":SqlRun41.mst",":SqlRun42.mst",":SqlRun43.mst",":SqlRun44.mst",":SqlRun45.mst",":SqlRun46.mst",":SqlRun47.mst",":SqlRun48.mst",":SqlRun49.mst",":SqlRun50.mst"


       };
        //Try to apply transform to msi package and return a list of product codes
        public static HashSet<String> GetTransformProductCode(string msi)
        {
            HashSet<string> productCodes = new HashSet<string>();

            using (var db = new QDatabase(msi.ToLower(), DatabaseOpenMode.ReadOnly))
            {
                Logger.LogMsg("******" + msi);
                try
                {
                    int cnt = 0;
                    foreach (string run in SqlRuns)
                    {

                        db.ApplyTransform(run, (TransformErrors)319);
                        cnt++;
                        //example:
                        //"SELECT `Table`, `Column`, `Row`, `Data`, `Current` FROM `_TransformView`";
                        //table="Property",Column="Value",Row="SqlRun",Data="SqlRun01.mst",Current="SqlRun00"
                        //table="Property",Column="Value",Row="ProductCode",Data="{C2D85992-11D5-4BC8-A8BF-77B6749FA9AC}",Current="{18B3B793-092C-4D03-BD6B-D388676CA997}"
                        string SqlSelectString = "SELECT `Table`, `Column`, `Row`, `Data`, `Current` FROM `_TransformView`";
                        string codeStr = SqlSelectString + " WHERE Row='ProductCode' ";

                        var ts = db.ExecuteQuery(codeStr);

                        if (ts.Count == 5)
                        {
                            string productCode = ts[3].ToString();
                            productCodes.Add(productCode);
                            //Logger.LogMsg("ApplyTransform:ProductCode=" + productCode);
                        }


                    }
                    Logger.LogMsg("ApplyTransform to \"" + msi + "\",Count:" + cnt + " ");
                }
                catch (Exception ex)
                {
                    //it is warning instead of error, some msi doesn't have transform
                    Logger.LogWarning("ApplyTransform:\"" + msi + "\":" + ex.Message);
                }
            }

            return productCodes;
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
