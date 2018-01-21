using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


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
         //   Start();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.label1.Visible = true;
            label1.Text = "Scanning...may take minutes.";
            Start();
        }
    }
}
