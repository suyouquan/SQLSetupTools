using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;

using System.Xml.Serialization;

using System.Xml.XmlConfiguration;
using System.Xml.Schema;



namespace SQLReg
{
    public class MsiRegistry
    {
        [XmlAttribute("Root")]
        public int Root;
        [XmlAttribute("Key")]
        public string Key;
        [XmlAttribute("Name")]
        public string Name;
        [XmlAttribute("Value")]
        public string Value;
        public MsiRegistry() { }
    }

    [XmlRootAttribute("MsiPackage",IsNullable = false)]
    public class MsiPackage
    {
        [XmlAttribute("PackageName")]
        public string PackageName;

        [XmlAttribute("FullPath")]
        public string FullPath;

        [XmlAttribute("ProductName")]
        public string ProductName;

        [XmlAttribute("ProductVersion")]
        public string ProductVersion;

        [XmlAttribute("ProductCode")]
        public string ProductCode = "";  //msi,  

        [XmlAttribute("UpgradeCode")]
        public string UpgradeCode = "";

        [XmlAttribute("PackageCode")]
        public string PackageCode;

        [XmlAttribute("failedPackage")]
        public bool failedPackage = false;

        [XmlAttribute("Cpu")]
        public string Cpu;//PlatformId, x86 or x64

        [XmlAttribute("ProductLanguage")]
        public string ProductLanguage;
        // public QDatabase qData;

        //list of products after transform
        public HashSet<String> TransformProductCodes = new HashSet<string>();
        public HashSet<String> Files = new HashSet<string>();//files in the MSI package
        
        // registry keys from MSI package
      
         public List<MsiRegistry> MsiRegistries = new List<MsiRegistry>();

        public MsiPackage( ) { }
        public MsiPackage(string msiFile)
        {
            //Do we need to take care of upcase or lower case, say, SQL_TOOLS.msi or sql_tools.msi??
            PackageName = Path.GetFileName(msiFile);
            FullPath = msiFile;

            try
            {
                //Note:have to use using to open the Qdatabase otherwise will get error 110 
                //if open a few msi/msp:
                // QDatabase not able to open file name in uppper case. so lower case to open it.
                using (var qData = new QDatabase(msiFile.ToLower(), DatabaseOpenMode.ReadOnly))
                {
                    if (msiFile.ToUpper().EndsWith(".MSI"))
                    {
                        foreach (var p in qData.Properties)
                        {

                            switch (p.Property)
                            {
                                case "ProductName": this.ProductName = p.Value; break;
                                case "ProductVersion": this.ProductVersion = p.Value; break;
                                case "ProductCode": this.ProductCode = p.Value; break;
                                case "UpgradeCode": this.UpgradeCode = p.Value; break;
                                case "PlatformId": this.Cpu = p.Value; break;
                                case "ProductLanguage": this.ProductLanguage = p.Value; break;

                                default:; break;
                            }


                        }
                        //Now try to apply transform and get its product code list after transform
                        TransformProductCodes = MSIHelper.GetTransformProductCode(FullPath);
                        this.PackageCode = qData.SummaryInfo.RevisionNumber;

                        //Now to read File list 
                        foreach (var f in qData.Files)
                        {
                            string[] names = f.FileName.Split('|');
                            if (names.Length > 1)
                            {
                                if (!string.IsNullOrEmpty(names[1]) && !Files.Contains(names[1]))
                                    Files.Add(names[1]);
                            }
                            else
                            {
                                if (!Files.Contains(f.FileName))
                                    Files.Add(f.FileName);

                            }
                        }

                        /*
                         
                         Use the following table to determine the ROOT registry value:
                         -1
                         For Per-User Installs, the registry value is written under HKCU; Per-Machine is HKLM
                         0
                         HKEY_CLASSES_ROOT
                         1
                         HKEY_CURRENT_USER
                         2
                         HKEY_LOCAL_MACHINE
                         3
                         HKEY_USERS
                         */

                        //Now read regitry keys
                        /*
                         if is is "*" The key is to be created, if absent, when the component is installed. Additionally, the key is to be deleted, if present, with all of its values and subkeys, when the component is uninstalled.
                         */
                        //only read those whose name is "*" which means the setup owns this key excusivlly
                        //we cannot delete the key shared with others, say, setup add things under key created by others
                        Dictionary<int, string> rootMap = new Dictionary<int, string>()
                        {
                            {0,"HKEY_CLASSES_ROOT" },
                            {1,"HKEY_CURRENT_USER" },
                            {2,"HKEY_LOCAL_MACHINE" },
                            {3,"HKEY_USERS" }

                        };


                        //some msi doesn't have registry 

                        try
                        {
                            foreach (var r in qData.Registries)
                            {



                                MsiRegistry smr = new MsiRegistry();
                                smr.Root = (int)r.Root;
                                smr.Key = r.Key;
                                smr.Name = r.Name;
                                smr.Value = r.Value;

                                MsiRegistries.Add(smr);
                            }

                        }catch(Exception ex)
                        {
                            Logger.LogError("MsiPackage:qData.Registries:\n" + ex.Message);
                        }

                    }
                    else throw new InvalidDataException("[" + msiFile + "]The file doesn't have msi extension!");

                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MsiPackage:" + ex.Message);

                failedPackage = true;
                return;
            }



        }







    }
}
