using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using System.IO;

namespace FixMissingMSI
{
    public enum CacheFileStatus
    {

        OK,
        Mismatched,
        Missing,
        Empty,
        Fixed
    }
   
    public static class myData
    {

        //this is for the cross UI thread 
        public static SortableBindingList<myRow> rows = new SortableBindingList<myRow>();


        public static int Index = 0;
        public static List<MsiMspPackage> sourcePkgs = new List<MsiMspPackage>();
        public delegate void callbackProgressFunc(string info);
        public delegate void callbackDoneFunc();
        public static callbackProgressFunc UpdateUI = null;
        public static callbackDoneFunc DoneCallBack = null;
        public static callbackDoneFunc DoneCallBack_Last = null;

        public static string setupSource = "";
        public static string filterString = "SQL";
        public static bool isFilterOn = true;

        public static DataGridView dataGridV = null;
        public static void Init(DataGridView gdv, callbackProgressFunc fn, callbackDoneFunc done, callbackDoneFunc last)
        {
            gdv.ReadOnly = true;
            gdv.AllowUserToAddRows = false;
            gdv.AllowUserToDeleteRows = false;

            UpdateUI = fn;
            DoneCallBack = done;
            DoneCallBack_Last = last;
            dataGridV = gdv;

        }

        private static void SetDataSource(SortableBindingList<myRow> newRows)
        {

            BindingSource source = new BindingSource(newRows, null);
            dataGridV.DataSource = source;
        }

        public static void RemoveDataSource()
        {
            SortableBindingList<myRow> emptyRows = new SortableBindingList<myRow>();
            BindingSource source = new BindingSource(emptyRows, null);
            dataGridV.DataSource = source;
        }
        public static void SetRow()
        {

            SetDataSource(rows);
        }
        public static void AddRow(myRow r)
        {
            rows.Add(r);
        }
        /// <summary>
        /// show only those missing/mismatched rows
        /// </summary>
        public static void SetFilter(DataGridView gdv)
        {
            UpdateUI("Filtering result set...");
            SortableBindingList<myRow> filteredRows
                  = new SortableBindingList<myRow>(myData.rows.Where(r => r.Status != CacheFileStatus.OK).ToList());

            SetDataSource(filteredRows);

            UpdateUI("");
        }

        public static void RemoveFilter(DataGridView gdv)
        {
            UpdateUI("Display all results...may take minutes...");
            SetDataSource(rows);
            UpdateUI("");
        }

        private static void AddRow(ProductInstallation p)
        {
            myRow r = new myRow(p);
            rows.Add(r);
            Logger.LogMsg(r.ToString());

            try
            {

                IEnumerable<PatchInstallation> patches = PatchInstallation.GetPatches(null, p.ProductCode, null, UserContexts.All, PatchStates.All);



                foreach (PatchInstallation patch in patches)
                {
                    myRow patchRow = new myRow(patch);
                    rows.Add(patchRow);
                    Logger.LogMsg(patchRow.ToString());

                }
            }catch(Exception ex)
            {
                Logger.LogError("AddRow:"+ex.Message);
            }
        }
       

        public static void ScanSetupMedia()
        {
            String setupSource = myData.setupSource;
            sourcePkgs.Clear();
            sourcePkgs= MsiMspPackage.ScanSetupMedia(setupSource);
        }

        public static void ScanProducts()
        {
            
            //clear the row first
            rows.Clear();
            myData.Index = 1;
            Logger.LogMsg("Scan Installed Products/Patches based on MSI/MSP from setup source:\"" + setupSource + "\"");

            UpdateUI("GetInstalledProducts...");
            var ps = MSIHelper.GetInstalledProducts();

            

            int k = 0;
            int t = ps.Count();
            foreach (ProductInstallation p in ps)
            {
                UpdateUI("Checking (" + (++k).ToString() + "/" + t.ToString() + ") " + p.ProductName);
                try
                {
                    if (isFilterOn && filterString!="")
                    {
                        if (!String.IsNullOrEmpty(p.ProductName))
                        {
                            if (p.ProductName.ToLower().Contains(filterString.ToLower()))
                            {
                                AddRow(p);
                                UpdateUI(p.ProductName + " added.");
                            }

                        } // if (!String.IsNullOrEmpty(p.ProductName))
                    }//filtered on
                    else //otherwise add every product
                    {
                        AddRow(p);
                        UpdateUI(p.ProductName + " added.");
                    }

                }
                catch (Exception e)
                {
                    Logger.LogError("ScanWithSQLSetupSource:ProductCode" +p.ProductCode +" "+ e.Message  );
                }




            }

            Logger.LogMsg("Installed Products/Patches scan found: " + rows.Count + " items.");
            DoneCallBack();

        } //ScanWithoutSQLSetupSource

