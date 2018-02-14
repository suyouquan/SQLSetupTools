using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
using System.Runtime.InteropServices;

namespace SQLReg
{
    public enum SourceType
    {
        COMClass,
        FromMSI,
        FromMSI_File,
        FromMSI_PackageName,
        FromMSI_PackageCode,
        FromMSI_TargetProductName,
        FromMSI_KBArticle,

        Installer_ProductCode,
        Installer_PatchCode,
        Installer_Component,
        Installer_Feature,
        Installer_UpgradeCode,
        KnownKey,
        Service,
        Uninstall,
        Other
    }
    public class Reason
    {
        public SourceType st;
        public string tag;
        public string comment;
        public bool cleanable;
        public Reason(SourceType st0, string tag0, string comment0 = "", bool canClean = false)
        {
            st = st0;
            comment = comment0;
            tag = tag0;
            cleanable = canClean;
        }
    }
    public static class Controller
    {
        public static List<RegKey> regKeys = new List<RegKey>();

        public static TreeNode rootNode = null;
        public static TreeNode rootNodeBig = null;
        public static int Index = 0;
        public delegate void callbackProgressFunc(string info);
        public delegate void callbackDoneFunc();
        public delegate void callbackDoneBGFunc();

        private static callbackProgressFunc UpdateUI = null;
        public static callbackDoneFunc DoneCallBack = null;
        public static callbackDoneBGFunc DoneBGCallBack = null;



        // public static List<SQLProduct> sqlProducts = new List<SQLProduct>();
        //All found SQL server  related keys
        //  public static HashSet<String> sqlRegKeys = new HashSet<string>();
        public static Dictionary<string, Reason> sqlRegKeys = new Dictionary<string, Reason>();

        //this record why the key is in sqlRegKeys
        //public static Dictionary<string, Reason> keyReason = new Dictionary<string, Reason>();



        //this registry is related to SQL product but cannot be delete because i don't know whether SQL exclusively owns it
        //or not, for example, from MSI, if name!="*", then i will add it to this collection
        //  public static HashSet<String> sqlRegKeys_Shared = new HashSet<string>();
        //we have cleanable in reason object, so we can remove this sqlRegKeys_Shared object

        public static string SQLSetupSource = "";
        public static string LastSQLSetupSource = "";

        public static bool ShowWarning = true;

        public static SQLProductSum sumSQLProduct = null;
        //  public static List<SQLProduct> products = new List<SQLProduct>();
        public static List<SQLProductSum> productSums = new List<SQLProductSum>();
        public static SQLProductSum sumFromSetupSrc = null;
        public static string cachedMetaDataFile = null;

        public static Dictionary<string, RegHive> regHives = new Dictionary<string, RegHive>();


        public static Dictionary<string, subKey> bigKeys = new Dictionary<string, subKey>()
            {
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components",null },

            };

        public static List<Thread> bigThreads = new List<Thread>();
        public static List<Thread> normalThreads = new List<Thread>();
        public static bool shouldAbort = false;

        public static void Init(callbackProgressFunc fn, callbackDoneFunc done, callbackDoneBGFunc bgDone)
        {

            UpdateUI = fn;
            DoneCallBack = done;

            rootNode = new TreeNode("Computer-" + System.Environment.MachineName);
            rootNode.Name = "Computer-" + System.Environment.MachineName; ;
            rootNode.Expand();

            rootNodeBig = new TreeNode("Computer-" + System.Environment.MachineName);
            rootNodeBig.Name = "Computer-" + System.Environment.MachineName; ;


            DoneBGCallBack = bgDone;
            //test();
        }



        public static void UpdateProgress(string msg, bool WriteToLogFile = false)
        {
            UpdateUI(msg);
            if (WriteToLogFile) Logger.LogMsg(msg);
        }
        public static void LoadSQLProductFromSetupSrc()
        {

            LastSQLSetupSource = SQLSetupSource;
            //  string path = @"D:\SETUP Media\2016";
            //  path = @"\\sqlbuilds\Released\SQLServer2016\RTM\13.0.1601.5\release\editions\SQLFull_CHS";
            UpdateProgress("Scanning MSI/MSP packages from:" + SQLSetupSource, true);
            //   SQLProduct sql2016 = new SQLProduct("SQL2016", SQLSetupSource);
            //  sqlProducts.Add(sql2016);

            string folder = Utility.CleanFileName(SQLSetupSource);

            if (folder.Length > 20)
            {
                folder = folder.Substring(0, 10) + "..." + folder.Substring(folder.Length - 10);
            }

            string name = "Cached_" + folder + System.DateTime.Now.ToString("_yyyy-MM-dd_HH_mm_ss");

            SQLProduct sql = new SQLProduct(name, SQLSetupSource);


            try
            {
                //Save it temp folder so that we can copy it to data folder.
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = Path.Combine(path, "Cache");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }
                sumFromSetupSrc = new SQLProductSum(name, SQLSetupSource, "By SQLMsiMspScan.");
                sumFromSetupSrc.InitOrAddHashSet(sql);
                string file = Path.Combine(path, name + ".sum.xml");
                string content = OutputProcessor.DataContractSerializeToXML<SQLProductSum>(sumFromSetupSrc);
                File.WriteAllText(file, content);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }


        }



