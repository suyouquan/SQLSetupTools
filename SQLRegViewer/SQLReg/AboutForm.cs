using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;

namespace SQLReg
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ver = "  Version " + ver.Replace(".0.0", "");

            string info = "SQL Registry Viewer " + ver;
            lblInfo.Text = info;

        }
        private void OpenURL(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
        private void label1_Click(object sender, EventArgs e)
        {
            OpenURL(((Label)sender).Text);
        }

        private void label5_Click(object sender, EventArgs e)
        {
            OpenURL(((Label)sender).Text);
        }

        private void label7_Click(object sender, EventArgs e)
        {
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenURL(((Label)sender).Text);
        }
    }
}
