using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;
using System.Threading;

using System.Diagnostics;
using System.Reflection;
using System.IO;
using Microsoft.Win32;

namespace ProductBrowser
{


      
    public partial class Form1 : Form
    {

        static Control lblInfo;
       
        static Control myform;

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            lblInfo = this.label1;
            myform = this;

            this.label1.Visible = false;
            this.label1.Text = "";

            Logger.SetupLog();

            Product.Init(UpdateStatus, DoneCallBack);
            treeView1.TreeViewNodeSorter = new NodeSorter();

            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + "  Version " + ver.Replace(".0.0", "");


        }

        public class NodeSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode tx = (TreeNode)x;
                TreeNode ty = (TreeNode)y;
                if (string.Compare(tx.Name, ty.Name, StringComparison.Ordinal) > 0) return 1;
                else if (string.Compare(tx.Name, ty.Name, StringComparison.Ordinal) == 0) return 0;
                else return -1;

            }
        }
        public void Start()
        {
            Thread th = new Thread(new ThreadStart(Product.Execute));
            this.menuStrip1.Enabled = false; //it will be enaled in callback function

            treeView1.Nodes.Clear();
            th.Start();
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
                string msg = "Populating TreeiView nodes...";
                Logger.LogMsg(msg);
                lblInfo.Text = msg;
                lblInfo.Refresh();


                treeView1.Nodes.Add(Product.root);

                this.menuStrip1.Enabled = true;
                lblInfo.Text = "";
                lblInfo.Refresh();
                lblInfo.Visible = false; 

                Logger.LogMsg("TreeView  populate done.");
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

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form scanf = new ScanForm();
            DialogResult r = scanf.ShowDialog();
            if (r == DialogResult.OK)
            {
                this.label1.Visible = true;
                label1.Text = "Scanning...may take minutes.";
                Start();
            }
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Logger.logFileName)) return;
            var process = new Process();
            process.StartInfo.FileName = "notepad.exe";
            process.StartInfo.Arguments = Logger.logFileName;

            process.Start();
        }

        private void exportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dialog = new SaveFileDialog();
          
            string filename = ""; 
            

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.FileName;

                if (!filename.ToLower().Contains(".txt")) filename = filename + ".txt";
                List<string> result=Product.GetAllNodesText();
                if(result.Count>0)
                {
                    File.WriteAllLines(filename, result.ToArray());
                    MessageBox.Show("File saved to:\n" + filename, "File Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {

                e.Cancel = true;

            }
        }

        private void Find()
        {
            Form search = new FindWhat();
            DialogResult r = search.ShowDialog();
            if (r == DialogResult.Yes)
            {
                myFind.findResult.node.TreeView.SelectedNode = myFind.findResult.node;
              
             //   if (myFind.findResult.isNodeText) this.treeView1.Focus();
                

            }
            else if (r == DialogResult.No)
            {
                MessageBox.Show("Key not found!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.treeView1.Focus();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            Form scanf = new ScanForm();
            DialogResult r = scanf.ShowDialog();
            if (r == DialogResult.OK)
            {
                this.label1.Visible = true;
                label1.Text = "Scanning...may take minutes.";
                Start();
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            myFind.treeViewSelectedNode = e.Node;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
                contextMenuStrip1.Tag = e.Node;
                contextMenuStrip1.Show(treeView1, e.Location);
            }

        }

        private void findToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(myFind.lastFindText))
            {

                bool isFound = myFind.Find(myFind.lastFindText);
                if (isFound)
                {
                    myFind.findResult.node.TreeView.SelectedNode = myFind.findResult.node;
                
                    if (myFind.findResult.isNodeText) this.treeView1.Focus();
                   
                }
                else
                    MessageBox.Show("Key not found!", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);


            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           ver= ver.Replace(".0.0", "");

            MessageBox.Show("Product Browser, Version " + ver + "\nA tool to list Windows Installer Products\nBy Simon Su @Microsoft, 2018.1.22", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void copyNodeNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = treeView1.SelectedNode.Text;
            if (!String.IsNullOrEmpty(text))
            {
                Clipboard.SetText(treeView1.SelectedNode.Text);
               
            }
        }

        private void copyNodeAndChildrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> result = Product.GetNodeAndChildren(treeView1.SelectedNode);
            if (result.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string s in result) sb.AppendLine(s);
                Clipboard.SetText(sb.ToString());

            }
            else Clipboard.SetText("Please select a product to copy.");
        }
    }
}
