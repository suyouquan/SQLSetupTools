using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.IO;
using Microsoft.Win32;


namespace SQLReg
{
    public partial class Form1 : Form
    {
        static Control lblInfo;
        static Control txtBox;
        static Control myform;
        public Form1()
        {
            InitializeComponent();
            Init();

         
           
        }

        public void Start()
        {
            Form scan = new scanForm();
            DialogResult r = scan.ShowDialog();
            if (r == DialogResult.OK)
            {
                GoScan();
            }
        }
        public void Init()
        {
            lblInfo = this.textBox2;
            txtBox = this.textBox1;
            myform = this;



            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + "  Version " + ver.Replace(".0.0", "");

            Logger.SetupLog();

            Controller.Init(UpdateStatus, DoneCallBack,DoneBGCallBack);

            propertyGrid.ReadOnly = true;
            propertyGrid.AllowUserToAddRows = false;
            propertyGrid.AllowUserToDeleteRows = false;


            textBox1.Text = "";

            int minSize = textBox1.Height + 2;
            splitContainer_outest
            .Panel1MinSize = minSize;
            splitContainer_outest.SplitterDistance = minSize;
            splitContainer_middle.Panel2MinSize = minSize;
            splitContainer_middle.SplitterDistance = splitContainer_middle.Height - minSize;

            textBox2.Width = this.Width - 80;

            //keys already sorted then generate the nodes. so we don't need the sorter, can save 4-12 seconds.
          //  regTree.TreeViewNodeSorter = new NodeSorter();


        }

   
     

