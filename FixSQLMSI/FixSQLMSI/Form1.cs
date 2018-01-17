using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Package;
 
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace FixSQLMSI
{
    public partial class Form1 : Form
    {
        static Control lbl;
        static Control myform;
        public Form1()
        {
            InitializeComponent();

            lbl = this.lbInfo;
            Logger.SetupLog();
            
            myData.Init(dataGridView1,UpdateStatus,DoneCallBack);
            this.Height = 600;
            this.Width = 1000;

            ResizeIt();

            myform = this;

            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + "  Verion" + ver.Replace(".0.0","");
            string msp = @"c:\temp\sqlbrowser.msp";
            msp = @"D:\SETUP Media\2016\SP1CU6\1033_ENU_LP\x64\Setup\SQLBROWSER.MSP";
            MsiMspPackage pkk = new MsiMspPackage(msp);
           
        }

        public void UpdateStatistics()
        {
            this.lbTotal.Text = "Total: " + this.dataGridView1.Rows.Count.ToString();
            int mismatched = 0, missing = 0, ok = 0;
            foreach (DataGridViewRow r in this.dataGridView1.Rows)
            {
                if ((CacheFileStatus)r.Cells["Status"].Value == CacheFileStatus.Mismatched)
                    mismatched++;
                else if ((CacheFileStatus)r.Cells["Status"].Value == CacheFileStatus.Missing)
                    missing++;
                else if ((CacheFileStatus)r.Cells["Status"].Value == CacheFileStatus.OK)
                    ok++;
            }

            this.lbOK.Text = "OK: " + ok;
            this.lbMismatched.Text = "Mismatched: " + mismatched;
            this.lbMissing.Text = "Missing: " + missing;
            // Logger.LogMsg("UpdateStatistics called.");
        }
        
       public void ShowInfo(string msg)
        {
            lbInfo.Text = msg;
          //lbInfo.Refresh();
        }
        private void SetColumnSort()
        {

            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
            {
                //设置自动排序
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }
        public void DoneCallBack()
        {
            myform.BeginInvoke((MethodInvoker)delegate
            {
                lbInfo.Text = "Scan done. Formatting rows...";
                lbInfo.Refresh();
                myData.SetRow();//scan done, update the datasource of the gridview
               
                UpdateColorForDataGridView();
                UpdateStatistics();
                this.Enabled = true;
                lbInfo.Text = "Done.";
                lbInfo.Refresh();
                SetColumnSort();
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
                Form1.lbl.BeginInvoke((MethodInvoker)delegate
                {
                    ShowInfo(msg);
                });
            }
        }

        private void btnFolderBrowse_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new FolderBrowserDialog();

           
            
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //  MessageBox.Show("You selected: " + dialog.FileName);
                tbSQLMediaPath.Text = dialog.SelectedPath;

            }



        }
 

        private void rbMissingOrMismatched_CheckedChanged(object sender, EventArgs e)
        {
            this.Enabled = false;
            //Missing/Mismtached only
            if (((RadioButton)sender).Checked == true)
            {
                lbInfo.Text = "Refreshing...may take minutes...";
                lbInfo.Refresh();
                myData.SetFilter(dataGridView1);
                UpdateColorForDataGridView();
                UpdateStatistics();
            }
            else
            {
                lbInfo.Text = "Refreshing...may take minutes...";
                lbInfo.Refresh();
                myData.RemoveFilter(dataGridView1);
                UpdateColorForDataGridView();
                UpdateStatistics();
            }
            lbInfo.Text = "";
            lbInfo.Refresh();

            this.Enabled = true;
        }

        

        private void btnLog_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Logger.logFileName)) return;
            var process = new Process();
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = Logger.logFileName;

            process.Start();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            typeof(DataGridView).InvokeMember(
            "DoubleBuffered",
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
            null,
            dataGridView1,
            new object[] { true });
        }




        private void btnScan_Click(object sender, EventArgs e)
        {
        
            if (String.IsNullOrEmpty(tbSQLMediaPath.Text))
            {
                DialogResult result = MessageBox.Show("SQL setup source folder is not specified."
                    + "\n\nYou may fail to fix missing/mismatched cached MSI/MSP files. If you specify the folder after SCAN, you need to click SCAN again." +
                    "\n\nPlease click Yes to scan anyway. "
                    + "or click No to cancel this operation.", "SQL setup source folder is not specified",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    lbInfo.Text = "Scan begin,may take minutes...";
                    lbInfo.Refresh();

                   // myData.RemoveFilter(dataGridView1);

                    rbAll.Checked = true;

                    this.Enabled = false;
                    myData.setupSource = "";
                    myData.RemoveDataSource();
                    Thread th = new Thread(new ThreadStart(myData.ScanWithoutSQLSetupSource));

                    // myData.ScanWithSQLSetupSource();
                    th.Start();
                  //  myData.ScanWithoutSQLSetupSource();
 

                }

            }
            else
            {
                if (!Directory.Exists(tbSQLMediaPath.Text.Trim()))
                {
                    MessageBox.Show("The specified folder doesn't exist!\n" + tbSQLMediaPath.Text.Trim(), "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    lbInfo.Text = "Scan begin...";
                    lbInfo.Refresh();
                  
                    //  myData.RemoveFilter(dataGridView1);

                    rbAll.Checked = true;
                    this.Enabled = false;
                    myData.setupSource = tbSQLMediaPath.Text.Trim();
                    myData.RemoveDataSource();
                    Thread th = new Thread(new ThreadStart(myData.ScanWithSQLSetupSource));

                    // myData.ScanWithSQLSetupSource();
                    th.Start();
                  

              
                }


            }


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
            var cnt = dataGridView1.SelectedCells.Count;
            if (cnt <= 0) return;
            string s = "";
            var rowIdx = 0;
            List<MyCell> cells = new List<MyCell>();
            foreach (var cell in dataGridView1.SelectedCells)
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
                lbInfo.Text = cnt + " selected cell(s) copied to clipboard.";
            }
        }

        private void btnScanFix_Click(object sender, EventArgs e)
        {

            String warning = "Do you want to fix those missing/mismatched msi/msp automatically ?";
            DialogResult result = MessageBox.Show(warning, "Confirmation",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                this.Enabled = false;
                int count=FixAll();
                this.Enabled = true;
                MessageBox.Show("Done. Fixed: "+count+" items.","Fix All");
                lbInfo.Text = "Fixed: " + count + " items.";
                Logger.LogMsg("Fixed: " + count + " items.");
            }

          

        }


        private void copySelectedCellsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                CopySelectedCell();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message + " " + ex.HResult.ToString("X8"));

            }
        }
        private void ResizeIt()
        {
            if (this.Height >= 500 && this.Width >= 900)
            {
                dataGridView1.Width = this.Width - 40;
                dataGridView1.Height = this.Height - 170;
                lbInfo.Top = this.Height - 60;
                //  lbMismatched.Top = lbMissing.Top = lbOK.Top = lbInfo.Top;
               btnExport.Left =this.Width - (btnLog.Width+120);
                btnLog.Left = this.Width - 100;
            }

        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeIt();
        }
        
        public void UpdateColorForDataGridView()
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {

                if ((CacheFileStatus)row.Cells["Status"].Value == CacheFileStatus.Missing)
                {
                    row.DefaultCellStyle.BackColor = Color.OrangeRed;
                    ((DataGridViewButtonCell)row.Cells["FixIt"]).Value = "Fix It";
                }
                else if ((CacheFileStatus)row.Cells["Status"].Value == CacheFileStatus.Mismatched)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    ((DataGridViewButtonCell)row.Cells["FixIt"]).Value = "Fix It";
                }

                else if ((CacheFileStatus)row.Cells["Status"].Value == CacheFileStatus.Fixed)
                {
                    row.DefaultCellStyle.BackColor = Color.YellowGreen;
                    
                    row.Cells["FixIt"].Value = null;
                    row.Cells["FixIt"] = new DataGridViewTextBoxCell();
                }

                else
                {
                    if (row.Cells["PackageName"].Value.ToString().ToUpper().Contains(".MSP"))
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255, 240);
                    else row.DefaultCellStyle.BackColor = Color.White;

                    //Hide the Fix it buton
                    row.Cells["FixIt"].Value = null;
                    row.Cells["FixIt"] = new DataGridViewTextBoxCell();

                }
             
            }
            Logger.LogMsg("UpdateColorForDataGridView done.");
        }
        
        
        private bool CopyFile(string source, string destination)
        {
            String warning = "Do you want to copy [" + source + "] to ["+destination+"] ?";
            DialogResult result = MessageBox.Show(warning, "Confirmation",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
               
                File.Copy(source, destination, true);
                if (File.Exists(destination))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("Copy [" + source + "] to [" + destination + "] failed.", "Copy failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return false;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1)
            {
                if (this.dataGridView1.Columns[e.ColumnIndex].Name == "FixIt")//
                {
                    //   MessageBox.Show(this.dataGridView1.Rows[e.RowIndex].Cells["PackageName"].Value.ToString());

                    if ((CacheFileStatus)(this.dataGridView1.Rows[e.RowIndex].Cells["Status"].Value)  == CacheFileStatus.Missing
                         || (CacheFileStatus)(this.dataGridView1.Rows[e.RowIndex].Cells["Status"].Value) == CacheFileStatus.Mismatched)
                        {
                        int idx = e.RowIndex;
                        var rowIndexCellValue=(int)this.dataGridView1.Rows[e.RowIndex].Cells["Index"].Value;
                        myRow r = null;
                        foreach(myRow rr in myData.rows)
                        {
                            if (rr.Index == rowIndexCellValue) { r = rr; break; }
                        }

                       
                        if(r==null)
                        {
                            MessageBox.Show("Internal data error! Clicked row not found in rows!");
                            return;
                        }
                        if (r.isPatch)
                        {
                            var matchedFile = myData.FindMsp(r.ProductName,r.PackageName,r.PatchCode );
                            if (!String.IsNullOrEmpty(matchedFile))
                            {
                                string destination = Path.Combine(@"c:\WINDOWS\INSTALLER\", r.CachedMsiMsp);
                                bool copied=CopyFile(matchedFile, destination);
                                if (copied)
                                {
                                    r.Status = CacheFileStatus.Fixed;

                                    var row = this.dataGridView1.Rows[e.RowIndex];
                                    row.DefaultCellStyle.BackColor = Color.YellowGreen;

                                    row.Cells["FixIt"].Value = null;
                                    row.Cells["FixIt"] = new DataGridViewTextBoxCell();

                                    UpdateStatistics();
                                    Logger.LogMsg("[Copy Done]" + destination + "==>" + destination);

                                }
                                else
                                    Logger.LogMsg("[Copy Failed]" + destination + "==>" + destination);
                            }
                            else
                                MessageBox.Show("Missing MSP not found!\n"+r.PackageName, "File Not Found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                        }
                        else

                        { 
                            var matchedFile = myData.FindMsi(r.ProductName,r.PackageName,r.ProductCode,r.ProductVersion,r.PackageCode);
                            if (!String.IsNullOrEmpty(matchedFile))
                            {

                                string destination = Path.Combine(@"c:\WINDOWS\INSTALLER\", r.CachedMsiMsp);
                                bool copied = CopyFile(matchedFile, destination);
                                if (copied)
                                {
                                    r.Status = CacheFileStatus.Fixed;

                                    var row = this.dataGridView1.Rows[e.RowIndex];
                                    row.DefaultCellStyle.BackColor = Color.YellowGreen;

                                    row.Cells["FixIt"].Value = null;
                                    row.Cells["FixIt"] = new DataGridViewTextBoxCell();

                                    UpdateStatistics();
                                    Logger.LogMsg("[Copy Done]" + destination + "==>" + destination);
                                }
                                else
                                    Logger.LogMsg("[Copy Failed]" + destination + "==>" + destination);


                            }

                            else
                                MessageBox.Show("Missing MSI not found!\n"+r.PackageName, "File Not Found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                        }

                    } //IF CLICK "fix it"


                } //if click onte first colum
            }

        }



       private   int FixAll()
        {
            bool modifed = false;
            int fixedCount = 0;
            foreach (myRow r in myData.rows)
            {
                if(r.Status==CacheFileStatus.Mismatched || r.Status==CacheFileStatus.Missing)
                {
                    string destination = Path.Combine(@"c:\WINDOWS\INSTALLER\", r.CachedMsiMsp);

                    if (r.isPatch)
                    {
                        var matchedFile = myData.FindMsp(r.ProductName, r.PackageName,r.PatchCode);
                        if (!String.IsNullOrEmpty(matchedFile))
                        {
                            Logger.LogMsg("[Found missing MSP]" + matchedFile);
                            File.Copy(matchedFile, destination,true);
                            if (File.Exists(destination)) 
                            {
                                r.Status = CacheFileStatus.Fixed;
                                modifed = true;
                                fixedCount++;
                                Logger.LogMsg("[Copy Done]" + matchedFile + "==>" + destination);
                            }
                            else
                                Logger.LogMsg("[Copy Failed]" + matchedFile + "==>" + destination);
                        }
                        else
                            Logger.LogMsg("[Missing MSP not found]" + matchedFile );
                    }
                    else
                    {
                        var matchedFile = myData.FindMsi(r.ProductName, r.PackageName, r.ProductCode, r.ProductVersion,r.PackageCode);
                        if (!String.IsNullOrEmpty(matchedFile))
                        {
                            Logger.LogMsg("[Found missing MSI]" + matchedFile);
                            File.Copy(matchedFile, destination, true);
                            if (File.Exists(destination))
                            {
                                r.Status = CacheFileStatus.Fixed;
                                modifed = true;
                                fixedCount++;
                                Logger.LogMsg("[Copy Done]" + matchedFile + "==>" + destination);
                            }
                            else
                                Logger.LogMsg("[Copy Failed]" + matchedFile + "==>" + destination);
                        }
                        else
                            Logger.LogMsg("[Missing MSI not found]" + matchedFile);

                    }




                }




            }//foreach

            if(modifed)
            {
                UpdateColorForDataGridView();
                UpdateStatistics();
            }

            return fixedCount;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (myData.rows.Count == 0)
            {
                MessageBox.Show("Empty result.Nothing to export.", "Empty Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


         

            System.Windows.Forms.SaveFileDialog dialog = new SaveFileDialog();

            string fileNameTXT = "";
            string fileNameCSV = "";
          
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string name = Path.GetFileName(dialog.FileName);
                string shortname = Path.GetFileNameWithoutExtension(dialog.FileName);
                string path =  dialog.FileName.Replace(name,"");
                fileNameTXT =Path.Combine( path,shortname+".txt");
                fileNameCSV = Path.Combine(path,shortname  + ".csv");

            }
            else return;
           

            this.Enabled = false;
            lbInfo.Text = "Export data to "+ fileNameTXT + " as text file, may take minutes...";
            lbInfo.Refresh();
            Logger.LogMsg("Export data to " + fileNameTXT + ", may take minutes...");
            

            string result = Output.FormatListTXT<myRow>(myData.rows);
            File.WriteAllText(fileNameTXT,result);

            lbInfo.Text = "Export data to " + fileNameCSV + " as csv file, may take minutes...";
            lbInfo.Refresh();
            Logger.LogMsg("Export data to " + fileNameCSV + ", may take minutes...");


            string resultCSV = Output.FormatListCSV<myRow>(myData.rows);
            File.WriteAllText(fileNameCSV, resultCSV);




            this.Enabled = true;
            lbInfo.Text = "Done.";
            lbInfo.Refresh();
            Logger.LogMsg("Export done.");
            


        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            this.Enabled = false;
            lbInfo.Text = "Sorting and coloring...";
            lbInfo.Refresh();
            

            this.UpdateColorForDataGridView();


            this.Enabled = true;
            lbInfo.Text = "Column sorted.";
            lbInfo.Refresh();
            
        }
    }
}
