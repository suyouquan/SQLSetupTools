using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;

namespace SQLReg.Analysis
{
    public static class Installer
    {

        public static HashSet<string> foundCompressedProductCodes = new HashSet<string>();
        public static HashSet<string> foundCompressedUpradeCodes = new HashSet<string>();
        public static HashSet<string> foundCompressedPatchCode = new HashSet<string>();

       public static SQLProductSum sumSQLProduct = Controller.sumSQLProduct;

        /// <summary>
        /// the main function, to analzye whether registry key is SQL server related or not.
        /// </summary>
        /// 
        public static void Add()
        {
            foundCompressedProductCodes.Clear();
            foundCompressedUpradeCodes.Clear();
            foundCompressedPatchCode.Clear();
            
            //reassign if customer click scan again.
            sumSQLProduct = Controller.sumSQLProduct;

            int cnt = 0;

            cnt = cnt + Add_HKCR_Products();
            cnt = cnt + Add_HKLM_Products();
            //need to called after Products because Products will get patch code
            cnt = cnt + Add_HKCR_Patches();
            cnt = cnt + Add_HKLM_Patches();
            cnt = cnt + Add_HKCR_UpgradeCodes();
            cnt = cnt + Add_HKLM_UpgradeCodes();
         //   cnt = cnt + Add_HKLM_Components();
            cnt = cnt + Add_HKCR_Dependencies();
            cnt = cnt + Add_HKCR_Features();

            Controller.UpdateProgress("Installer Add keys:" + cnt, true);




        } //Add


