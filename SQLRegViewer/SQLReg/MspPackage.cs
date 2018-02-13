using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using System.Xml;
using System.Xml.Serialization;

namespace SQLReg
{
    [XmlRootAttribute("MspPackage", IsNullable = false)]
    public class MspPackage
    {

        [XmlAttribute("FullPath")]
        public string FullPath;

        [XmlAttribute("PatchCode")]
        public string PatchCode = "";

        [XmlAttribute("Targets")]
        public string Targets;

        [XmlAttribute("PackageName")]
        public string PackageName;

        [XmlAttribute("DisplayName")]
        public string DisplayName;

        [XmlAttribute("Description")]
        public string Description;

        [XmlAttribute("PatchVersion")]
        public string PatchVersion; //if msp it is PatchVersion

        [XmlAttribute("BaselineVersion")]
        public string BaselineVersion = ""; //For msp 

        [XmlAttribute("KBArticle")]
        public string KBArticle = "";

        [XmlAttribute("MoreInfoURL")]
        public string MoreInfoURL;

        [XmlAttribute("TargetProductName")]
        public string TargetProductName = "";

        [XmlAttribute("PatchFamily")]
        public string PatchFamily;


        [XmlAttribute("failedPackage")]
                public bool failedPackage = false;

        public MspPackage() { }
        public MspPackage(string mspFile)
        {

            PackageName = Path.GetFileName(mspFile);
            FullPath = mspFile;

            try
            {
                //Note:have to use using to open the Qdatabase otherwise will get error 110 
                //if open a few msi/msp:
                // QDatabase not able to open file name in uppper case. so lower case to open it.
                using (var qData = new QDatabase(mspFile.ToLower(), DatabaseOpenMode.ReadOnly))
                {
                     if (mspFile.ToUpper().EndsWith(".MSP"))
                    {
                        var tbl = qData.Tables["MsiPatchMetadata"];

                        if (tbl != null)
                        {
                            var props = qData.ExecuteStringQuery("SELECT `Property`,`Value` FROM `MsiPatchMetadata`");



                            for (int i = 0; i < props.Count - 1; i++)
                            {
                                string prop = props[i];
                                switch (prop)
                                {
                                    case "DisplayName": this.DisplayName = props[i + 1]; break;
                                    case "PatchVersion": this.PatchVersion = props[i + 1]; break;
                                    case "BaselineVersion": this.BaselineVersion = props[i + 1]; break;
                                    case "TargetProductName": this.TargetProductName = props[i + 1]; break;
                                    case "MoreInfoURL": this.MoreInfoURL = props[i + 1]; break;
                                    case "KBArticle": this.KBArticle = props[i + 1]; break;
                                    case "Description": this.Description = props[i + 1]; break;

                                    default:;break;
                                }
                               

                            }

               
                        }

                        
                         this.PatchCode = qData.SummaryInfo.RevisionNumber;
                        this.Targets = qData.SummaryInfo.Template;





                    }

                    else { throw new InvalidDataException("Invalid file name:" + mspFile); };


                }
            }
            catch (Exception ex)
            {
                Logger.LogError("MspPackage:" + ex.Message);

                failedPackage = true;
                return;
            }

        }

    }
}