        //load processed data from PreData folder
        public static SQLProductSum LoadSQLProductFromMetaData()
        {
            SQLProductSum sum = null;
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = Path.Combine(path, "MetaData");
            if (!Directory.Exists(path))
            {
                Logger.LogMsg("[LoadSQLProductFromMetaData]Data folder doesn't exist:" + path);
                return null;
            }

            UpdateProgress("Loading MSI/MSP meta data from:" + path + ", may take minutes...", true);

            List<string> xmlFiles = Utility.GetFilesFromFolder(path, new string[] { "*.xml" });
            SQLProductSum totalSum = new SQLProductSum("All-Pre-Sum", "Pre-MetaData", "Merge All sum in predata folder into one");
            foreach (string f in xmlFiles)
            {
                try
                {
                    UpdateProgress("Reading MSI/MSP meta data from pre-processed data:" + Path.GetFileName(f) + ", may take minutes...", true);
                    sum = OutputProcessor.DataContractDeSerializeToXML<SQLProductSum>(f);
                    totalSum.AddProductSum(sum);


                }
                catch (Exception ex)
                {
                    Logger.LogError("[LoadSQLProductFromMetaData]:" + ex.Message);
                }

            }

            return totalSum;
        }



        public static void GenerateRegNodes()
        {
            UpdateProgress("Generating treeview nodes from sqlRegKeys:" + sqlRegKeys.Keys.Count + " keys...", true);

            rootNode.Nodes.Clear();
            RegNode.nodesList.Clear();
            int cnt = 0;
            List<string> keys = sqlRegKeys.Keys.OrderBy(p => p).ToList();//sort them to avoid the treeview node sorter.
            foreach (string regkey in keys)
            {
                // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Microsoft SQL Server
                if (cnt % 500 == 0)
                {
                    Controller.UpdateProgress("Processing " + regkey, false);
                    //If closing
                    if (Controller.shouldAbort) return;

                }
                cnt++;

                RegNode.AddKey(rootNode, regkey);
            }
        }