        public class NodeSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = (TreeNode)x;
                TreeNode ty = (TreeNode)y;
                if (string.Compare(tx.Name, ty.Name, StringComparison.Ordinal) > 0) return 1;
                else if (string.Compare(tx.Name, ty.Name, StringComparison.Ordinal) ==0) return 0;
                else return -1;
             
            }
        }
        public void PopulateTreeView()
        {
            //use myData.rows to populate treeview
            regTree.Nodes.Add(Controller.rootNode);

        }

        public void ShowInfo(string msg)
        {
            lblInfo.Text = msg;
            //lbInfo.Refresh();
        }
        public void DoneCallBack()
        {
            myform.BeginInvoke((MethodInvoker)delegate
            {
                string msg= "Populating TreeiView (" + Controller.rootNode.GetNodeCount(true) + " nodes), may take serveral minutes...";
                Logger.LogMsg(msg);
                lblInfo.Text = msg;
                lblInfo.Refresh();
                //      myData.SetRow();//scan done, update the datasource of the gridview

                PopulateTreeView();


                this.menuStrip1.Enabled = true;
                lblInfo.Text = "";
                lblInfo.Refresh();
                Logger.LogMsg("TreeView  populate done. Background task is still running.");

                Thread background = new Thread(new ThreadStart(Controller.BackgroundTask));

                exportMenu.Enabled =exportToolStripMenuItem.Enabled= false;
                scanRegistryToolStripMenuItem.Enabled = false;
                findToolStripMenuItem.Enabled = findToolStripMenuItem1.Enabled = false;
                findNextToolStripMenuItem.Enabled = false;

                background.Start();

            });
        }

        //background task completed.
        public void DoneBGCallBack()
        {
            myform.BeginInvoke((MethodInvoker)delegate
            {
                string msg = "Background task finished, update treeview, may take minutes...";
                Logger.LogMsg(msg);
                lblInfo.Text = msg;
                lblInfo.Refresh();

                //now bigItems load completed. now need to add it to my node.

                foreach(string key in Controller.bigKeys.Keys)
                {
                    TreeNode node = RegNode.FindFromTree(Controller.rootNodeBig, key);
                    if(node!=null)
                    {
                        int idx = key.LastIndexOf("\\");
                        string parent = key.Substring(0, idx);
                        //TreeNode pnode = RegNode.FindFromTree(Controller.rootNode, parent);

                        TreeNode pnode = RegNode.FindLowestParent(Controller.rootNode, node);
                        TreeNode bigRightNode = RegNode.FindFromTree(Controller.rootNodeBig, pnode.Name);
                        if (pnode != null && bigRightNode!=null)
                        {
                            foreach (TreeNode nd in bigRightNode.Nodes)
                            {
                                pnode.Nodes.Add(nd);
                            }
                           RegNode.UpdateParentNodeText(pnode);
                        }
                        else
                        {
                            if (pnode == null) Logger.LogMsg("pnode is null.");
                            if (bigRightNode == null) Logger.LogMsg("bigRightNode is null.");
                            Logger.LogError("node's parent not found:" + node.Name);
                        }

                    }
                    else Logger.LogError("node not found for big key:" + key);
                }

                exportMenu.Enabled = exportToolStripMenuItem.Enabled = true;
                scanRegistryToolStripMenuItem.Enabled = true;
                findToolStripMenuItem.Enabled = findToolStripMenuItem1.Enabled = true;
                findNextToolStripMenuItem.Enabled = true;


                Logger.LogMsg("TreeView updated.");
                lblInfo.Text = "";
                MessageBox.Show("Scan Finished.", "Finish", MessageBoxButtons.OK, MessageBoxIcon.Information);

               
            });
        }

        public void UpdateStatus(string msg)// string data, int colorAARRGGBB = 0)
        {



            if (!this.InvokeRequired)
            {
                ShowInfo(msg);

            }

            else
            {
                Form1.lblInfo.BeginInvoke((MethodInvoker)delegate
                {
                    ShowInfo(msg);
                });
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // tbInfo.Width = this.Width - 80;
            textBox2.Width = this.Width - 80;
        }

        private void GoScan()
        {
            Thread th = new Thread(new ThreadStart(Controller.ScanSQLRegistry));
            this.menuStrip1.Enabled = false; //it will be enaled in callback function
            regTree.Nodes.Clear();
            th.Start();
        }


        private void scanRegistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Start();

        }
        public void SetDataSource(TreeNode node)
        {
            RegNode.SetSelectedInfo(-1, -1);

            if (node.Tag == null)
            {
                propertyGrid.DataSource = null;
                propertyGrid.Rows.Clear();
                return;
            }
            List<RegProperty> lst = ((RegKey)node.Tag).RegProperties;
            if (lst != null)
            {
                BindingList<RegProperty> blst = new BindingList<RegProperty>(lst);
                BindingSource src = new BindingSource(blst, null);
                propertyGrid.DataSource = src;
            }
            else
            {
                propertyGrid.DataSource = null;
                propertyGrid.Rows.Clear();
            }

        }

        private void SelectNode(TreeNode nd)
        {
            TreeNode node = nd;
            RegNode.treeViewSelectedNode = node;
            textBox1.Text = node.Name;
            textBox2.Text = "";
            SetDataSource(node);

            RegNode.SetSelectedInfo(-1, -1);
            propertyGrid.ClearSelection();

            if (node.Tag != null && ((RegKey)node.Tag).IsSQLRoot)
            {
                if (Controller.sqlRegKeys.ContainsKey(node.Name))
                {
                    Reason r = Controller.sqlRegKeys[node.Name];
                    textBox2.Text = "Reason:" + (String.IsNullOrEmpty(r.comment) ? r.tag : r.comment);
                }

            }
        }
       
        private void regTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectNode(e.Node);
        }

        private void propertyGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //clear the first selected cell at the very beginning
            this.propertyGrid.ClearSelection();
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Logger.logFileName)) return;
            var process = new Process();
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = Logger.logFileName;

            process.Start();
        }

        private string GetAllNodes_BreadthFirst()
        {
            StringBuilder sb = new StringBuilder();
            Controller.UpdateProgress("Saving result to file...");

            TreeNode root = Controller.rootNode;

            Queue<TreeNode> queue = new Queue<TreeNode>();

            queue.Enqueue(root);

            while (queue.Count > 0)
            {

                TreeNode currentNode = queue.Dequeue();

                if (currentNode.Tag != null)
                {
                    RegKey rk = (RegKey)currentNode.Tag;
                    sb.AppendLine("\n[" + rk.Path + "]");
                    foreach (RegProperty rp in rk.RegProperties)
                    {
                        sb.AppendLine(String.Format("\"{0}\" \"{1}\" \"{2}\"", rp.Name, rp.Type, rp.Data));
                    }

                }

                foreach (TreeNode node in currentNode.Nodes) queue.Enqueue(node);

            } //while queue.count>0



            return sb.ToString();

        }

        private string GetAllNodes_DepthFirst(TreeNode node)
        {
            //Get current nodes' details
            StringBuilder sb = new StringBuilder();

            if (node.Tag != null)
            {
                RegKey rk = (RegKey)node.Tag;
                if (Controller.sqlRegKeys.ContainsKey(rk.Path))
                {
                    sb.AppendLine("\n[" + Controller.sqlRegKeys[rk.Path].comment + "]");
                }
                else sb.AppendLine("\n");
                sb.AppendLine("[" + rk.Path + "]");
                foreach (RegProperty rp in rk.RegProperties)
                {
                    sb.AppendLine(String.Format("{0,-20} {1,-16} \"{2}\"", "\"" + rp.Name + "\"", rp.Type, rp.Data));
                }
            }

            foreach (TreeNode nd in node.Nodes)
            {
                string s = GetAllNodes_DepthFirst(nd);
                sb.AppendLine(s);
            }

            return sb.ToString();

        }
        private void Export(string prefix, TreeNode node)
        {
            System.Windows.Forms.SaveFileDialog dialog = new SaveFileDialog();
            Controller.UpdateProgress("Saving result to file...");

            string fileNameDetail = "";//detailed one
            string fileNameOutline = ""; //outline one

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string name = Path.GetFileName(dialog.FileName);
                string shortname = Path.GetFileNameWithoutExtension(dialog.FileName);
                //If it has details or outline remove it.
                shortname = shortname.Replace("_detail", "").Replace("_outline", "");

                string path = dialog.FileName.Replace(name, "");
                fileNameDetail = Path.Combine(path, shortname + "_detail.txt");
                fileNameOutline = Path.Combine(path, shortname + "_outline.csv");


                List<string> sorted = null;
                if (prefix == "") sorted = Controller.sqlRegKeys.Keys.OrderBy(s => s).ToList();
                else
                {
                    var pl = Controller.sqlRegKeys.Keys.Where(s => s.StartsWith(prefix));
                    sorted = pl.OrderBy(s => s).ToList();
                }
                if (sorted.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string s in sorted)
                    {
                        sb.AppendLine("\"" + s + "\"");

                    }
                    File.WriteAllText(fileNameOutline, sb.ToString());
                }
                else //nothing found, just export itself 
                    File.WriteAllText(fileNameOutline, prefix);

                //Now save the details

                string detail = GetAllNodes_DepthFirst(node);
                File.WriteAllText(fileNameDetail, detail);

                Process.Start("explorer.exe", path);

                Logger.LogMsg("Export to:\n\"" + fileNameDetail + " \"\n\"" + fileNameOutline + "\"");
                MessageBox.Show("Export Finished. Files saved to:\n" + fileNameDetail + "\n" + fileNameOutline, "Export Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Controller.UpdateProgress("");
                
            }
            else
            { Controller.UpdateProgress(""); return; }

        }
         
        public static string[] GetCleanUpForKey(string path,string comment="")
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_restore = new StringBuilder();

            string filename = Utility. CleanFileName(path) + ".hiv";
            sb.AppendLine("");
            if (comment != "") sb.AppendLine("REM " + comment);
            sb.AppendLine("REG SAVE " + "\"" + path + "\" \"" + filename + "\" /y /reg:64");
            sb.AppendLine("REG DELETE " + "\"" + path + "\" /f /reg:64");

            sb_restore.AppendLine("");
            if (comment != "") sb_restore.AppendLine("REM " + comment);
            sb_restore.AppendLine("REG ADD " + "\"" + path + "\"  /f ");
            sb_restore.AppendLine("REG RESTORE " + "\"" + path + "\" \"" + filename + "\" /f ");

            string[] result = new string[] { sb.ToString(), sb_restore.ToString() };
            return result;
        }
        public string[] GetCleanup(string prefix)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb_restore = new StringBuilder();

            var sorted = Controller.sqlRegKeys.OrderBy(p => p.Value.comment);
            foreach (KeyValuePair<string,Reason> kv in sorted)
            {
                string s = kv.Key;
                string comment = kv.Value.comment;

                bool matched = false;
                if (string.IsNullOrEmpty(prefix)) matched = true;
                else if (s.StartsWith(prefix)) matched = true;

                if (matched)
                {
                    if (Controller.sqlRegKeys[s].cleanable)
                    {
                        string[] r = GetCleanUpForKey(s, comment);
                        sb.Append(r[0]);
                        sb_restore.AppendLine(r[1]);
                    }
                }
            }

            string[] result = new string[] { sb.ToString(), sb_restore.ToString() };
            return result;

        }

      

        public void GenerateCleanupScript(TreeNode node)
        {

            //at this moment, the functionality to generate cleanup script for a node (not root) is removed. but the code is here still.

            string[] result = null;
            string name = "";
            if (RegNode.IsMeOrParentSQLRoot(node))
            {
                result = GetCleanUpForKey(node.Name);
              
            }

            else 
            {
                //we should export all its child keys which are SQL exclusively owned.
                
                 

                if (node == Controller.rootNode) result = GetCleanup("");
                else result = GetCleanup(node.Name);

                     

            }
            name = Utility.CleanFileName(node.Name);
            SaveCleanupScript(name,result);

        }

        //just delete installer\products and patches, don't touch others.
        //Also generate msiexec.exe /X {xxx-xxx-xxx-xxx} /quiet /L*V xxx.log
        public string GetMsiExecUninstallScript()
        {

            StringBuilder sb = new StringBuilder();
            /*
            List<string> keys = new List<string>() {
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Products",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UserData\S-1-5-18\Patches",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\UpgradeCodes",
                @"HKEY_CLASSES_ROOT\Installer\Products",
                @"HKEY_CLASSES_ROOT\Installer\Patches",
                @"HKEY_CLASSES_ROOT\Installer\UpgradeCodes"
                        };
                        */

            foreach(string pd in Analysis.Installer.foundCompressedProductCodes)
            {
                string pcode = Utility.ConvertCompressedCodeToNormal(pd);
                string cmd = "msiexec.exe /X {"+pcode+"} /quiet /L*V {"+pcode+"}.log";
                string comment = "";
                if(Controller.sumSQLProduct.productCodeToName.ContainsKey(pd))
                {
                    comment = "REM "+Controller.sumSQLProduct.productCodeToName[pd];
                }
                if(comment!="") sb.AppendLine(comment);
                sb.AppendLine(cmd);
                sb.AppendLine("");
            }


            return sb.ToString();
        }
       private List<string> GetReadMeTxt()
        {
            List<string> readme = new List<string>();
            
            readme.Add("Please always try to uninstall SQL server from control panel->uninstall or from SQL setup program. ");
            readme.Add("\n");
            readme.Add("In case the normal way doesn't work, you can try below cleanup steps:");
            readme.Add("\n");
            readme.Add("About the Cleanup Script");
            readme.Add("===========================");
            
            readme.Add("1)Backup the machine, its registry key, and your SQL server databases.");
            readme.Add(@"2)Backup ""C:\Program Files\Microsoft SQL Server"" and ""C:\Program Files (x86)\Microsoft SQL Server"". If it is  not ""C:\"" in your machine please change it accordingly." );
            readme.Add(@"3)Go to [Computer Management]->[Local users and Groups], delete those SQL server related users or groups.");
            readme.Add("4)Rename RegCleanUp_xxx_xxx.txt to RegCleanUp.bat, and run it in a DOS prompt window with admin rights.");
            readme.Add("5)Reboot the machine.");
            readme.Add(@"6)Delete ""C:\Program Files\Microsoft SQL Server"" and ""C:\Program Files (x86)\Microsoft SQL Server""");
            readme.Add("If the two folders cannot be deleted due to file in use error, check whether there are any running SQL related services or applications holding the files. If any please stop them and try again.");
            readme.Add("Once the above two folders are deleted, your are done, and you can go ahead to install SQL server again.");
            readme.Add("\n");

            readme.Add("About the Restore Script");
            readme.Add("==========================");
            readme.Add("In case you need to restore the stuff deleted by the cleanup script, ");
            readme.Add(@"1)Restore ""C:\Program Files\Microsoft SQL Server"" and ""C:\Program Files (x86)\Microsoft SQL Server""");
            readme.Add("2)Rename the RegRestore_xxx_xxx.txt to RegRestore.bat and run it in DOS prompt window with admin rights, ");
            readme.Add("3)Go to [Computer Management]->[Local users and Groups], add back the deleted users and groups.Add them to admin group if you don't know their permission configurations.");
            readme.Add("4)Reboot the machine.");


            return readme;
        }

        public void SaveCleanupScript(string name,string[] result)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.RootFolder = Environment.SpecialFolder.MyComputer;

         //   System.Windows.Forms.SaveFileDialog dialog = new SaveFileDialog();
            Controller.UpdateProgress("Saving Cleanup script to files...");

            string fileNameDelete = "";//detailed one
            string fileNameRestore = ""; //outline one
            name = name.Replace("Computer-", "");

            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                string folder =folderDlg.SelectedPath;
                string target = "";
                string subFolder = System.DateTime.Now.ToString("MM-dd_HH_mm_ss");
                //if it is 
                if (folder.Contains(subFolder)) target = folder;
                else
                {
                    target = Path.Combine(folder, subFolder);

                    if (!Directory.Exists(target))
                    {
                        Directory.CreateDirectory(target);
                    }
                }

                string msiexec = GetMsiExecUninstallScript();

                string fileNameMsi = Path.Combine(target, "msiexec_uninstall_" + name + ".txt");
                string readmeFile = Path.Combine(target, "readme.txt");
                fileNameDelete = Path.Combine(target,   "RegCleanUp_"+ name + ".txt");
                fileNameRestore = Path.Combine(target,    "RegRestore_" +name  + ".txt");

                StringBuilder sbAdmin = new StringBuilder();
                sbAdmin.AppendLine("REM ***********************************************");
                sbAdmin.AppendLine("REM Check whether batch is running in Admin mode...");
                sbAdmin.AppendLine("@echo off");
                sbAdmin.AppendLine("fsutil dirty query %systemdrive% >nul 2>nul");
                sbAdmin.AppendLine("if %errorlevel% NEQ 0 (");
                sbAdmin.AppendLine("echo Please run the batch with admin rights.");
                sbAdmin.AppendLine("GOTO END");
                sbAdmin.AppendLine(") ");
                sbAdmin.AppendLine("@echo on");
                sbAdmin.AppendLine("REM ***********************************************");
                sbAdmin.AppendLine("\n");
                StringBuilder sbEnd = new StringBuilder();



                File.WriteAllLines(readmeFile, GetReadMeTxt().ToArray());

                /*
                //msiexec /X
                //I don't want to provide this one because it is not restorable?   
                File.WriteAllText(fileNameMsi, sbAdmin.ToString() + msiexec + "\n:END\n");
               */
                //installer
                File.WriteAllText(fileNameDelete, sbAdmin.ToString() + result[0]);
                
                //service
                foreach(string s in Analysis.Services.serviceCleanupScript)  File.AppendAllText(fileNameDelete  ,s+"\n" );
                File.AppendAllText(fileNameDelete, "\n:END");
                //restore
                File.WriteAllText(fileNameRestore, sbAdmin.ToString() + result[1]);
                foreach(string s in Analysis.Services.serviceRestoreScript) File.AppendAllText(fileNameRestore, s+"\n");
                File.AppendAllText(fileNameRestore, "\n:END");

                Process.Start("explorer.exe", target);

                Logger.LogMsg("Cleanup script to:\n\"" + fileNameMsi+"\n" + fileNameDelete + " \"\n\"" + fileNameRestore + "\"");
                MessageBox.Show("Cleanup script saved. Files saved to:\n" + fileNameDelete + "\n" + fileNameRestore, "Cleanup Script Saved Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Controller.UpdateProgress("");



            }
            else
            { Controller.UpdateProgress(""); return; }

        }
        private void exportMenu_Click(object sender, EventArgs e)
        {
            Export("", Controller.rootNode);
        }
        private void Find()
        {
            Form search = new SearchForm();
            DialogResult r = search.ShowDialog();
            if (r == DialogResult.Yes)
            {
                RegNode.findResult.node.TreeView.SelectedNode = RegNode.findResult.node;
                SetSelectedForGrid();
                if (RegNode.findResult.isNodeText) this.regTree.Focus();
                else this.propertyGrid.Focus();

            }
            else if (r == DialogResult.No)
            {
                MessageBox.Show("Key not found!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.regTree.Focus();
        }
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find();

        }
        private void SetSelectedForGrid()
        {
            propertyGrid.ClearSelection();
            int row = RegNode.findResult.hitRow;
            int col = RegNode.findResult.hitCol;
            if (row >= 0 && row < propertyGrid.Rows.Count
                && col >= 0 && col <= 2)
            {
                propertyGrid.Rows[row].Cells[col].Selected = true;
            }
        }
        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(RegNode.lastFindText))
            {

                bool isFound = RegNode.Find(RegNode.lastFindText);
                if (isFound)
                {
                    RegNode.findResult.node.TreeView.SelectedNode = RegNode.findResult.node;
                    SetSelectedForGrid();
                    if (RegNode.findResult.isNodeText) this.regTree.Focus();
                    else this.propertyGrid.Focus();
                }
                else
                    MessageBox.Show("Key not found!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);


            }

        }

        private void propertyGrid_SelectionChanged(object sender, EventArgs e)
        {
            int cnt = propertyGrid.SelectedCells.Count;
            if (cnt == 0) return;
            int minCol = 0;
            int minRow = 0;
            minRow = Math.Max(propertyGrid.SelectedCells[0].RowIndex, minRow);
            minCol = Math.Max(propertyGrid.SelectedCells[0].ColumnIndex, minCol);
            for (int i = 1; i < cnt; i++)
            {
                minRow = Math.Min(propertyGrid.SelectedCells[i].RowIndex, minRow);
                minCol = Math.Min(propertyGrid.SelectedCells[i].ColumnIndex, minCol);
            }
            RegNode.SetSelectedInfo(minRow, minCol);
            //   textBox2.Text = minRow + ":" + minCol;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cleanupScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // MessageBox.Show("Cleanup Script function is still under testing, will be released in next update.");
          //  return;

            if (Controller.ShowWarning == true)
            {
                Form warning = new CleanupWarning();
                DialogResult r =
                    warning.ShowDialog();

                if (r == DialogResult.Cancel) return;

            }
            GenerateCleanupScript(Controller.rootNode);
        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void regTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                regTree.SelectedNode = e.Node;
                contextMenuStrip1.Tag = e.Node;
                contextMenuStrip1.Show(regTree, e.Location);
            }


        }

        private void copyKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = regTree.SelectedNode.Name;
            if (!String.IsNullOrEmpty(text))
            {
                Clipboard.SetText(regTree.SelectedNode.Name);
                UpdateStatus(text + " copied to clipboard.");
            }
        }

        private void goToRegEditToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var process = new Process();
            process.StartInfo.FileName = "RegEdit.exe";
            string path = "Computer\\" + regTree.SelectedNode.Name;
            Logger.LogMsg("GotoRegEdit:" + path);
            if (!String.IsNullOrEmpty(path))
            {
                RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Applets\Regedit", true);
                if (rKey != null)
                {
                    rKey.SetValue("LastKey", path);
                    process.StartInfo.Arguments = "/m"; //multiple instance
                    process.Start();
                    Logger.LogMsg("Start RegEdit.exe with " + path);
                }
                else
                {
                  //  RegistryKey rKey2 = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Applets\Regedit");
                    

                    //No one ever run regedit.exe. so just run it and close it.
                    Logger.LogMsg("rKey is null! " + path);
                    process.Start();
                    process.Close(); //Now it shoudl have the key set.

                     
                }
            }


        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (regTree.SelectedNode == Controller.rootNode)
            {
                Export("", Controller.rootNode);
            }
            else
                Export(regTree.SelectedNode.Name, regTree.SelectedNode);
        }

        private class MyCell
        {
            public Int32 rowIdx;
            public Int32 colIdx;
            public String Value;
            public MyCell(int ri, int ci, string v)
            {
                rowIdx = ri; colIdx = ci; Value = v;
            }

        }
        private void CopySelectedCell()
        {
            var cnt = propertyGrid.SelectedCells.Count;
            if (cnt <= 0) return;
            string s = "";
            var rowIdx = 0;
            List<MyCell> cells = new List<MyCell>();
            foreach (var cell in propertyGrid.SelectedCells)
            {
                if (cell.GetType() == typeof(DataGridViewTextBoxCell))
                {
                    DataGridViewTextBoxCell c = (DataGridViewTextBoxCell)cell;
                    cells.Add(new MyCell(c.RowIndex, c.ColumnIndex, c.Value == null ? "" : c.Value.ToString()));
                }
            }
            cells = cells.OrderBy(p => p.rowIdx).ThenBy(p => p.colIdx).ToList();
            bool isFirst = true;
            foreach (MyCell c in cells)
            {

                if (rowIdx != c.rowIdx)
                {
                    rowIdx = c.rowIdx;
                    s = s + "\n\r";
                    isFirst = true;
                }
                //first cell
                if (isFirst) { s = s + "\"" + c.Value + "\""; isFirst = false; }
                else s = s + "," + "\"" + c.Value + "\"";
            }


            if (!String.IsNullOrEmpty(s))
            {
                Clipboard.SetText(s.Trim());
                UpdateStatus(cnt + " selected cell(s) copied to clipboard.");
            }
        }
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var cnt = propertyGrid.SelectedCells.Count;
            if (cnt <= 0)
            {
                UpdateStatus("Please select at least one cell to copy.");
                return;
            }

            try
            {
                CopySelectedCell();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message );

            }
        }

        private void cleanupScriptToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        
            //I don't want to implement this. it is too risky to privide them to end user
            /*
            if (Controller.ShowWarning==true)
            {
                Form warning = new CleanupWarning();
                DialogResult r=
                    warning.ShowDialog();

                if (r == DialogResult.Cancel) return;

            }

            if (regTree.SelectedNode == Controller.rootNode)
            {
                GenerateCleanupScript( Controller.rootNode);
            }
            else
                GenerateCleanupScript( regTree.SelectedNode);
                */
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Start();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form about = new AboutForm();
            about.ShowDialog();
            
        }

        private void readMeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //string path2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            
            path = Path.Combine(path, "Manual");
            path = Path.Combine(path, "SQLRegistryViewer Manual.pdf");

            Process.Start("explorer.exe", path);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
            {

                e.Cancel = true;

            }
            else
            {
                //Need to destroy those running threads
                Controller.shouldAbort = true;
                this.Text = this.Text + " Closing,Please wait for a while...";
                foreach(Thread td in Controller.normalThreads)
                {
                    if (td.ThreadState != System.Threading.ThreadState.Stopped) td.Abort();
                }

                foreach (Thread td in Controller.bigThreads)
                {
                    if (td.ThreadState != System.Threading.ThreadState.Stopped) td.Abort();
                }

                //Now wait for them to exit
                foreach (Thread td in Controller.normalThreads)
                {
                    td.Join();
                }

                foreach (Thread td in Controller.bigThreads)
                {
                    td.Join();
                }

            }
        }
    }
}
