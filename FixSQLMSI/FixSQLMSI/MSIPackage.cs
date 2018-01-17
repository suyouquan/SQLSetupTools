using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using System.IO;

namespace FixSQLMSI
{
    public class MsiMspPackage
    {
        public string MsiMspFileName;
        public string FullPath;
        public string ProductName;
        public string ProductVersion; //if msp it is PatchVersion
        public string ProductCode = "";  //msi, if msp , get it from sumary:targets colume
        public string UpgradeCode = "";
        public string PackageCode = "";//for msi
        public string PatchCode = "";//for msp
        public string BaselineVersion = ""; //For msp 
        public bool isMsp = false;
        public bool failedPackage = false;
       // public QDatabase qData;


        

        public MsiMspPackage(string msimspFileName)
        {
            MsiMspFileName = Path.GetFileName(msimspFileName);
            FullPath = msimspFileName;

            try
            {
                //Note:have to use using to open the Qdatabase otherwise will get error 110 
                //if open a few msi/msp:
                // QDatabase not able to open file name in uppper case. so lower case to open it.
                using (var qData = new QDatabase(msimspFileName.ToLower(), DatabaseOpenMode.ReadOnly))
                {
                    if (MsiMspFileName.ToUpper().EndsWith(".MSI"))
                    {
                        foreach (var p in qData.Properties)
                        {
                            if (p.Property == "ProductName") ProductName = p.Value;
                            if (p.Property == "ProductVersion") ProductVersion = p.Value;
                            if (p.Property == "ProductCode")
                            {
                                ProductCode = p.Value;


                            }
                            if (p.Property == "UpgradeCode") UpgradeCode = p.Value;

                        }

                        try
                        {
                            this.PackageCode = qData.SummaryInfo.RevisionNumber;

                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex.Message);
                        }
                    }
                    else if (MsiMspFileName.ToUpper().EndsWith(".MSP"))
                    {
                        var tbl = qData.Tables["MsiPatchMetadata"];

                        if (tbl != null)
                        {
                            var props = qData.ExecuteStringQuery("SELECT `Property`,`Value` FROM `MsiPatchMetadata`");



                            for (int i = 0; i < props.Count - 1; i++)
                            {
                                string prop = props[i];
                                if (prop == "DisplayName") this.ProductName = props[i + 1];
                                if (prop == "PatchVersion") this.ProductVersion = props[i + 1];
                                if (prop == "BaselineVersion") this.BaselineVersion = props[i + 1];

                            }

                            //  var  PatchVersion= qData.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", "PatchVersion") ;
                            //  string name = qData.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", "DisplayName") as string;
                            // string baselineVersion = qData.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", "BaselineVersion") as string;

                        }

                        this.ProductCode = qData.SummaryInfo.Template;
                        this.isMsp = true;

                        try
                        {
                            this.PatchCode = qData.SummaryInfo.RevisionNumber;

                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex.Message);
                        }


                    }

                    else { throw new InvalidDataException("Invalid file name:" + msimspFileName); };


                }
            }
            catch (Exception ex)
            {
                Logger.LogError(":MsiMspPackage:" + ex.Message);
               
                failedPackage = true;
                return;
            }


        }

       
        public static List<MsiMspPackage> ScanSetupMedia(string folder)
        {

            List<string> msiFiles = Utility.GetFilesFromFolder(folder, new string[] { "*.msi" ,"*.msp"});

              List<MsiMspPackage> tmp_Packages = new List<MsiMspPackage>();


            //now for each msi file, get its product code

            foreach (string f in msiFiles)
            {
                if (f.ToUpper().Contains("IA64")) continue;
                MsiMspPackage pkg = new MsiMspPackage(f);
                if(!pkg.failedPackage)     tmp_Packages.Add(pkg);


            }
            return tmp_Packages;
        }





       








    }
}