        public static List<Thread> LoadRegHives()
        {
            Dictionary<string, subKey> keys = new Dictionary<string, subKey>()
            {
                //this one will be lazy loaded
             //   {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Components",null },

                { @"HKEY_CLASSES_ROOT\Installer\Products",new subKey("SourceList", "PackageName") },
                { @"HKEY_CLASSES_ROOT\Installer\Patches",new subKey("SourceList", "PackageName") },
                { @"HKEY_CLASSES_ROOT\Installer\Dependencies",null },
                { @"HKEY_CLASSES_ROOT\Installer\Features",null },
                { @"HKEY_CLASSES_ROOT\Installer\UpgradeCodes",null },
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products",null },
                {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Patches",null },

                {@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UpgradeCodes",null },

                //COM+ related
                { @"HKEY_CLASSES_ROOT\Wow6432Node\CLSID",new subKey("InProcServer32", "") },
                {@"HKEY_CLASSES_ROOT\CLSID",new subKey("InProcServer32", "") },
                {@"HKEY_CLASSES_ROOT\Wow6432Node\TypeLib",null },
                {@"HKEY_CLASSES_ROOT\TypeLib",null },
                {@"HKEY_CLASSES_ROOT\Wow6432Node\Interface",null },
                {@"HKEY_CLASSES_ROOT\Interface",null},


        };

            List<Thread> threads = new List<Thread>();
            foreach (KeyValuePair<string, subKey> kv in keys)
            {
                LoadRegHive lrh = new LoadRegHive(kv.Key, kv.Value);
                Thread th = new Thread(new ThreadStart(lrh.Load));
                th.Start();
                threads.Add(th);
            }

            return threads;
        }
        public static void ScanSQLRegistry()
        {
            UpdateProgress("Scan starts...", true);

            // var m=LoadSQLProductFromPreData();
            //  m.name = "SQL2008-2017RTM_AllLang";
            //  m.source = "";
            //  m.SaveData("SQL2008-2017RTM_AllLang", @"c:\temp\SQL2008-SQL2017_AllLang.sum.xml");

            ////  LoadSQLProductFromSetupSrc();


            if (sumSQLProduct != null) sumSQLProduct.Reset();

            //  SQLProduct sql = new SQLProduct("SQL2012", @"D:\SETUP Media\2016\2016 SQLFull_ENU"); 
            // SQLProduct sql = new SQLProduct("SQL2012", @"\\shlabprod01\Products\Applications\Server\SQL\SQL Server 2012\SQLFULL");
            //    string data = OutputProcessor.SerializeToXML<SQLProduct>(sql);
            /*
            string f = @"c:\temp\SQL2016RTM_ENU.xml";
            //  File.WriteAllText(f, data);
            SQLProduct sql0 = new SQLProduct(f);
            sqlProducts.Add(sql0);
            */

            Thread sourceTD = null;
            if (SQLSetupSource != "")
            {

                sourceTD = new Thread(new ThreadStart(LoadSQLProductFromSetupSrc));
                sourceTD.Start();

            }


            //start thread to load registry keys first.
            regHives.Clear();
            normalThreads = LoadRegHives();

            //and also load big Items
            bigThreads = LoadBigItems();

            //Load pre-processed data;
            sumSQLProduct = LoadSQLProductFromMetaData();

            if (!string.IsNullOrEmpty(cachedMetaDataFile))
            {
                sumSQLProduct.AddProductSumFromFile(cachedMetaDataFile);
            }

            if (sourceTD != null) sourceTD.Join();


            //Time to merge it.
            if (sumFromSetupSrc != null) sumSQLProduct.AddProductSum(sumFromSetupSrc);

            ////Now need to add products to gProduct
            //foreach (SQLProduct sql in Controller.products)
            //{
            //    sumSQLProduct.InitOrAddHashSet(sql);
            //}
            //Time to get SQL related keys
            sqlRegKeys.Clear();

            //wait for all thread loading is completed.
            foreach (Thread th in normalThreads) th.Join();

            //If closing
            if (Controller.shouldAbort) return;


            Analysis.KnownKeys.Add();
            Analysis.Installer.Add();

            Analysis.Services.Add();
            Analysis.Uninstall.Add();//need to call after Installer.Add because it will reference data geot from installer.Add function
            Analysis.COMClass.Add();

            Analysis.FromMSI.Add();
            //Now generate nodes for treeview

            Logger.LogMsg("Total found SQL related keys:" + sqlRegKeys.Count);
            GenerateRegNodes();

            Logger.LogMsg("UpdateNodeTextWithNodeCount started.");
            RegNode.UpdateNodeTextWithNodeCount(Controller.rootNode);
            Logger.LogMsg("UpdateNodeTextWithNodeCount done.");


            UpdateProgress("Scan done.", true);
            DoneCallBack();


        }

        public static void BackgroundTask()
        {
            //Here we start new thread to load Components which is too big to load so i loaded it in background.
            //List<Thread> bigThreads = LoadBigItems();
            foreach (Thread th in bigThreads) th.Join();

            //Now start another thread to compare the Component key.
            if (Controller.shouldAbort) return;

            Thread thComponents = new Thread(new ThreadStart(Analysis.Installer.Add_HKLM_Components));
            thComponents.Start();
            thComponents.Join();

            if (Controller.shouldAbort) return;


            GenerateNodeForBigItems();

            Logger.LogMsg("UpdateNodeTextWithNodeCount (BG) started.");
            RegNode.UpdateNodeTextWithNodeCount(Controller.rootNodeBig);
            Logger.LogMsg("UpdateNodeTextWithNodeCount (BG) done.");

            DoneBGCallBack();



        }




        public static void GenerateNodeForBigItems()
        {


            int cnt = 0;
            List<string> keys = sqlRegKeys.Keys.OrderBy(p => p).ToList();//sort them to avoid the treeview node sorter.
            foreach (string s in bigKeys.Keys)
            {
                foreach (string regkey in keys)
                {
                    if (regkey.StartsWith(s))
                    {

                        if (cnt % 500 == 0)
                            Controller.UpdateProgress("Processing " + regkey, false);
                        cnt++;

                        RegNode.AddKey(rootNodeBig, regkey);
                    }
                }
            }
        }
        public static List<Thread> LoadBigItems()
        {


            List<Thread> threads = new List<Thread>();
            foreach (KeyValuePair<string, subKey> kv in bigKeys)
            {
                LoadRegHive lrh = new LoadRegHive(kv.Key, kv.Value);
                Thread th = new Thread(new ThreadStart(lrh.Load));
                th.Start();
                threads.Add(th);
            }

            return threads;

        }
        /*
        public class typeCnt { public string type; public int count; }
        public static void test()
        {


            List<typeCnt> result = Controller.sqlRegKeys
                .GroupBy(p => p.Value.st)
                .Select(pn => new typeCnt
                {
                    type = pn.Key.ToString(),
                    count = pn.Count()

                }).ToList();



            List<string> file = Controller.sqlRegKeys
             .Where(p => p.Value.st == SourceType.FromMSI_File)
             .Select(pn =>   pn.Value.tag 
              ).ToList();
            File.WriteAllLines(@"c:\temp\files.txt", file.ToArray());

        }*/

    }
}
