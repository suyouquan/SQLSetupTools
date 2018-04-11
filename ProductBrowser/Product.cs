using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using Microsoft.Win32;
using System.Windows.Forms;


namespace ProductBrowser
{
    public static class Product
    {

        public delegate void callbackProgressFunc(string info);
        public delegate void callbackDoneFunc();
        private static callbackProgressFunc UpdateUI = null;
        public static callbackDoneFunc DoneCallBack = null;



        public static TreeNode root;
        public static string filterString = "SQL";
        public static bool isFilterOn = false;
        public static void Init(callbackProgressFunc fn, callbackDoneFunc done)
        {
            string computer = System.Environment.MachineName;
            root = new TreeNode(computer);


            UpdateUI = fn;
            DoneCallBack = done;
        }
        public static void UpdateProgress(string msg, bool WriteToLogFile = false)
        {
            UpdateUI(msg);
            if (WriteToLogFile) Logger.LogMsg(msg);
        }

        public static void Execute()
        {
            root.Nodes.Clear();
            GetAllProducts();
            DoneCallBack();
        }

        public static void AddProductProperty(TreeNode node, ProductInstallation p)
        {
            TreeNode nd = new TreeNode("ProductName : " + p.ProductName);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;
            nd.Name = nd.Text;
            node.Nodes.Add(nd);

            nd = new TreeNode("ProductVersion : " + p.ProductVersion);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);


            nd = new TreeNode("AdvertisedLanguage : " + p.AdvertisedLanguage);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("AdvertisedPackageCode : " + p.AdvertisedPackageCode);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd.Name = nd.Text;
            node.Nodes.Add(nd);

            nd = new TreeNode("AdvertisedPackageName : " + p.AdvertisedPackageName);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("AdvertisedPerMachine : " + p.AdvertisedPerMachine);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("AdvertisedProductName : " + p.AdvertisedProductName);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("AdvertisedTransforms : " + p.AdvertisedTransforms);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("AdvertisedVersion : " + p.AdvertisedVersion);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("Context : " + p.Context);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("InstallLocation : " + p.InstallLocation);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("InstallSource : " + p.InstallSource);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("IsAdvertised : " + p.IsAdvertised);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("IsElevated : " + p.IsElevated);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("IsInstalled : " + p.IsInstalled);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("LocalPackage : " + p.LocalPackage);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd = new TreeNode("ProductCode : " + p.ProductCode);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;


            nd = new TreeNode("ProductId : " + p.ProductId);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;


            nd = new TreeNode("Publisher : " + p.Publisher);
            nd.Name = nd.Text;
            node.Nodes.Add(nd);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            if (p.SourceList != null)
            {
                nd = new TreeNode("SourceList");
                nd.Name = nd.Text;
                node.Nodes.Add(nd);

                TreeNode ndkid = new TreeNode("PackageName : " + p.SourceList.PackageName);
                ndkid.Name = ndkid.Text;
                ndkid.ImageIndex = 1;
                ndkid.SelectedImageIndex = 1;

                nd.Nodes.Add(ndkid);

                ndkid = new TreeNode("LastUsedSource : " + p.SourceList.LastUsedSource);
                ndkid.Name = ndkid.Text;
                ndkid.ImageIndex = 1;
                ndkid.SelectedImageIndex = 1;

                nd.Nodes.Add(ndkid);


                ndkid = new TreeNode("MediaPackagePath : " + p.SourceList.MediaPackagePath);
                ndkid.Name = ndkid.Text;
                ndkid.ImageIndex = 1;
                ndkid.SelectedImageIndex = 1;

                nd.Nodes.Add(ndkid);



            }
            else
            {
                nd = new TreeNode("SourceList : ");
                nd.Name = nd.Text;
                node.Nodes.Add(nd);
                nd.ImageIndex = 1;
                nd.SelectedImageIndex = 1;


            }



        }

        public static void AddPatchProperty(TreeNode node, PatchInstallation p)
        {

            TreeNode nd = new TreeNode("DisplayName : " + p.DisplayName);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;
            nd.Name = nd.Text;
            node.Nodes.Add(nd);

            nd = new TreeNode("InstallDate : " + p.InstallDate);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);


            nd = new TreeNode("IsInstalled : " + p.IsInstalled);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("IsObsoleted : " + p.IsObsoleted);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd.Name = nd.Text;
            node.Nodes.Add(nd);