        private static int Add_HKCR_Products()
        {

            int cnt = 0;
            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Products  
            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Products\FCB7990CA27B16A4B88236108DBC3510\SourceList
            string sub = "Products";
            Controller.UpdateProgress(@"Analyzing HKEY_CLASSES_ROOT\Installer\" + sub + "...", true);
            subKey sk = new subKey("SourceList", "PackageName");
            //  RegHive reghive = new RegHive(@"HKEY_CLASSES_ROOT\Installer\" + sub, sk);
            RegHive reghive = Controller.regHives[@"HKEY_CLASSES_ROOT\Installer\Products"];
            //first, check product code

            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_CLASSES_ROOT\Installer\" + sub + @"\{0}", key);
                //  Controller.UpdateProgress("Comparing " + reg);


                string comment = "Product code:" + key;
                if (sumSQLProduct.productCodeToName.ContainsKey(key)) comment = sumSQLProduct.productCodeToName[key];

                if (foundCompressedProductCodes.Contains(key)
                    || sumSQLProduct.compressedProductCodes.Contains(key)
                    )
                {

                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {

                        cnt++;
                        Controller.sqlRegKeys.Add(reg,
                        new Reason(SourceType.Installer_ProductCode, key, comment, true));


                    }
                    if (!foundCompressedProductCodes.Contains(key))
                        foundCompressedProductCodes.Add(key);

                    //Need to add patch information
                    string pKey = reg + "\\Patches";
                    if (RegHelper.IsKeyExist(pKey))
                    {
                        RegKey rk = new RegKey(pKey, "");
                        if (rk.Properties.ContainsKey("Patches"))
                        {
                            string[] patches = rk.Properties["Patches"][1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string s in patches)
                            {
                                if (!foundCompressedPatchCode.Contains(s.Trim()))
                                {
                                    foundCompressedPatchCode.Add(s.Trim());

                                }
                                if (!sumSQLProduct.patchCodeToName.ContainsKey(s.Trim()))
                                    sumSQLProduct.patchCodeToName.Add(s.Trim(), comment);
                            }
                        }


                    }

                } //match



            }
            //second, check package name to add more
            foreach (KeyValuePair<string, List<string>> kv in reghive.propertyMap)
            {

                //Just check *.msi, since we don't ship xml about patch.
                if (sumSQLProduct.packageNames.Contains(Path.GetFileName(kv.Key).ToLower()))
                {
                    foreach (string path in kv.Value)
                    {
                        string comment = "Package name matches:" + kv.Key;
                        string name1 = Path.GetFileName(kv.Key).ToLower();
                        if (sumSQLProduct.packageToName.ContainsKey(name1))
                            comment = sumSQLProduct.packageToName[name1];




                        if (!Controller.sqlRegKeys.ContainsKey(path))
                        {

                            cnt++;
                            Controller.sqlRegKeys.Add(path,
                            new Reason(SourceType.FromMSI_PackageName, kv.Key, comment, true));

                        }
                        int start = path.IndexOf("Products\\") + "Products\\".Length;
                        string code = path.Substring(start, 32);

                        if (!foundCompressedProductCodes.Contains(code))
                        {
                            foundCompressedProductCodes.Add(code);

                        }
                        if (!sumSQLProduct.productCodeToName.ContainsKey(code))
                            sumSQLProduct.productCodeToName.Add(code, comment);

                        //Need to add patch information
                        string pKey = path + "\\Patches";
                        if (RegHelper.IsKeyExist(pKey))
                        {
                            RegKey rk = new RegKey(pKey, "");
                            if (rk.Properties.ContainsKey("Patches"))
                            {
                                string[] patches = rk.Properties["Patches"][1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string s in patches)
                                {
                                    if (!foundCompressedPatchCode.Contains(s.Trim()))
                                    {
                                        foundCompressedPatchCode.Add(s.Trim());
                                    }
                                    if (!sumSQLProduct.patchCodeToName.ContainsKey(s.Trim()))
                                        sumSQLProduct.patchCodeToName.Add(s.Trim(), comment);
                                }
                            }


                        }

                    }

                } //match

            }

            //third, scan package code to see if any matches...could hit less

            foreach (RegKey rk in reghive.regKeys)
            {

                if (rk.Properties.ContainsKey("PackageCode"))
                {
                    string pcode = rk.Properties["PackageCode"][1];

                    if (sumSQLProduct.compressedPackageCodes.Contains(pcode))
                    {
                        string comment = "Package Code:" + pcode;
                        if (sumSQLProduct.packageCodeToName.ContainsKey(pcode))
                            comment = sumSQLProduct.packageCodeToName[pcode];

                        if (!Controller.sqlRegKeys.ContainsKey(rk.Path))
                        {

                            cnt++;
                            Controller.sqlRegKeys.Add(rk.Path,
                            new Reason(SourceType.FromMSI_PackageCode, pcode, comment, true));

                        }

                        int start = rk.Path.IndexOf("Products\\") + "Products\\".Length;
                        string code = rk.Path.Substring(start, 32);

                        if (!foundCompressedProductCodes.Contains(code))
                        {
                            foundCompressedProductCodes.Add(code);

                        }
                        if (!sumSQLProduct.productCodeToName.ContainsKey(code))
                            sumSQLProduct.productCodeToName.Add(code, comment);

                        //Need to add patch information
                        string pKey = rk.Path + "\\Patches";
                        if (RegHelper.IsKeyExist(pKey))
                        {
                            RegKey prk = new RegKey(pKey, "");
                            if (prk.Properties.ContainsKey("Patches"))
                            {
                                string[] patches = prk.Properties["Patches"][1].Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string s in patches)
                                {
                                    if (!foundCompressedPatchCode.Contains(s.Trim()))
                                    {
                                        foundCompressedPatchCode.Add(s.Trim());

                                    }
                                    if (!sumSQLProduct.patchCodeToName.ContainsKey(s.Trim()))
                                        sumSQLProduct.patchCodeToName.Add(s.Trim(), comment);
                                }
                            }


                        }


                    }

                }



            }//foreach
            Controller.UpdateProgress("Add_HKCR_Products keys:" + cnt, true);
            return cnt;
        }


