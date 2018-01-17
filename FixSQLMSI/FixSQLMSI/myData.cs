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

namespace FixSQLMSI
{
    public enum CacheFileStatus
    {

        OK,
        Mismatched,
        Missing,
        Empty,
        Fixed
    }
    public class myRow
    {
        public int Index { get; set; }
        public CacheFileStatus Status { get; set; }
        public String PackageName { get; set; }
        public String CachedMsiMsp { get; set; }
        public String CachedMsiMspVersion { get; set; }
        public String ProductVersion { get; set; }
        public String PatchBaselineVersion { get; set; }
        public String ProductName { get; set; }
        public String ProductCode { get; set; }
        public string PackageCode { get; set; } //for MSI
        public string PatchCode { get; set; } //For msp

        public string LastUsedSource { get; set; }
        public String IsAdvertised { get; set; }

        public String Publisher { get; set; }

        //  private Color RowColor { get; set; }
        public DateTime InstallDate { get; set; }
        // public String InstallLocation { get; set; }
        public String InstallSource { get; set; }

        //for  patch

        //   public String PatchVersion { get; set; }

        public String MoreInfoURL { get; set; }
        public bool isPatch = false;

        public string FixCommand { get; set; }
        public string Comment { get; set; }
        /// <summary>
        /// Add row for MSI package
        /// </summary>
        /// <param name="p"></param>
        public myRow(ProductInstallation p)
        {

           // if(!string.IsNullOrEmpty(p.ProductName) && )
            this.Index = myData.Index++;
            try
            {

                this.ProductVersion = p.ProductVersion.ToString();
                if (!string.IsNullOrEmpty(p.AdvertisedPackageCode)) this.PackageCode = p.AdvertisedPackageCode;
                CacheFileStatus stat = CacheFileStatus.Missing;
                if (!String.IsNullOrEmpty(p.LocalPackage))
                {
                    CachedMsiMsp = Path.GetFileName(p.LocalPackage);



                    if (File.Exists(p.LocalPackage))
                    {
                        try
                        {

                            String ver = MSIHelper.Get(p.LocalPackage, "ProductVersion");
                            this.CachedMsiMspVersion = ver;
                            string cachedPackageCode = MSIHelper.GetRevisionNumber(p.LocalPackage);
                            if(!string.IsNullOrEmpty(cachedPackageCode) && !string.IsNullOrEmpty(this.PackageCode))
                            {
                                if(string.Compare(this.PackageCode,cachedPackageCode,StringComparison.Ordinal)!=0)
                                {
                                    stat = CacheFileStatus.Mismatched;
                                    this.Comment="Package code doesn't matched. cached file has package code:"+cachedPackageCode+", but Installer expected:"+this.PackageCode;

                                }

                            }
                            //if (!String.IsNullOrEmpty(this.ProductVersion))
                            //{
                            //    if (ver != this.ProductVersion)
                            //    {
                            //       //Cannot set  it here,  if sp1 applied then production version is sp1 vesion, but cached msi is still RTM version
                            //        // stat = CacheFileStatus.Mismatched;
                            //       // Comment = p.LocalPackage + ": ProductVersion not matched";
                            //    }
                            //}
                            /*
                            //Cannot compare product name, sp1/sp2 will change product name...
                            if (stat != CacheFileStatus.Mismatched)
                            {
                                //Cannot compare product name, sp1/sp2 will change product name...
                                String pn = MSIHelper.Get(p.LocalPackage, "ProductName");
                                if (p.ProductName != pn)
                                {
                                    stat = CacheFileStatus.Mismatched;
                                    Comment = p.LocalPackage + ": ProductName not matched! [" + pn + "] vs [" + p.ProductName + "]";
                                }
                            }
                            */

                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("[Mismatched MSI check failed]" + p.LocalPackage + " \n" + ex.Message);
                        }

                        if (stat != CacheFileStatus.Mismatched) stat = CacheFileStatus.OK;
                    }
                    else
                    {
                        stat = CacheFileStatus.Missing;
                        Comment = p.LocalPackage + " doesn't exist.";

                    }
                }
                else stat = CacheFileStatus.Empty;

                this.Status = stat;

                if (p.SourceList != null)
                {
                    this.PackageName = p.SourceList.PackageName;
                    this.LastUsedSource = p.SourceList.LastUsedSource;
                }
                this.InstallDate = p.InstallDate;
                //  this.InstallLocation = p.InstallLocation;
                this.InstallSource = p.InstallSource;
                this.IsAdvertised = p.IsAdvertised.ToString();
                this.ProductCode = p.ProductCode;
                this.ProductName = p.ProductName;

                this.Publisher = p.Publisher;

                

            }
            catch (Exception e)
            {
                Logger.LogError("myRow:ProductInstallation:" + e.Message + ":" + e.HResult.ToString());
            }

        }