            nd = new TreeNode("IsSuperseded : " + p.IsSuperseded);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);


            nd = new TreeNode("LocalPackage : " + p.LocalPackage);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("PatchCode : " + p.PatchCode);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd.Name = nd.Text;
            node.Nodes.Add(nd);

            nd = new TreeNode("ProductCode : " + p.ProductCode);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);


            nd = new TreeNode("Transforms : " + p.Transforms);
            nd.Name = nd.Text;
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            node.Nodes.Add(nd);

            nd = new TreeNode("Uninstallable : " + p.Uninstallable);
            nd.ImageIndex = 1;
            nd.SelectedImageIndex = 1;

            nd.Name = nd.Text;
            node.Nodes.Add(nd);


            if (p.SourceList != null)
            {
                nd = new TreeNode("SourceList");
                nd.Name = nd.Text;
                node.Nodes.Add(nd);

                TreeNode ndkid = new TreeNode("PackageName : " + p.SourceList.PackageName);
                ndkid.Name = ndkid.Text;
                ndkid.ImageIndex = 1;
                ndkid.SelectedImageIndex = 1;

                nd.Nodes.Add(ndkid);

                ndkid = new TreeNode("LastUsedSource : " + p.SourceList.LastUsedSource);
                ndkid.Name = ndkid.Text;
                ndkid.ImageIndex = 1;
                ndkid.SelectedImageIndex = 1;

                nd.Nodes.Add(ndkid);


                ndkid = new TreeNode("MediaPackagePath : " + p.SourceList.MediaPackagePath);
                ndkid.Name = ndkid.Text;
                ndkid.ImageIndex = 1;
                ndkid.SelectedImageIndex = 1;

                nd.Nodes.Add(ndkid);



            }
            else
            {
                nd = new TreeNode("SourceList : ");
                nd.Name = nd.Text;
                node.Nodes.Add(nd);
                nd.ImageIndex = 1;
                nd.SelectedImageIndex = 1;


            }

        }
        public static void GetAllProducts()
        {
            var allPrds = ProductInstallation.GetProducts(null, null, UserContexts.All);

            int i = 0;
            if (isFilterOn && filterString != "")
            {
                root.Text = System.Environment.MachineName + " filtered by \"" + filterString + "\"";

            }
            else root.Text = System.Environment.MachineName;
             foreach (ProductInstallation p in allPrds)
            {
               // if (string.IsNullOrEmpty(p.ProductName)) continue;
               if(isFilterOn && filterString!="")
                {

                    if (string.IsNullOrEmpty(p.ProductName)) continue;
                    else if (!p.ProductName.ToLower().Contains(filterString.ToLower())) continue;
                }
                try
                {
                    UpdateProgress("Processing (" + i + ") " + p.ProductName);
                    i++; //if (i > 40) break;
                    TreeNode node = new TreeNode(p.ProductName);
                    node.Name = p.ProductName;
                    root.Nodes.Add(node);
                    TreeNode nodeProp = new TreeNode("Properties");
                    node.Nodes.Add(nodeProp);
                    try
                    {
                        AddProductProperty(nodeProp, p);
                    }
                    catch(Exception ex)
                    {
                        TreeNode error = new TreeNode(ex.Message);
                        nodeProp.Nodes.Add(error);
                        Logger.LogError(p.ProductName + ":" + ex.Message);
                    }


                    List<PatchInstallation> patches = PatchInstallation.GetPatches(null, p.ProductCode, null, UserContexts.All, PatchStates.All).ToList();
                    if (patches.Count > 0)
                    {
                        TreeNode pa = new TreeNode("Patches");
                        node.Nodes.Add(pa);
                        foreach (PatchInstallation pch in patches)
                        {

                            TreeNode nd = new TreeNode(pch.DisplayName);
                            nd.Name = pch.DisplayName;
                            pa.Nodes.Add(nd);

                            try
                            {
                                AddPatchProperty(nd, pch);
                            }
                            catch (Exception ex)
                            {
                                TreeNode error = new TreeNode(ex.Message);
                                nd.Nodes.Add(error);
                                Logger.LogError(pch.DisplayName+":"+ex.Message);
                            }
                            
                        }
                    }

                    if (p.Features != null)
                    {
                        List<FeatureInstallation> lst = p.Features.ToList();
                        if (lst.Count > 0)
                        {
                            TreeNode pa = new TreeNode("Features");
                            node.Nodes.Add(pa);

                            foreach (FeatureInstallation fi in lst)
                            {
                                string txt = "FeatureName : " + fi.FeatureName + "; State : " + fi.State.ToString();
                                TreeNode nd = new TreeNode(txt);
                                nd.Name = nd.Text;
                                pa.Nodes.Add(nd);
                                nd.ImageIndex = 1;
                                nd.SelectedImageIndex = 1;




                            }
                        }

                    }






                }
                catch (Exception ex)
                {
                    Logger.LogError("GetAllProducts:ProductCode:"+p.ProductCode+"\n" + ex.Message);
                }
            }
        }

        public static void NodeTextToList(List<string> lst,TreeNode node,int level)
        {
            int indent = (level-2) * 3;
            if (indent < 0) indent = 0;

            lst.Add(node.Text.PadLeft(node.Text.Length+indent,' '));
            if (level == 1) lst.Add("".PadLeft(node.Text.Length+1, '='));
            foreach ( TreeNode nd in node.Nodes)
            {
                NodeTextToList(lst,nd,level+1);
            }

            if (level == 1) lst.Add("\n");

        }

        public static List<string> GetAllNodesText()
        {
            List<string> result = new List<string>();
           
            int level = 0;
            NodeTextToList(result, root,  level);

            return result;

        }

        public static List<string> GetNodeAndChildren(TreeNode node)
        {
            
            List<string> result = new List<string>();
            if (node == root) return result;
            
            int level = node.Level;
            NodeTextToList(result, node, level);

            return result;

        }


    }
}