        private static int Add_HKLM_Products()
        {
            int cnt = 0;
            /****************************************************************************************************/
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products  
            /****************************************************************************************************/
            string sub = "Products";
            Controller.UpdateProgress(@"Analyzing HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub + "...", true);
            // RegHive reghive = new RegHive(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub);
            RegHive reghive = Controller.regHives[@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products"];
            //first, check product code

            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub + @"\{0}", key);
                //  Controller.UpdateProgress("Comparing " + reg);



                if (foundCompressedProductCodes.Contains(key) //use this to shortcut it if it found already
                    || sumSQLProduct.compressedProductCodes.Contains(key)
                   )
                {
                    string comment = "Product code:" + key;
                    if (sumSQLProduct.productCodeToName.ContainsKey(key))
                        comment = sumSQLProduct.productCodeToName[key];


                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {

                        cnt++;
                        Controller.sqlRegKeys.Add(reg,
                        new Reason(SourceType.Installer_ProductCode, key, comment, true));

                    }
                    if (!foundCompressedProductCodes.Contains(key))
                    {
                        foundCompressedProductCodes.Add(key);

                    }
                    if (!sumSQLProduct.productCodeToName.ContainsKey(key))
                        sumSQLProduct.productCodeToName.Add(key, comment);
                    //Save the patch if any.

                    string patchKey = reg + "\\Patches";
                    HashSet<string> mypatches = RegHelper.GetSubKeys(patchKey);
                    foreach (string s in mypatches)
                    {
                        if (!foundCompressedPatchCode.Contains(s))
                            foundCompressedPatchCode.Add(s);

                        if (!sumSQLProduct.patchCodeToName.ContainsKey(s.Trim()))
                            sumSQLProduct.patchCodeToName.Add(s.Trim(), comment);


                    }


                }


            }
            Controller.UpdateProgress("Add_HKLM_Products keys:" + cnt, true);
            return cnt;
        }

        private static int Add_HKCR_Patches()
        {

            int cnt = 0;


            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Patches  
            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Patches\023089E60AE02D343916E2D48AFCAA73\SourceList

            string sub = "Patches";
            Controller.UpdateProgress(@"Analyzing HKEY_CLASSES_ROOT\Installer\" + sub + "...", true);
            subKey sk = new subKey("SourceList", "PackageName");
            //   RegHive reghive = new RegHive(@"HKEY_CLASSES_ROOT\Installer\" + sub, sk);

            RegHive reghive = Controller.regHives[@"HKEY_CLASSES_ROOT\Installer\Patches"];

            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_CLASSES_ROOT\Installer\" + sub + @"\{0}", key);
                //  Controller.UpdateProgress("Comparing " + reg);

                //for Patches, use Patch codes!!

                if (foundCompressedPatchCode.Contains(key)
                    || sumSQLProduct.compressedPatchCodes.Contains(key))
                {

                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {
                        string comment = "Patch code:" + key;
                        if (sumSQLProduct.patchCodeToName.ContainsKey(key)) comment = sumSQLProduct.patchCodeToName[key];


                        cnt++;
                        Controller.sqlRegKeys.Add(reg,
                        new Reason(SourceType.Installer_PatchCode, key, comment, true));



                    }

                    if (!foundCompressedPatchCode.Contains(key))
                        foundCompressedPatchCode.Add(key);

                }

            }

            //check source->packageName, whether match?
            foreach (KeyValuePair<string, List<string>> kv in reghive.propertyMap)
            {

                //check msp as well, sumSQLProduct.packageNames may not have .msp but it could have.msi file
                if (sumSQLProduct.packageNames.Contains(Path.GetFileName(kv.Key).ToLower())
                    || sumSQLProduct.packageNames.Contains(Path.GetFileNameWithoutExtension(kv.Key).ToLower() + ".msi")
                    )
                {
                    foreach (string path in kv.Value)
                    {

                        string comment = "Package name matches:" + kv.Key;
                        string name1 = Path.GetFileName(kv.Key).ToLower();
                        string name2 = Path.GetFileNameWithoutExtension(kv.Key).ToLower() + ".msi";
                        if (sumSQLProduct.packageToName.ContainsKey(name1))
                            comment = sumSQLProduct.packageToName[name1];

                        else if (sumSQLProduct.packageToName.ContainsKey(name2))
                            comment = sumSQLProduct.packageToName[name2];


                        if (!Controller.sqlRegKeys.ContainsKey(path))
                        {


                            cnt++;
                            Controller.sqlRegKeys.Add(path,
                            new Reason(SourceType.FromMSI_PackageName, kv.Key, comment, true));



                        }
                        int start = path.IndexOf("Patches\\") + "Patches\\".Length;
                        string code = path.Substring(start, 32);

                        if (!foundCompressedPatchCode.Contains(code))
                        {
                            foundCompressedPatchCode.Add(code);

                        }
                        if (!sumSQLProduct.patchCodeToName.ContainsKey(code))
                        {
                            sumSQLProduct.patchCodeToName.Add(code, comment);
                        }
                    }

                }

            }
            Controller.UpdateProgress("Add_HKCR_Patches keys:" + cnt, true);
            return cnt;
        }

