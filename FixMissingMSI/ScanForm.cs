using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;
using System.IO;

namespace FixMissingMSI
{
    public partial class ScanForm : Form
    {
        public ScanForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            textBox1.Text = myData.filterString;
            tbSQLMediaPath.Text = myData.setupSource;

            if (myData.isFilterOn)
            {
                radioButtonFilter.Checked = true;
                textBox1.Enabled = true;
            }
            else
            {
                radioButtonAll.Checked = true;
                textBox1.Enabled = false;
            }

        }
        private void btnScan_Click(object sender, EventArgs e)
        {
            string source = tbSQLMediaPath.Text.Trim();
            if (source != "")
            {
                if (!Directory.Exists(source))
                {
                    MessageBox.Show("The specified folder doesn't exist!\n" + tbSQLMediaPath.Text.Trim(), "Invalid folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //need to return
                    return;
                }
                else myData.setupSource = source;
            }
            else
            {
                DialogResult result = MessageBox.Show("Setup source folder is not specified."
                    + "\n\nYou may fail to fix missing/mismatched cached MSI/MSP files." +
                    "\n\nPlease click Yes to scan anyway. "
                    + "or click No to cancel this operation.", "Setup source folder is not specified",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    return;
                }
                else { myData.setupSource = ""; }
            }
            if (radioButtonAll.Checked == true)
            {

                myData.isFilterOn = false;
                this.DialogResult = DialogResult.OK;
            }
            else if (radioButtonFilter.Checked == true)
            {
                if (this.textBox1.Text.Trim() == "")
                {
                    MessageBox.Show("Please input something as product name filter.", "Empty string", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                    myData.filterString = this.textBox1.Text.Trim();
                    myData.isFilterOn = true;
                    this.DialogResult = DialogResult.OK;
                }
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

        private void radioButtonAll_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonAll.Checked == true)
            {

                textBox1.Enabled = false;
            }
            else if (radioButtonFilter.Checked == true)
            {
                textBox1.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
