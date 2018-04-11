using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;

namespace ProductBrowser
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
            textBox1.Text = Product.filterString;
            if (Product.isFilterOn)
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
            if (radioButtonAll.Checked == true)
            {
                
                Product.isFilterOn = false;
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

                    Product.filterString = this.textBox1.Text.Trim();
                    Product.isFilterOn = true;
                    this.DialogResult = DialogResult.OK;
                }
            }
         
            

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
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
    }
}