        /// <summary>
        /// Add row for MSP package
        /// </summary>
        /// <param name="p"></param>
        public myRow(PatchInstallation p)
        {
            this.Index = myData.Index++;

            try
            {

                if (!string.IsNullOrEmpty(p.PatchCode)) this.PatchCode = p.PatchCode;

                CacheFileStatus stat = CacheFileStatus.Missing;
                if (!String.IsNullOrEmpty(p.LocalPackage))
                {
                    if (File.Exists(p.LocalPackage))
                    {
                        try
                        {

                            string cachePatchCode = MSIHelper.GetRevisionNumber(p.LocalPackage);
                            if (!string.IsNullOrEmpty(cachePatchCode) && !string.IsNullOrEmpty(this.PatchCode))
                            {
                                if (string.Compare(this.PatchCode, cachePatchCode, StringComparison.Ordinal) != 0)
                                {
                                    stat = CacheFileStatus.Mismatched;
                                    this.Comment = "Patch code doesn't matched. cached file has package code:" + cachePatchCode + ", but Installer expected:" + this.PatchCode;

                                }

                            }
                            //String ver = MSIHelper.MspGetMetadata(p.LocalPackage, "BaselineVersion");
                            //if (!String.IsNullOrEmpty(this.ProductVersion))
                            //{
                            //    if (ver != this.ProductVersion)
                            //    {
                            //        stat = CacheFileStatus.Mismatched;
                            //        Comment = p.LocalPackage + ": BaselineVersion in MSP not matched the ProductVersion in its MSI. If SP applied this could be normal.";
                            //    }
                            //}
                            //add one more check. displayname should match
                            if (stat != CacheFileStatus.Mismatched)
                            {
                                String pn = MSIHelper.MspGetMetadata(p.LocalPackage, "DisplayName");
                                if (p.DisplayName != pn)
                                {
                                    stat = CacheFileStatus.Mismatched;
                                    Comment = p.LocalPackage + ": DisplayName not matched! Cached file has DisplayName:[" + pn + "] but Installer expected: [" + p.DisplayName + "]";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError("[Mismatched MSP check failed]" + p.LocalPackage + " \n" + ex.Message);
                        }

                        if (stat != CacheFileStatus.Mismatched) stat = CacheFileStatus.OK;
                    }
                    else
                    {
                        stat = CacheFileStatus.Missing;
                        Comment = p.LocalPackage + " doesn't exist";
                    }
                }
                else stat = CacheFileStatus.Empty;

                this.Status = stat;

                if (p.SourceList != null)
                {
                    this.PackageName = p.SourceList.PackageName;
                    this.LastUsedSource = p.SourceList.LastUsedSource;
                }
                this.InstallDate = p.InstallDate;

                //patch doesn't have source
                //this.InstallSource = p.InstallSource;
                //this.IsAdvertised = p.IsAdvertised.ToString();
                this.ProductCode = p.ProductCode;
                this.ProductName = p.DisplayName;
                //patch won't have product version
                // this.ProductVersion = p.ProductVersion.ToString();
                //  this.Publisher = p.Publisher;
                this.isPatch = true;

                if (!String.IsNullOrEmpty(p.LocalPackage))
                {
                    CachedMsiMsp = Path.GetFileName(p.LocalPackage);
                    if (File.Exists(p.LocalPackage))
                    {
                        //    CachedMsiMspVersion = MSIHelper.Get(p.LocalPackage, "ProductVersion").ToString();


                        using (var db = new QDatabase(p.LocalPackage, DatabaseOpenMode.ReadOnly))
                        {
                            //using try catch since some msp doesn't all this properties
                            try { this.CachedMsiMspVersion = db.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", "PatchVersion") as string; } catch { }
                            try { this.PatchBaselineVersion = db.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", "BaselineVersion") as string; } catch { };
                            try { this.MoreInfoURL = db.ExecuteScalar("SELECT `Value` FROM `MsiPatchMetadata` WHERE `Property` = '{0}'", "MoreInfoURL") as string; } catch { };



                        }
                    }
                }





            }
            catch (Exception e)
            {
                Logger.LogError("myRow:patch:" + e.Message + ":" + e.HResult.ToString());
            }

        }
        public override string ToString()
        {
            string s = "";

            s = "\"" + this.Index.ToString() + "\"";

            s = s + "\"" + this.Status.ToString() + "\"";
            if (!String.IsNullOrEmpty(this.PackageName)) s = s + ",\"" + this.PackageName.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.CachedMsiMsp)) s = s + ",\"" + this.CachedMsiMsp.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.CachedMsiMspVersion)) s = s + ",\"" + this.CachedMsiMspVersion.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.ProductVersion)) s = s + ",\"" + this.ProductVersion.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.ProductName)) s = s + ",\"" + this.ProductName.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.ProductCode)) s = s + ",\"" + this.ProductCode.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.LastUsedSource)) s = s + ",\"" + this.LastUsedSource.ToString() + "\"";
            else s = s + ",\"\"";


            if (!String.IsNullOrEmpty(this.IsAdvertised)) s = s + ",\"" + this.IsAdvertised.ToString() + "\"";
            else s = s + ",\"\"";


            if (!String.IsNullOrEmpty(this.Publisher)) s = s + ",\"" + this.Publisher.ToString() + "\"";
            else s = s + ",\"\"";

            if (this.InstallDate != null) s = s + ",\"" + this.InstallDate.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.InstallSource)) s = s + ",\"" + this.InstallSource.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.PatchBaselineVersion)) s = s + ",\"" + this.PatchBaselineVersion.ToString() + "\"";
            else s = s + ",\"\"";

            if (!String.IsNullOrEmpty(this.MoreInfoURL)) s = s + ",\"" + this.MoreInfoURL.ToString() + "\"";
            else s = s + ",\"\"";

            return s;
        }


        public myRow(CacheFileStatus status, string nm, string ver, string pn)
        {
            Status = status;
            PackageName = nm;
            ProductVersion = ver;
            ProductName = pn;
            //RowColor = color;

        }

        public myRow()
        {

        }
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
        public static string setupSource = "";
        public static DataGridView dataGridV = null;
        public static void Init(DataGridView gdv, callbackProgressFunc fn, callbackDoneFunc done)
        {
            gdv.ReadOnly = true;
            gdv.AllowUserToAddRows = false;
            gdv.AllowUserToDeleteRows = false;

            UpdateUI = fn;
            DoneCallBack = done;
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
                Logger.LogError(ex.Message);
            }
        }
        /// <summary>
        /// REturn a list of myRow which ProductName has key word of "SQL"
        /// </summary>
        public static void ScanWithoutSQLSetupSource()
        {
            //clear the row first
            rows.Clear();
            sourcePkgs.Clear();
            myData.Index = 1;

            Logger.LogMsg("Scan Installed Products to find out those where ProductName like '%SQL%'...");

            UpdateUI("GetInstalledProducts...");
            var ps = MSIHelper.GetInstalledProducts();


            int k = 0;
            int t = ps.Count();
            foreach (ProductInstallation p in ps)
            {
                UpdateUI("Checking (" + (++k).ToString() + "/" + t.ToString() + ") " + p.ProductName);
                try
                {
                    if (!String.IsNullOrEmpty(p.ProductName))
                        if (p.ProductName.ToUpper().Contains("SQL"))
                        {
                            AddRow(p);
                            UpdateUI(p.ProductName + " added.");

                        }


                }
                catch (Exception e)
                {
                    Logger.LogError("ScanWithoutSQLSetupSource:" + e.Message + ":" + e.HResult.ToString());
                }




            }

            Logger.LogMsg("Installed Products/Patches scan found: " + rows.Count + " items.");
            AddMsiMspPackageFromLastUsedSource();

            UpdateFixCommand(rows);

            DoneCallBack();

        } //ScanWithoutSQLSetupSource


        public static void ScanWithSQLSetupSource()
        {
            String setupSource = myData.setupSource;
            //clear the row first
            rows.Clear();
            sourcePkgs.Clear();
            myData.Index = 1;

            Logger.LogMsg("Scan Installed Products/Patches based on MSI/MSP from setup source:\"" + setupSource + "\"");

            UpdateUI("GetInstalledProducts...");
            var ps = MSIHelper.GetInstalledProducts();

            List<MsiMspPackage> pkgs = MsiMspPackage.ScanSetupMedia(setupSource);
            //save it for later use

            sourcePkgs = pkgs;

            //buid hash list to speed up compare speed
            HashSet<string> msiPNames = new HashSet<string>();
            HashSet<string> pkgNames = new HashSet<string>();

            foreach (MsiMspPackage kg in pkgs)
            {
                if (!kg.isMsp && !String.IsNullOrEmpty(kg.ProductName))
                {
                    if (!msiPNames.Contains(kg.ProductName)) msiPNames.Add(kg.ProductName);
                }
                if (!kg.isMsp)
                {
                    if (!pkgNames.Contains(kg.MsiMspFileName))
                    {
                        pkgNames.Add(kg.MsiMspFileName);
                    }
                }
            }

            int k = 0;
            int t = ps.Count();
            foreach (ProductInstallation p in ps)
            {
                UpdateUI("Checking (" + (++k).ToString() + "/" + t.ToString() + ") " + p.ProductName);
                try
                {

                    if (!String.IsNullOrEmpty(p.ProductName))
                    {
                        if (p.ProductName.ToUpper().Contains("SQL"))
                        {
                            AddRow(p);

                        }

                        else
                        {
                            if (msiPNames.Contains(p.ProductName)) AddRow(p);
                            else if ( p.SourceList != null && !String.IsNullOrEmpty(p.SourceList.PackageName))
                                if (pkgNames.Contains(p.SourceList.PackageName))
                                {
                                    AddRow(p);
                                }

                            
                                 /* below foreach is very slow. use use above hashset compare which is far faster.
                            foreach (MsiMspPackage pkg in pkgs)
                            {
                                //if transform has been applied, the product code in the package may not be the same as product installed.
                                //so use displlay name instead
                                //  if (pkg.ProductCode == p.ProductCode && !pkg.isMsp)

                                //we just add msi because for patch will be added in AddRow

                                if (!pkg.isMsp && pkg.ProductName==p.ProductName)
                                {
                                    AddRow(p);

                                    UpdateUI(p.ProductName + " added.");
                                    break;
                                }

                               
                                if (!pkg.isMsp && p.SourceList!=null)
                                {
                                    if (p.SourceList.PackageName == pkg.MsiMspFileName)
                                    {
                                        AddRow(p);

                                        UpdateUI(p.ProductName + " added.");
                                        break;
                                    }
                                }

                           
                            } //foreach
                            */

                        } //else if it doens't have "SQL"


                    } // if (!String.IsNullOrEmpty(p.ProductName))

                }
                catch (Exception e)
                {
                    Logger.LogError("ScanWithoutSQLSetupSource:" + e.Message + ":" + e.HResult.ToString("X8"));
                }




            }

            Logger.LogMsg("Installed Products/Patches scan found: " + rows.Count + " items.");


            AddMsiMspPackageFromLastUsedSource();

            UpdateFixCommand(rows);

            DoneCallBack();

        } //ScanWithoutSQLSetupSource

        private static void AddMsiMspPackageFromLastUsedSource()
        {
            Logger.LogMsg("Add msi/msp packages from LastUsedSource...");
            UpdateUI("Add msi/msp packages from LastUsedSource...");
            int i = 0; int j = 0;
            foreach (myRow r in rows)
            {
               
                j++;
                string path = Path.Combine(r.LastUsedSource, r.PackageName);
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


            }

            Logger.LogMsg("Total " + i + " packages from LastUsedSource added.");
            UpdateUI("Total " + i + " packages from LastUsedSource added.");


        }

        public static string FindMsi(string productName, String pkgName, string pcode, string version,string pkgCode)
        {

            foreach (MsiMspPackage pkg in sourcePkgs)
            {
                if (//pkg.ProductCode == pcode --Cannot compare product code since transform could be applied, which means product code installed is not the same as the one in msi package
                     pkg.MsiMspFileName == pkgName
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

        private static bool IsBaseLineMSIExist(MsiMspPackage pkg)
        {
            foreach (MsiMspPackage p in sourcePkgs)
            {
                if (!p.isMsp &&
                    p.MsiMspFileName == pkg.MsiMspFileName &&
                    p.ProductVersion == pkg.BaselineVersion)
                {
                    Logger.LogMsg("[Baseline MSI found]" + p.FullPath);
                    UpdateUI("[Baseline MSI found]" + p.FullPath);
                    return true;

                }

            }
            Logger.LogMsg("[Baseline MSI not found]" + pkg.MsiMspFileName);
            UpdateUI("[Baseline MSI not found]" + pkg.MsiMspFileName);
            return false;
        }


        public static string FindMsp(string displayName, String pkgName,string patchCode)
        {

            foreach (MsiMspPackage pkg in sourcePkgs)
            {
                if (pkg.isMsp
                    && pkg.ProductName == displayName
                    && pkg.MsiMspFileName == pkgName
                    && pkg.PatchCode==patchCode
                    )

                {
                    Logger.LogMsg("[Missing MSP found]" + pkg.FullPath);
                    UpdateUI("[Missing MSP found]" + pkg.FullPath);
                    return pkg.FullPath;
                    /* Should I check baseline MSI?
                    //check to see if baseline MSI exists  or not
                    Logger.LogMsg("Check Baseline MSI whether it exists...");
                    if (IsBaseLineMSIExist(pkg))
                        return pkg.FullPath;
                        */

                }



            }
            Logger.LogMsg("[Missing MSP not found]" + pkgName);
            return null;

        }



        public static void UpdateFixCommand(BindingList<myRow> rws)
        {
            Logger.LogMsg("UpdateFixCommand...");
            UpdateUI("UpdateFixCommand...");
            foreach (myRow r in rws)
            {
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

        }



    }//public class myDAta
















}