        public static void AfterDone()
        {
            if(setupSource!="") ScanSetupMedia();
            AddMsiMspPackageFromLastUsedSource();
            UpdateFixCommand();

            DoneCallBack_Last();
        }
        private static void AddMsiMspPackageFromLastUsedSource()
        {
            Logger.LogMsg("Add msi/msp packages from LastUsedSource...");
            UpdateUI("Add msi/msp packages from LastUsedSource,may take minutes...");
            int i = 0; int j = 0;
            foreach (myRow r in rows)
            {
                try
                {
                    if (string.IsNullOrEmpty(r.LastUsedSource)) continue;
                    j++;
                    string path = Path.Combine(r.LastUsedSource, r.PackageName);
                    UpdateUI("Checking ("+j+") " + path);
                    if (File.Exists(path))
                    {
                        Logger.LogMsg(j + " [Found]" + path);
                        MsiMspPackage pkg = new MsiMspPackage(path);
                        if (!pkg.failedPackage)
                            sourcePkgs.Add(new MsiMspPackage(path));
                        i++;
                    }
                    else
                    {
                        Logger.LogMsg(j + " [Not Found]" + path);
                    }
                }catch(Exception ex)
                {
                    Logger.LogError("AddMsiMspPackageFromLastUsedSource:" + ex.ToString());
                }

            }

            Logger.LogMsg("Total " + i + " packages from LastUsedSource added.");
            UpdateUI("Total " + i + " packages from LastUsedSource added.");


        }

        public static string FindMsi(string productName, String pkgName, string pcode, string version,string pkgCode)
        {

            foreach (MsiMspPackage pkg in sourcePkgs)
            {
                if (//pkg.ProductCode == pcode --Cannot compare product code since transform could be applied, which means product code installed is not the same as the one in msi package
                     pkg.MsiMspFileName.ToUpper() == pkgName.ToUpper()
                    //cannot compare version. for example, for sp1, its version is changed, different from RTM version
                    // && pkg.ProductVersion == version
                    //cannot compare product name as well. sp1/sp2 could change it.
                   // && pkg.ProductName == productName
                   //Its package code should be matched
                   && pkg.PackageCode== pkgCode
                    )
                {
                    Logger.LogMsg("[Found missing MSI]" + pkg.FullPath);
                    UpdateUI("[Found missing MSI]" + pkg.FullPath);
                    return pkg.FullPath;

                }

            }
            Logger.LogMsg("[Missing MSI not found]" + pkgName);
            UpdateUI("[Missing MSI not found]" + pkgName);
            return null;

        }

       


        public static string FindMsp(string displayName, String pkgName,string patchCode)
        {

            foreach (MsiMspPackage pkg in sourcePkgs)
            {
                if (pkg.isMsp
                  //  && pkg.ProductName == displayName //remark this, use patch code and package name is good enough.
                    && pkg.MsiMspFileName.ToUpper() == pkgName.ToUpper()
                    && pkg.PatchCode==patchCode
                    )

                {
                    Logger.LogMsg("[Missing MSP found]" + pkg.FullPath);
                    UpdateUI("[Missing MSP found]" + pkg.FullPath);
                    return pkg.FullPath;
                     
                }



            }
            Logger.LogMsg("[Missing MSP not found]" + pkgName);
            return null;

        }



        public static void UpdateFixCommand()
        {
            BindingList<myRow> rws = rows;
            Logger.LogMsg("UpdateFixCommand...");
            UpdateUI("UpdateFixCommand...");
            int cnt = 0;
            foreach (myRow r in rws)
            {
                UpdateUI("UpdateFixCommand "+cnt);cnt++;
                if (r.Status == CacheFileStatus.Missing || r.Status == CacheFileStatus.Mismatched)
                {
                    if (r.isPatch)
                    {
                        var matchedFile = myData.FindMsp(r.ProductName, r.PackageName,r.PatchCode);
                        if (!String.IsNullOrEmpty(matchedFile))
                        {
                            r.FixCommand = "COPY \"" + matchedFile + "\" \"C:\\WINDOWS\\INSTALLER\\" + r.CachedMsiMsp + "\"";
                            Logger.LogMsg("[FixCommand]" + r.FixCommand);
                        }
                    }
                    else
                    {
                        var matchedFile = myData.FindMsi(r.ProductName, r.PackageName, r.ProductCode, r.ProductVersion,r.PackageCode);
                        if (!String.IsNullOrEmpty(matchedFile))
                        {
                            r.FixCommand = "COPY \"" + matchedFile + "\" \"C:\\WINDOWS\\INSTALLER\\" + r.CachedMsiMsp + "\"";
                            Logger.LogMsg("[FixCommand]" + r.FixCommand);
                        }
                    }
                }
            }

            UpdateUI("UpdateFixCommand done.");
            Logger.LogMsg("UpdateFixCommand done.");
        }



    }//public class myDAta
















}
