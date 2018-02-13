using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SQLReg
{
    public partial class CleanupWarning : Form
    {
        public CleanupWarning()
        {
            InitializeComponent();
        }

        private void CleanupWarning_Load(object sender, EventArgs e)
        {
          
        }

        private void btnIUnderstand_Click(object sender, EventArgs e)
        {
            
            this.DialogResult = DialogResult.OK;

        }

        private void ckboxShowAgain_CheckedChanged(object sender, EventArgs e)
        {
            if (ckboxShowAgain.Checked) Controller.ShowWarning = false;
            else Controller.ShowWarning = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
             this.DialogResult = DialogResult.Cancel;
        }
    }
}
