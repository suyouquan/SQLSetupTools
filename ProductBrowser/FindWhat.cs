using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductBrowser
{
    public partial class FindWhat : Form
    {
        public FindWhat()
        {
            InitializeComponent();
            textBox1.Text = myFind.lastFindText;
        }

        private void Go()
        {
            myFind.lastFindText = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
            {

                bool isFound = myFind.Find(textBox1.Text.Trim());
                if (isFound)
                {

                    this.DialogResult = DialogResult.Yes;
                }
                else this.DialogResult = DialogResult.No;

            }

        }

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            Go();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Go();
            }

        }
    }
}
