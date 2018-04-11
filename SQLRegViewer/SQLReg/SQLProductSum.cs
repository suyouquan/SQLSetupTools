using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace SQLReg
{
    //  [XmlRootAttribute("g_SQLProduct", IsNullable = false)]
    [DataContract(Name = "SQLProductSum")]
    public class SQLProductSum
    {
        [DataMember]
        public String name = "";
        [DataMember]
        public String description = "";
        [DataMember]
        public String source = "";
        [DataMember]
        public HashSet<string> packageNames = new HashSet<string>();
        [DataMember]
        public HashSet<string> productCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> compressedProductCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> upgradeCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> compressedUpgradeCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> PackageCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> compressedPackageCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> patchCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> compressedPatchCodes = new HashSet<string>();
        [DataMember]
        public HashSet<string> kbArticles = new HashSet<string>();
        [DataMember]
        public HashSet<string> targetProductNames = new HashSet<string>();
        [DataMember]
        public HashSet<string> targets = new HashSet<string>();
        [DataMember]
        public HashSet<string> files_EXE_DLL = new HashSet<string>();


        [DataMember]
        public Dictionary<string, string> MsiRegistries = new Dictionary<string, string>();

        //use below dictionary to give sqlRegKeys a detailed reason
        [DataMember]
        public Dictionary<string, string> productCodeToName = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> patchCodeToName = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> packageToName = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> packageCodeToName = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> upgradeCodeToName = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> othersToName = new Dictionary<string, string>();

        public SQLProductSum(string nm, string src, string desc)
        {
            name = nm;
            description = desc;
            source = src;
        }



        public void Reset()
        {


            packageNames.Clear();

            productCodes.Clear();

            compressedProductCodes.Clear();

            upgradeCodes.Clear();

            compressedUpgradeCodes.Clear();

            PackageCodes.Clear();

            compressedPackageCodes.Clear();

            patchCodes.Clear();

            compressedPatchCodes.Clear();

            kbArticles.Clear();

            targetProductNames.Clear();

            targets.Clear();

            files_EXE_DLL.Clear();

            MsiRegistries.Clear();

            productCodeToName.Clear();
            patchCodeToName.Clear();
            packageToName.Clear();
            packageCodeToName.Clear();
            upgradeCodeToName.Clear();
            othersToName.Clear();



        }


        public void SaveData(string desc, string fileName)
        {

            this.description = desc;
            string data = OutputProcessor.DataContractSerializeToXML<SQLProductSum>(this);
            File.WriteAllText(fileName, data);
        }
        public void AddProductSum(SQLProductSum newsum)
        {
            this.source = this.source + "|" + newsum.source;
            this.name = this.name + "|" + newsum.name;
            this.description = this.description + "|" + newsum.description;

            packageNames.UnionWith(newsum.packageNames);
            productCodes.UnionWith(newsum.productCodes);
            compressedProductCodes.UnionWith(newsum.compressedProductCodes);
            upgradeCodes.UnionWith(newsum.upgradeCodes);
            compressedUpgradeCodes.UnionWith(newsum.compressedUpgradeCodes);
            PackageCodes.UnionWith(newsum.PackageCodes);
            compressedPackageCodes.UnionWith(newsum.compressedPackageCodes);
            patchCodes.UnionWith(newsum.patchCodes);
            compressedPatchCodes.UnionWith(newsum.compressedPatchCodes);
            kbArticles.UnionWith(newsum.kbArticles);
            targetProductNames.UnionWith(newsum.targetProductNames);
            targets.UnionWith(newsum.targets);
            files_EXE_DLL.UnionWith(newsum.files_EXE_DLL);

            MsiRegistries = MsiRegistries.Concat(newsum.MsiRegistries).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);

            productCodeToName = productCodeToName.Union(newsum.productCodeToName).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);
            patchCodeToName = patchCodeToName.Union(newsum.patchCodeToName).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);
            packageToName = packageToName.Union(newsum.packageToName).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);
            packageCodeToName = packageCodeToName.Union(newsum.packageCodeToName).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);
            upgradeCodeToName = upgradeCodeToName.Union(newsum.upgradeCodeToName).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);
            othersToName = othersToName.Union(newsum.othersToName).GroupBy(e => e.Key).ToDictionary(g => g.Key, g => g.First().Value);

        }
        public void InitOrAddHashSet(SQLProduct me)
        {
            Logger.LogMsg("InitOrAddHashSet started.");
            int ignore = 0;
            int cnt = 0;
            foreach (MsiPackage pkg in me.msiPackages)
            {
                //If closing
                if (Controller.shouldAbort) return;


                cnt++;

                try
                {
                    //Ignore Visual Studio related things?

                    if (string.IsNullOrEmpty(pkg.ProductName))
                    {
                        ignore++;
                        Controller.UpdateProgress("[Ignore]" + pkg.PackageName + " because product name is empty", true);
                        continue;
                    }
                    /*
                    if ((pkg.ProductName.ToLower().Contains("visual studio")
                              || pkg.ProductName.ToLower().Contains("visualstudio")))
                    {
                        ignore++;
                        Controller.UpdateProgress("[Ignore]" + pkg.PackageName + " " + pkg.ProductName, true);
                        continue;
                    }
                    */
                    //I want to ignore those redist packages

                    if (pkg.FullPath.ToLower().Contains("redist"))
                    {
                        ignore++;
                        Controller.UpdateProgress("[Ignore]" + pkg.FullPath, true);
                        continue;
                    }


                    Controller.UpdateProgress("Analyzing meta data for (" + cnt + ") " + pkg.PackageName, false);

                    if (!String.IsNullOrEmpty(pkg.ProductCode) && !productCodes.Contains(pkg.ProductCode))
                    {
                        string compressedPd = Utility.ConvertGUID2CompressedRegistryStyle(pkg.ProductCode);
                        productCodes.Add(pkg.ProductCode);
                        compressedProductCodes.Add(compressedPd);
                        if (!productCodeToName.ContainsKey(compressedPd)) productCodeToName.Add(compressedPd, pkg.ProductName);
                    }

                    if (pkg.TransformProductCodes != null)
                    {
                        foreach (string pc in pkg.TransformProductCodes)
                        {
                            if (!String.IsNullOrEmpty(pc) && !productCodes.Contains(pc))
                            {
                                string compd = Utility.ConvertGUID2CompressedRegistryStyle(pc);
                                productCodes.Add(pc);
                                compressedProductCodes.Add(compd);
                                if (!productCodeToName.ContainsKey(compd)) productCodeToName.Add(compd, pkg.ProductName);
                            }

                        }
                    }

                    if (!string.IsNullOrEmpty(pkg.PackageName))
                    {
                        string name = Path.GetFileName(pkg.PackageName).ToLower();
                        if (!packageNames.Contains(name))
                        {
                            packageNames.Add(name);
                            if (!packageToName.ContainsKey(name)) packageToName.Add(name, pkg.ProductName);

                        }
                    }

                    if (!string.IsNullOrEmpty(pkg.UpgradeCode))
                    {
                        if (!upgradeCodes.Contains(pkg.UpgradeCode))
                        {
                            string ugrpd = Utility.ConvertGUID2CompressedRegistryStyle(pkg.UpgradeCode);
                            upgradeCodes.Add(pkg.UpgradeCode);
                            compressedUpgradeCodes.Add(ugrpd);
                            if (!upgradeCodeToName.ContainsKey(ugrpd)) upgradeCodeToName.Add(ugrpd, pkg.ProductName);
                        }


                    }


                    if (!string.IsNullOrEmpty(pkg.PackageCode))
                    {
                        if (!PackageCodes.Contains(pkg.PackageCode))
                        {
                            string pkgcd = Utility.ConvertGUID2CompressedRegistryStyle(pkg.PackageCode);
                            PackageCodes.Add(pkg.UpgradeCode);

                            compressedPackageCodes.Add(pkgcd);
                            if (!packageCodeToName.ContainsKey(pkgcd))
                                packageCodeToName.Add(pkgcd, pkg.ProductName);
                        }


                    }

                    foreach (string f in pkg.Files)
                    {
                        if (!files_EXE_DLL.Contains(f)
                            && (f.ToLower().EndsWith(".dll") || f.ToLower().EndsWith(".exe"))
                            )
                        {
                            files_EXE_DLL.Add(f.ToLower());

                        }
                    }



                    foreach (MsiRegistry mr in pkg.MsiRegistries)
                    {


                        AddKey(mr);


                    }
                }
                catch (Exception ex)
                {
                    Controller.UpdateProgress("[InitOrAddHashSet]" + pkg.PackageName + "\n" + ex.Message, true);
                }

            }//foreach



            //for MSP

            cnt = 0;
            foreach (MspPackage pkg in me.mspPackages)
            {
                //If closing
                if (Controller.shouldAbort) return;

                cnt++;
                Controller.UpdateProgress("Analyzing meta data for (" + cnt + ") " + pkg.PackageName, false);

                try
                {
                    if (string.IsNullOrEmpty(pkg.DisplayName))
                    {
                        ignore++;
                        Controller.UpdateProgress("[Ignore]" + pkg.PackageName + " because DisplayName  is empty", true);
                        continue;
                    }
                    /*
                    //ignore Visual Studio thing
                    if ((pkg.DisplayName.ToLower().Contains("visual studio")
                            || pkg.DisplayName.ToLower().Contains("visualstudio")))
                    {
                        Controller.UpdateProgress("[Ignore]" + pkg.PackageName + " " + pkg.DisplayName, true);
                        continue;
                    }
                    */
                    //I want to ignore those redist packages

                    if (pkg.FullPath.ToLower().Contains("redist"))
                    {
                        ignore++;
                        Controller.UpdateProgress("[Ignore]" + pkg.FullPath, true);
                        continue;
                    }


                    if (!string.IsNullOrEmpty(pkg.PackageName))
                    {
                        string name = Path.GetFileName(pkg.PackageName).ToLower();
                        if (!packageNames.Contains(name))
                        {
                            packageNames.Add(name);
                            if (!packageToName.ContainsKey(name))
                                packageToName.Add(name, pkg.DisplayName);
                        }
                    }


                    if (!string.IsNullOrEmpty(pkg.PatchCode))
                    {
                        if (!patchCodes.Contains(pkg.PatchCode))
                        {
                            string pcd = Utility.ConvertGUID2CompressedRegistryStyle(pkg.PatchCode);
                            patchCodes.Add(pkg.PatchCode);
                            compressedPatchCodes.Add(pcd);
                            if (!patchCodeToName.ContainsKey(pcd))
                                patchCodeToName.Add(pcd, pkg.DisplayName);

                        }
                    }



                    if (!string.IsNullOrEmpty(pkg.TargetProductName))
                    {
                        if (!targetProductNames.Contains(pkg.TargetProductName))
                        {
                            targetProductNames.Add(pkg.TargetProductName);
                            if (!othersToName.ContainsKey(pkg.TargetProductName))
                                othersToName.Add(pkg.TargetProductName, pkg.DisplayName);
                        }

                    }

                    if (!string.IsNullOrEmpty(pkg.KBArticle))
                    {
                        if (!kbArticles.Contains(pkg.KBArticle.ToLower()))
                        {
                            kbArticles.Add(pkg.KBArticle.ToLower());
                            if (!othersToName.ContainsKey(pkg.KBArticle))
                                othersToName.Add(pkg.KBArticle.ToLower(), pkg.DisplayName);
                        }


                    }

                    if (!string.IsNullOrEmpty(pkg.Targets))
                    {
                        if (!targets.Contains(pkg.Targets))
                        {
                            targets.Add(pkg.Targets);
                            if (!othersToName.ContainsKey(pkg.Targets))
                                othersToName.Add(pkg.Targets, pkg.DisplayName);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Controller.UpdateProgress("[InitOrAddHashSet]" + pkg.PackageName + "\n" + ex.Message, true);
                }

            }//foreach

            // LoadSQLProductFromSetupSrc is called before LoadSQLProductFromMetaData,
            //so below information will tell how many new things got from user speficied folder.

            Controller.UpdateProgress("==============================================", true);
            Controller.UpdateProgress("Source:" + me.source, true);
            Controller.UpdateProgress(me.msiPackages.Count + " MSI packages meta data processed.", true);
            Controller.UpdateProgress(me.mspPackages.Count + " MSP packages meta data processed.", true);
            Controller.UpdateProgress("==============================================", true);
            Controller.UpdateProgress("Cumulative results:", true);
            Controller.UpdateProgress("Unique Packgage Name:" + packageNames.Count, true);
            Controller.UpdateProgress("Ignore Packgages:" + ignore, true);
            Controller.UpdateProgress("Package Codes:" + PackageCodes.Count, true);
            Controller.UpdateProgress("Product Codes:" + productCodes.Count, true);
            Controller.UpdateProgress("Patch Codes:" + patchCodes.Count, true);
            Controller.UpdateProgress("Upgrade Codes:" + upgradeCodes.Count, true);
            Controller.UpdateProgress("EXE/DLL files:" + files_EXE_DLL.Count, true);
            Controller.UpdateProgress("Registries:" + MsiRegistries.Count, true);
            Controller.UpdateProgress("KB Articles:" + kbArticles.Count, true);
            Controller.UpdateProgress("targetProductNames:" + targetProductNames.Count, true);
            Controller.UpdateProgress("targets:" + targets.Count, true);

            Controller.UpdateProgress("==============================================", true);
            Controller.UpdateProgress("");


        }//function


        public void AddProductSumFromFile(string f)
        {

            try
            {
                Logger.LogMsg("AddProductSumFromFile:" + f);
                SQLProductSum sum = OutputProcessor.DataContractDeSerializeToXML<SQLProductSum>(f);
                this.AddProductSum(sum);

            }
            catch (Exception ex)
            {
                Logger.LogError("[AddProductSumFromFile]:" + ex.Message);
            }

        }
        private void AddKey(MsiRegistry mr)
        {

            Dictionary<string, string> keys = MsiRegistries;


            int root = mr.Root;
            if (root == -1) root = 2;//for sql server, should be per machine, not per user. so just use 2.
            if (root < 0 || root > 3)
            {
                Logger.LogWarning("root=" + root + " which is out of valid range," + "" + mr.Key);
                return;//don't check invalid values
            }

            string HK = RegHelper.rootMap[root];
            string path = HK + "\\" + mr.Key;

            if (mr.Name == "-" && String.IsNullOrEmpty(mr.Value))
            {

            }

            //First if it already exists, need to check whether name=* or not

            if (keys.ContainsKey(path))
            {
                //If new one is *, then change existing one
                if (mr.Name == "*")
                    keys[path] = "*";
                return;
            }

            //otherwise

            //Check whether it has parent in it
            //If my name!="*" and there is parent there already, don't add it
            //If myname="*" and there is parent in it, add me.
            //

            if (mr.Name == "*")
            {
                //If there is parent, and  parent also =*, don't add it.
                //If there is parent but parent !=*, add it
                //If there is no parent, add it.
                //If there is kid, delete kid
                int hasParent = 0;
                int parentNameIsStar = 0;
                List<string> kids = new List<string>();
                foreach (KeyValuePair<string, string> kv in keys)
                {
                    //check parent keys
                    if (path.StartsWith(kv.Key))
                    {
                        hasParent++;
                        if (kv.Value == "*") parentNameIsStar++;
                    }

                    //check kid
                    if (kv.Key.StartsWith(path)) kids.Add(kv.Key);

                }

                //No parent, add it.
                if (hasParent <= 0) keys.Add(path, mr.Name);
                //Has parent,but parent is not star
                else if (parentNameIsStar == 0) keys.Add(path, mr.Name);

                //delete keys, becuase i am with star, any kids should be deleted
                foreach (string s in kids)
                    keys.Remove(s);

            }
            /*
            else //Do nothing, don't add them
            {
               
                //If there is parent,  don't add it.
                //If there is no parent, add it.
                //If there is kid, if kid is star, don't delete it. otherwise delete it
                int hasParent = 0;
                int parentNameIsStar = 0;
                Dictionary<string, string> kids = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kv in keys)
                {
                    //check parent keys
                    if (path.StartsWith(kv.Key))
                    {
                        hasParent++;
                        if (kv.Value == "*") parentNameIsStar++;
                    }

                    //check kid
                    if (kv.Key.StartsWith(path)) kids.Add(kv.Key, kv.Value);

                }

                //No parent, add it.
                if (hasParent <= 0) keys.Add(path, mr.Name);


                //delete keys if kid is not star
                foreach (KeyValuePair<string, string> kv in kids)
                {
                    if(kv.Value!="*")
                    keys.Remove(kv.Key);
                }

            }
            */





        }






    }
}
