namespace FixMissingMSI
{
    partial class ScanForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanForm));
            this.tbSQLMediaPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFolderBrowse = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radioButtonFilter = new System.Windows.Forms.RadioButton();
            this.radioButtonAll = new System.Windows.Forms.RadioButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSQLMediaPath
            // 
            this.tbSQLMediaPath.Location = new System.Drawing.Point(31, 59);
            this.tbSQLMediaPath.Margin = new System.Windows.Forms.Padding(6);
            this.tbSQLMediaPath.Name = "tbSQLMediaPath";
            this.tbSQLMediaPath.Size = new System.Drawing.Size(920, 31);
            this.tbSQLMediaPath.TabIndex = 34;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(696, 25);
            this.label1.TabIndex = 33;
            this.label1.Text = "Please specify a folder which contains setup source and extracted files:";
            // 
            // btnFolderBrowse
            // 
            this.btnFolderBrowse.Location = new System.Drawing.Point(984, 46);
            this.btnFolderBrowse.Margin = new System.Windows.Forms.Padding(6);
            this.btnFolderBrowse.Name = "btnFolderBrowse";
            this.btnFolderBrowse.Size = new System.Drawing.Size(174, 58);
            this.btnFolderBrowse.TabIndex = 35;
            this.btnFolderBrowse.Text = "Browse";
            this.btnFolderBrowse.UseVisualStyleBackColor = true;
            this.btnFolderBrowse.Click += new System.EventHandler(this.btnFolderBrowse_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(647, 665);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(446, 59);
            this.btnCancel.TabIndex = 38;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(114, 665);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(446, 59);
            this.btnScan.TabIndex = 37;
            this.btnScan.Text = "Scan Now";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.radioButtonFilter);
            this.groupBox1.Controls.Add(this.radioButtonAll);
            this.groupBox1.Location = new System.Drawing.Point(31, 454);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1129, 169);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Scan Filter";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(304, 98);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(455, 31);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "SQL";
            // 
            // radioButtonFilter
            // 
            this.radioButtonFilter.AutoSize = true;
            this.radioButtonFilter.Checked = true;
            this.radioButtonFilter.Location = new System.Drawing.Point(33, 100);
            this.radioButtonFilter.Name = "radioButtonFilter";
            this.radioButtonFilter.Size = new System.Drawing.Size(269, 29);
            this.radioButtonFilter.TabIndex = 1;
            this.radioButtonFilter.TabStop = true;
            this.radioButtonFilter.Text = "Product name contains:";
            this.radioButtonFilter.UseVisualStyleBackColor = true;
            // 
            // radioButtonAll
            // 
            this.radioButtonAll.AutoSize = true;
            this.radioButtonAll.Location = new System.Drawing.Point(33, 47);
            this.radioButtonAll.Name = "radioButtonAll";
            this.radioButtonAll.Size = new System.Drawing.Size(158, 29);
            this.radioButtonAll.TabIndex = 0;
            this.radioButtonAll.Text = "All Products";
            this.radioButtonAll.UseVisualStyleBackColor = true;
            this.radioButtonAll.CheckedChanged += new System.EventHandler(this.radioButtonAll_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(31, 112);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(1129, 327);
            this.textBox2.TabIndex = 39;
            this.textBox2.Text = resources.GetString("textBox2.Text");
            // 
            // ScanForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1194, 771);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbSQLMediaPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFolderBrowse);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScanForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scan";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSQLMediaPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFolderBrowse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton radioButtonFilter;
        private System.Windows.Forms.RadioButton radioButtonAll;
        private System.Windows.Forms.TextBox textBox2;
    }
}