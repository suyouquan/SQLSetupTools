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
                            if (!string.IsNullOrEmpty(cachedPackageCode) && !string.IsNullOrEmpty(this.PackageCode))
                            {
                                if (string.Compare(this.PackageCode, cachedPackageCode, StringComparison.Ordinal) != 0)
                                {
                                    stat = CacheFileStatus.Mismatched;
                                    this.Comment = "Package code doesn't matched. cached file has package code:" + cachedPackageCode + ", but Installer expected:" + this.PackageCode;

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
                Logger.LogError("myRow:ProductInstallation:" + e.Message);
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
                            //don't check this. patch code is good enough to check mismatched msp file.
                            /*
                            if (stat != CacheFileStatus.Mismatched)
                            {
                                String pn = MSIHelper.MspGetMetadata(p.LocalPackage, "DisplayName");
                                if (p.DisplayName != pn)
                                {
                                    stat = CacheFileStatus.Mismatched;
                                    Comment = p.LocalPackage + ": DisplayName not matched! Cached file has DisplayName:[" + pn + "] but Installer expected: [" + p.DisplayName + "]";
                                }
                            }
                            */
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
                Logger.LogError("myRow:patch:" + e.Message);
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

}