        private static int Add_HKLM_Patches()
        {

            int cnt = 0;
            /****************************************************************************************************/
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Patches  
            /****************************************************************************************************/
            string sub = "Patches";
            Controller.UpdateProgress(@"Analyzing HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub + "...", true);
            // RegHive reghive = new RegHive(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub);
            RegHive reghive = Controller.regHives[@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Patches"];

            //first, check product code
            //using regKeys instead subkeys
            foreach (RegKey rk in reghive.regKeys)
            {

                string reg = rk.Path;

                //   Controller.UpdateProgress("Comparing " + reg);

                if (foundCompressedPatchCode.Contains(rk.leaf)
                    || sumSQLProduct.compressedPatchCodes.Contains(rk.leaf)
                    )
                {

                    string comment = "Patch code:" + rk.leaf;
                    if (sumSQLProduct.patchCodeToName.ContainsKey(rk.leaf))
                        comment = sumSQLProduct.patchCodeToName[rk.leaf];


                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {
                        cnt++;
                        Controller.sqlRegKeys.Add(reg,
                        new Reason(SourceType.Installer_PatchCode, rk.leaf, comment, true));

                    }

                    if (!foundCompressedPatchCode.Contains(rk.leaf))
                        foundCompressedPatchCode.Add(rk.leaf);


                }
                else
                {
                    //read the local package (*.msp) and read its patch code?

                    if (rk.Properties.ContainsKey("LocalPackage"))
                    {
                        string lp = rk.Properties["LocalPackage"][1];
                        MspPackage pkg = new MspPackage(lp);



                        if (!string.IsNullOrEmpty(pkg.TargetProductName))
                        {
                            if (sumSQLProduct.targetProductNames.Contains(pkg.TargetProductName))
                            {

                                string comment = "TargetProductName:" + pkg.TargetProductName;
                                if (sumSQLProduct.othersToName.ContainsKey(pkg.TargetProductName))
                                    comment = sumSQLProduct.patchCodeToName[pkg.TargetProductName];

                                if (!Controller.sqlRegKeys.ContainsKey(reg))
                                {

                                    cnt++;
                                    Controller.sqlRegKeys.Add(reg,
                                    new Reason(SourceType.FromMSI_TargetProductName, pkg.TargetProductName, comment, true));
                                }

                                if (!foundCompressedPatchCode.Contains(rk.leaf))
                                    foundCompressedPatchCode.Add(rk.leaf);

                            }

                        }//  if (!string.IsNullOrEmpty(pkg.TargetProductName))




                    }



                }//else



            }

            Controller.UpdateProgress("Add_HKLM_Patches keys:" + cnt, true);

            return cnt;

        }

        private static int Add_HKCR_Dependencies()
        {
            int cnt = 0;
            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Dependencies  
            /****************************************************************************************************/
            string sub = "Dependencies";
            //HKEY_CLASSES_ROOT\Installer\Dependencies\{070C38AC-05CE-43DF-9A20-141332F6AB2B}
            //hive_HKCR_Installer_Dependencies
            Controller.UpdateProgress(@"Analyzing HKEY_CLASSES_ROOT\Installer\" + sub + "...", true);
            //RegHive reghive = new RegHive(@"HKEY_CLASSES_ROOT\Installer\" + sub + "");

            RegHive reghive = Controller.regHives[@"HKEY_CLASSES_ROOT\Installer\Dependencies"];

            //its subkeys are product codes
            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_CLASSES_ROOT\Installer\" + sub + @"\{0}", key);
                // Controller.UpdateProgress("Comparing " + reg);


                if (sumSQLProduct.productCodes.Contains(key))
                {
                    string compKey = Utility.ConvertGUID2CompressedRegistryStyle(key);
                    string comment = "Product code from MSI package:" + key;
                    if (sumSQLProduct.productCodeToName.ContainsKey(compKey)) comment = sumSQLProduct.productCodeToName[compKey];

                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {
                        Controller.sqlRegKeys.Add(reg,
                            new Reason(SourceType.Installer_ProductCode, compKey, comment, false));
                        cnt++;

                    }

                    if (!foundCompressedProductCodes.Contains(compKey))
                        foundCompressedProductCodes.Add(compKey);


                }



            }
            Controller.UpdateProgress("Add_HKCR_Dependencies keys:" + cnt, true);
            return cnt;
        }

        private static int Add_HKCR_Features()
        {
            int cnt = 0;

            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Features  
            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\Features\0040152C8855F3932AB75A480ABAB749
            string sub = "Features";
            Controller.UpdateProgress(@"Analyzing HKEY_CLASSES_ROOT\Installer\" + sub + "...", true);
            //RegHive reghive = new RegHive(@"HKEY_CLASSES_ROOT\Installer\" + sub);
            RegHive reghive = Controller.regHives[@"HKEY_CLASSES_ROOT\Installer\Features"];

            //its subkeys are compressed product codes
            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_CLASSES_ROOT\Installer\" + sub + @"\{0}", key);
                //   Controller.UpdateProgress("Comparing " + reg);




                //for Features, use compressedProductCodes
                if (foundCompressedProductCodes.Contains(key)
                    || sumSQLProduct.compressedProductCodes.Contains(key))
                {

                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {

                        string comment = "Product code from MSI package:" + key;
                        if (sumSQLProduct.productCodeToName.ContainsKey(key)) comment = sumSQLProduct.productCodeToName[key];


                        Controller.sqlRegKeys.Add(reg,
                            new Reason(SourceType.Installer_ProductCode, key, comment, true)

                            );
                        cnt++;

                    }

                    if (!foundCompressedProductCodes.Contains(key))
                        foundCompressedProductCodes.Add(key);



                }


            }
            Controller.UpdateProgress("Add_HKCR_Features keys:" + cnt, true);
            return cnt;
        }



        private static int Add_HKCR_UpgradeCodes()
        {
            int cnt = 0;

            /****************************************************************************************************/
            //HKEY_CLASSES_ROOT\Installer\UpgradeCodes  
            /****************************************************************************************************/

            //HKEY_CLASSES_ROOT\Installer\UpgradeCodes\018E681CDBF373D3CA372AA3D490C521
            string sub = "UpgradeCodes";
            Controller.UpdateProgress(@"Analyzing HKEY_CLASSES_ROOT\Installer\" + sub + "...", true);
            //RegHive reghive = new RegHive(@"HKEY_CLASSES_ROOT\Installer\" + sub);

            RegHive reghive = Controller.regHives[@"HKEY_CLASSES_ROOT\Installer\UpgradeCodes"];

            //first, check upgrade code
            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_CLASSES_ROOT\Installer\" + sub + @"\{0}", key);
                //  Controller.UpdateProgress("Comparing " + reg);

                if (sumSQLProduct.compressedUpgradeCodes.Contains(key))
                {
                    string comment = "Upgrade Code:" + key;
                    if (sumSQLProduct.upgradeCodeToName.ContainsKey(key))
                        comment = sumSQLProduct.upgradeCodeToName[key];


                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {
                        cnt++;
                        Controller.sqlRegKeys.Add(reg,
                        new Reason(SourceType.Installer_UpgradeCode, key, comment, true));
                    }
                    if (!foundCompressedUpradeCodes.Contains(key))
                    {
                        foundCompressedUpradeCodes.Add(key);
                    }
                    if (!sumSQLProduct.upgradeCodeToName.ContainsKey(key))
                        sumSQLProduct.upgradeCodeToName.Add(key, comment);



                    RegKey rk = new RegKey(reg, "");
                    foreach (RegProperty rp in rk.RegProperties)
                    {
                        if (rp.Name.Length == 32)
                        {
                            if (!foundCompressedProductCodes.Contains(rp.Name))
                            {
                                foundCompressedProductCodes.Add(rp.Name);

                            }
                            if (!sumSQLProduct.productCodeToName.ContainsKey(rp.Name))
                                sumSQLProduct.productCodeToName.Add(rp.Name, comment);
                        }

                    }
                }


            }


            //second,check product code under the key
            //HKEY_CLASSES_ROOT\Installer\UpgradeCodes\0158E79888DB55D45B27961CE0F2428C->will store product code under this key

            foreach (RegKey rk in reghive.regKeys)
            {
                bool found = false;
                foreach (RegProperty rp in rk.RegProperties)
                {

                    //If any name matches product code
                    if (foundCompressedProductCodes.Contains(rp.Name) ||
                        sumSQLProduct.compressedProductCodes.Contains(rp.Name))
                    {

                        string comment = "Product code:" + rp.Name;
                        if (sumSQLProduct.productCodeToName.ContainsKey(rp.Name))
                            comment = sumSQLProduct.productCodeToName[rp.Name];


                        if (!Controller.sqlRegKeys.ContainsKey(rk.Path))
                        {

                            cnt++;
                            Controller.sqlRegKeys.Add(rk.Path,
                            new Reason(SourceType.Installer_ProductCode, rp.Name, comment, true));
                        }

                        if (!foundCompressedProductCodes.Contains(rp.Name))
                        {
                            foundCompressedProductCodes.Add(rp.Name);
                        }
                        if (!sumSQLProduct.productCodeToName.ContainsKey(rp.Name))
                            sumSQLProduct.productCodeToName.Add(rp.Name, comment);

                        found = true;
                    }


                    if (found) break;
                } //foreach

            }//forach
            Controller.UpdateProgress("Add_HKCR_UpgradeCodes keys:" + cnt, true);
            return cnt;
        }


        private static int Add_HKLM_UpgradeCodes()
        {
            int cnt = 0;

            /****************************************************************************************************/
            //HKLM
            /****************************************************************************************************/

            /****************************************************************************************************/
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UpgradeCodes  
            /****************************************************************************************************/
            string sub = "UpgradeCodes";
            Controller.UpdateProgress(@"Analyzing HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\" + sub + "...", true);
            //RegHive reghive = new RegHive(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\" + sub);

            RegHive reghive = Controller.regHives[@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UpgradeCodes"];

            //first, check upgrade code
            foreach (string key in reghive.subKeys)
            {

                string reg = String.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\" + sub + @"\{0}", key);
                //   Controller.UpdateProgress("Comparing " + reg);




                if (sumSQLProduct.compressedUpgradeCodes.Contains(key)
                    || foundCompressedUpradeCodes.Contains(key)
                    )
                {
                    string comment = "Upgrade Code:" + key;
                    if (sumSQLProduct.upgradeCodeToName.ContainsKey(key))
                        comment = sumSQLProduct.upgradeCodeToName[key];


                    if (!Controller.sqlRegKeys.ContainsKey(reg))
                    {

                        cnt++;
                        Controller.sqlRegKeys.Add(reg,
                        new Reason(SourceType.Installer_UpgradeCode, key, comment, true));

                    }
                    if (!foundCompressedUpradeCodes.Contains(key))
                    {
                        foundCompressedUpradeCodes.Add(key);
                    }
                    if (!sumSQLProduct.upgradeCodeToName.ContainsKey(key))
                        sumSQLProduct.upgradeCodeToName.Add(key, comment);


                    //Need to add product code under this key

                    RegKey rk = new RegKey(reg, "");
                    foreach (RegProperty rp in rk.RegProperties)
                    {
                        if (rp.Name.Length == 32)
                        {
                            if (!foundCompressedProductCodes.Contains(rp.Name))
                            {
                                foundCompressedProductCodes.Add(rp.Name);
                            }
                            if (!sumSQLProduct.productCodeToName.ContainsKey(rp.Name))
                                sumSQLProduct.productCodeToName.Add(rp.Name, comment);
                        }

                    }


                }


            }

            foreach (RegKey rk in reghive.regKeys)
            {
                bool found = false;
                foreach (RegProperty rp in rk.RegProperties)
                {

                    //If any name matches product code
                    if (foundCompressedProductCodes.Contains(rp.Name) ||
                        sumSQLProduct.compressedProductCodes.Contains(rp.Name))
                    {

                        string comment = "Product code:" + rp.Name;
                        if (sumSQLProduct.productCodeToName.ContainsKey(rp.Name))
                            comment = sumSQLProduct.productCodeToName[rp.Name];


                        if (!Controller.sqlRegKeys.ContainsKey(rk.Path))
                        {

                            cnt++;
                            Controller.sqlRegKeys.Add(rk.Path,
                            new Reason(SourceType.Installer_ProductCode, rp.Name, comment, true));
                        }

                        if (!foundCompressedProductCodes.Contains(rp.Name))
                        {
                            foundCompressedProductCodes.Add(rp.Name);
                         }
                          if (!sumSQLProduct.productCodeToName.ContainsKey(rp.Name))
                                sumSQLProduct.productCodeToName.Add(rp.Name, comment);
 
                        found = true;
                    }


                    if (found) break;
                } //foreach

            }//forach

            Controller.UpdateProgress("Add_HKLM_UpgradeCodes keys:" + cnt, true);
            return cnt;

        }



        public static void Add_HKLM_Components()
        {
            int cnt = 0;
            /****************************************************************************************************/
            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components  
            /****************************************************************************************************/

            string sub = "Components";
            Controller.UpdateProgress(@"Analyzing HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub + "...", true);
            //RegHive reghive = new RegHive(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\" + sub);

            RegHive reghive = Controller.regHives[@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components"];
            //first, check product code
            //using regKeys instead subkeys
            int k = 0;
            foreach (RegKey rk in reghive.regKeys)
            {
                bool found = false;
                string reg = rk.Path;

                if (k % 2000 == 0)
                    Controller.UpdateProgress("Checking " + reg);
                k++;


                foreach (RegProperty rp in rk.RegProperties)
                {
                    if (foundCompressedProductCodes.Contains(rp.Name)
                        || sumSQLProduct.compressedProductCodes.Contains(rp.Name)
                        )
                    {

                        string comment = "Product code:" + rp.Name;
                        if (sumSQLProduct.productCodeToName.ContainsKey(rp.Name)) comment = sumSQLProduct.productCodeToName[rp.Name];


                        if (!Controller.sqlRegKeys.ContainsKey(reg))
                        {

                            cnt++;
                            Controller.sqlRegKeys.Add(reg,
                            new Reason(SourceType.Installer_Component, rp.Name, comment, false));

                        }

                        if (!foundCompressedProductCodes.Contains(rp.Name))
                            foundCompressedProductCodes.Add(rp.Name);

                        found = true;
                    }
                    /*, don't want to check this, though it can add some
                    else
                    {

                        try
                        {
                            if (sumSQLProduct.files_EXE_DLL.Contains(Path.GetFileName(rp.Data.ToLower())))
                            {
                                if (!Controller.sqlRegKeys.ContainsKey(reg))
                                {

                                    cnt++;
                                    Controller.sqlRegKeys.Add(reg,
                                    new Reason(SourceType.Installer_Component, rp.Name, "Product Code:" + rp.Name,true));

                                }

                                if (!foundCompressedProductCodes.Contains(rp.Name))
                                    foundCompressedProductCodes.Add(rp.Name);
                                found = true;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    */

                    if (found) break;

                } //foreach

            } //foreach 



            Controller.UpdateProgress("Add_HKLM_Components keys:" + cnt, true);

          //  return cnt;
        }


    }
}




