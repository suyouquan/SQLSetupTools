using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SQLReg
{
    [XmlRootAttribute("SQLProduct", IsNullable = false)]
    public class SQLProduct
    {

        [XmlAttribute("ProductFamily")]
        public string ProductFamily = "";
        [XmlIgnore]
        public string source;

        public List<MsiPackage> msiPackages = new List<MsiPackage>();
        public List<MspPackage> mspPackages = new List<MspPackage>();


      

        
        public SQLProduct() { }
        public SQLProduct(string pdFamily, string path)
        {
            ProductFamily = pdFamily;

            source = path;


            ScanSetupMedia(path);
         //   Controller.gSQLProduct.InitOrAddHashSet(this);
          
        }
     
        //load SQLProduct from meta
        public SQLProduct(string filePath)
        {


            SQLProduct p = (SQLProduct)OutputProcessor.DeserializeFromXML<SQLProduct>(filePath);
            this.msiPackages = p.msiPackages;
            this.mspPackages = p.mspPackages;

            if (p != null)
            {
            //    Controller.gSQLProduct.InitOrAddHashSet(this);
            }

             
        }
        public void ScanSetupMedia(string folder)
        {
            Controller.UpdateProgress("Scanning msi/msp from " + folder + ",may take minutes...", true);
            List<string> msiFiles = Utility.GetFilesFromFolder(folder, new string[] { "*.msi", "*.msp" });


            //now for each msi file, get its product code

            Controller.UpdateProgress("Generate packages" + "...", true);
            int cnt = 0;
            //package code and patch code can uniquely identity a package
            HashSet<string> packageCodes = new HashSet<string>();
            HashSet<string> patchCodes = new HashSet<string>();

            foreach (string f in msiFiles)
            {
                Controller.UpdateProgress("Reading ("+cnt+") " + f);
                if (f.ToUpper().Contains("IA64"))
                {
                    Controller.UpdateProgress("[WARNING]" + f + " contains \"IA64\",ignore it");
                    continue;
                }

                //If closing
                if (Controller.shouldAbort) return;


                /*
                    //ignore those VC related
                    if (f.ToLower().Contains("\\redist"))
                    {
                        Controller.UpdateProgress("[WARNING]" + f + " contains \"Redist\",ignore it");
                        continue;
                    }
                    */

                if (f.ToLower().EndsWith(".msi"))
                {
                    MsiPackage mp = new MsiPackage(f);

                    if (!string.IsNullOrEmpty(mp.PackageCode))
                    {
                        if (packageCodes.Contains(mp.PackageCode)) continue;
                        packageCodes.Add(mp.PackageCode);
                    }
                    else
                    {
                        Controller.UpdateProgress("[WARNING]" + mp.PackageName + ":package code is empty or null", true);
                    }
                    //if (!string.IsNullOrEmpty(mp.ProductName) 
                    //    &&  (mp.ProductName.ToLower().Contains("visual studio")
                    //    || mp.ProductName.ToLower().Contains("visualstudio"))
                    //    )
                    //{
                    //    Controller.UpdateProgress("[WARNING]" + f + " contains \"Visual Studio\",ignore it");
                    //    continue;
                    //}


                    if (!mp.failedPackage) msiPackages.Add(mp);
                }
                else //msp
                {
                    MspPackage mp = new MspPackage(f);

                    if (!string.IsNullOrEmpty(mp.PatchCode))
                    {
                        if (packageCodes.Contains(mp.PatchCode)) continue;
                        packageCodes.Add(mp.PatchCode);
                    }
                    else
                    {
                        Controller.UpdateProgress("[WARNING]" + mp.PackageName + ":patch code is empty or null", true);
                    }

                    //if (!string.IsNullOrEmpty(mp.DisplayName) && 
                    //      (mp.DisplayName.ToLower().Contains("visual studio")
                    //       || mp.DisplayName.ToLower().Contains("visualstudio"))
                    //       )
                    //{
                    //    Controller.UpdateProgress("[WARNING]" + f + " contains \"Visual Studio\",ignore it");
                    //    continue;
                    //}


                    if (!mp.failedPackage) mspPackages.Add(mp);
                }
                cnt++;
            }
            Controller.UpdateProgress("======== Total " + cnt + " Packages" + " processed.========");

        }



     











    }
}
