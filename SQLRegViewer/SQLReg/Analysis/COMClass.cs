using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32;

namespace SQLReg.Analysis
{
   public class COMClass
    {

        public static void Add()
        {
            var sumSQLProduct = Controller.sumSQLProduct;

            HashSet<string> foundCLSID = new HashSet<string>();
            HashSet<string> foundTypeLib = new HashSet<string>();
            HashSet<string> foundProgID = new HashSet<string>();

            string[] hives = new string[] { "HKEY_CLASSES_ROOT", "HKEY_CLASSES_ROOT\\Wow6432Node" };

            foreach (string hiveRoot in hives)
            {

                /*******************************************************************/
                /*
                 HKEY_CLASSES_ROOT\CLSID
                 */
                /*******************************************************************/
                int cnt = 0;
                subKey sbk = new subKey("InProcServer32", "");
                Controller.UpdateProgress(@"Scanning " + hiveRoot,true);
                
                //RegHive reghive = new RegHive(hiveRoot + @"\CLSID", sbk);
                RegHive reghive = Controller.regHives[hiveRoot+@"\CLSID"];


                Logger.LogMsg("reghive.propertyMap:" + reghive.propertyMap.Count);

                foreach (KeyValuePair<string, List<string>> kv in reghive.propertyMap)
                {
                    //Ignore those like:C:\Program Files\Common Files\Microsoft Shared\VS7Debug\pdm.dll
                    if (kv.Key.ToLower().Contains("microsoft shared")) continue;
                 //   foreach (SQLProduct sql in sqlProducts)
                    {
                        try
                        {
                            if (sumSQLProduct.files_EXE_DLL.Contains(Path.GetFileName(kv.Key).ToLower()))
                            {
                                foreach (string path in kv.Value)
                                {
                                    if (!Controller.sqlRegKeys.ContainsKey(path))
                                    {
                                        cnt++;
                                        Controller.sqlRegKeys.Add(path, new Reason(SourceType.FromMSI_File, kv.Key));
                                   
                                    }

                                    //HKEY_CLASSES_ROOT\CLSID\{FE2D84B0-BBD5-4EC6-9A74-EE1C55148D78}\TypeLib
                                    string typeLibKey = path + "\\TypeLib";
                                    if (RegHelper.IsKeyExist(typeLibKey))
                                    {
                                        RegKey rk = new RegKey(typeLibKey, "");
                                        if (rk.Properties.ContainsKey("(Default)"))
                                        {
                                            if (!foundTypeLib.Contains(rk.Properties["(Default)"][1].ToString()))
                                                foundTypeLib.Add(rk.Properties["(Default)"][1].ToString());
                                        }
                                    }

                                    //HKEY_CLASSES_ROOT\CLSID\{02E7E69E-E80A-48E3-8B1D-6448C25B1710}\ProgID
                                    string progIDKey = path + "\\ProgID";
                                    if (RegHelper.IsKeyExist(progIDKey))
                                    {
                                        RegKey rk = new RegKey(progIDKey, "");
                                        if (rk.Properties.ContainsKey("(Default)"))
                                        {
                                            if (!foundProgID.Contains(rk.Properties["(Default)"][1].ToString()))
                                                foundProgID.Add(rk.Properties["(Default)"][1].ToString());
                                        }


                                    }

                                    int idx = path.IndexOf("CLSID\\") + 6;
                                    string CLSID = path.Substring(idx, 38);

                                    if (!foundCLSID.Contains(CLSID)) foundCLSID.Add(CLSID);


                                }
                               
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex.Message);
                        }
                      
                    } //foreach SQL products
                }

                Controller.UpdateProgress(reghive.hivePath+" Add keys:" + cnt, true);

                /*******************************************************************/
                /*
                 HKEY_CLASSES_ROOT\TypeLib
                 */
                /*******************************************************************/
                cnt = 0;
                Controller.UpdateProgress(@"Scanning " + hiveRoot,true);
                //reghive = new RegHive(hiveRoot + @"\TypeLib");
                 reghive = Controller.regHives[hiveRoot + @"\TypeLib"];

                string[] winxx = new string[] { "win32", "win64" };
                foreach (RegKey rk in reghive.regKeys)
                {
                    foreach (string win in winxx)
                    {
                        string fullpath = RegHelper.FindSubKey(rk.Path, win, 4);
                        if (!string.IsNullOrEmpty(fullpath))
                        {
                            //we find one

                            RegKey subrk = new RegKey(fullpath, "");
                            if (subrk.Properties.ContainsKey("(Default)"))
                            {
                                try
                                {
                                    string file = subrk.Properties["(Default)"][1].ToLower();
                                    string name = Path.GetFileName(file);

                                    if (file.Contains("microsoft shared")) continue;
                                        if (sumSQLProduct.files_EXE_DLL.Contains(name))
                                        {
                                            if (!Controller.sqlRegKeys.ContainsKey(rk.Path))
                                            {
                                                Controller.sqlRegKeys.Add(rk.Path,new Reason(SourceType.FromMSI_File, file));
                                                cnt++;
                                           
                                        }
                                            if (!foundTypeLib.Contains(rk.leaf)) foundTypeLib.Add(rk.leaf);

                                        }
                                 

                                }
                                catch (Exception ex) { }
                            }
                        }//if
                    }//foreach winxx
                }//foreach

                Controller.UpdateProgress(reghive.hivePath + " Add keys:" + cnt, true);

                //now check ProdID
                //HKEY_CLASSES_ROOT\DTS.ConnectionManagerOlap.5\CLSID

                RegistryKey hkcr = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
                string[] allnames = hkcr.GetSubKeyNames();
                  cnt = 0;
                foreach (string s in allnames)
                {
                    string sub = hiveRoot + "\\" + s;
                    RegistryKey subkey = hkcr.OpenSubKey(s);

                    if (subkey == null) continue;
                    string[] subnames = subkey.GetSubKeyNames();
                    if (subnames.Contains("CLSID") || subnames.Contains("Clsid"))
                    {
                        string result = RegHelper.GetValueForProperty(sub + "\\CLSID", "");
                        if (foundCLSID.Contains(result))
                        {
                         
                            //we find one
                            if (!Controller.sqlRegKeys.ContainsKey(sub))
                            {
                                Controller.sqlRegKeys.Add(sub,new Reason(SourceType.COMClass,result,"CLSID:"+result));
                               cnt++;
                            }
                        }
                    }

                }

                Controller.UpdateProgress("HKEY_CLASSES_ROOT\\ProdID Add keys:" + cnt, true);

                //Add HKCR\ProdID
                cnt = 0;
                
                foreach (string s in foundProgID)
                {
                    string path = hiveRoot + "\\" + s;
                    if (Controller.sqlRegKeys.ContainsKey(path)) continue;
                    else
                    {
                        if (RegHelper.IsKeyExist(path))
                        {
                            cnt++; Controller.sqlRegKeys.Add(path,new Reason(SourceType.COMClass,s, "ProdID:" + s));
                            
                          
                        }
                    }

                }

                Controller.UpdateProgress("HKEY_CLASSES_ROOT\\ProdID (from foundProgID) Add keys:" + cnt, true);

                //for HKEY_CLASSES_ROOT\AppID I don't know how to add it here

                //
                /********************************************************/
                //HKEY_CLASSES_ROOT\Interface
                /********************************************************/
                //HKEY_CLASSES_ROOT\Interface\{33d00d41-c94f-5a61-9ab7-280dcefa0b08}\ProxyStubClsid32

                //below could have less hits...
                Controller.UpdateProgress(@"Scanning " + hiveRoot,true);
                //  reghive = new RegHive(hiveRoot + @"\Interface");

                reghive = Controller.regHives[hiveRoot + @"\Interface"];
                cnt = 0;

                foreach (string sub in reghive.subKeys)
                {

                    try
                    {
                        string path = hiveRoot + "\\Interface\\" + sub;
                        if (!RegHelper.IsKeyExist(path + "\\ProxyStubClsid32")) continue;
                        RegKey rk = new RegKey(path + "\\ProxyStubClsid32", "");
                        if (rk.Properties.ContainsKey("(Default)"))
                        {
                            string clsid = rk.Properties["(Default)"][1];
                            if (foundCLSID.Contains(clsid))
                            {
                            
                                if (!Controller.sqlRegKeys.ContainsKey(path))
                                {    cnt++;
                                    Controller.sqlRegKeys.Add(path,new Reason(SourceType.COMClass,clsid,"CLSID:"+clsid));
                                    
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.Message);
                    }

                }

                Controller.UpdateProgress(reghive.hivePath+" Add keys:" + cnt, true);

            }// for hiveRoot











        }
    }
}
