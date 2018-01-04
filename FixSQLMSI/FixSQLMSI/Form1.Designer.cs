namespace FixSQLMSI
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tbSQLMediaPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFolderBrowse = new System.Windows.Forms.Button();
            this.btnLog = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.FixIt = new System.Windows.Forms.DataGridViewButtonColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copySelectedCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFixAll = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.rbMissingOrMismatched = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.lbTotal = new System.Windows.Forms.Label();
            this.lbMismatched = new System.Windows.Forms.Label();
            this.lbMissing = new System.Windows.Forms.Label();
            this.lbOK = new System.Windows.Forms.Label();
            this.lbInfo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnExport = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbSQLMediaPath
            // 
            this.tbSQLMediaPath.Location = new System.Drawing.Point(24, 50);
            this.tbSQLMediaPath.Margin = new System.Windows.Forms.Padding(6);
            this.tbSQLMediaPath.Name = "tbSQLMediaPath";
            this.tbSQLMediaPath.Size = new System.Drawing.Size(1070, 31);
            this.tbSQLMediaPath.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(866, 25);
            this.label1.TabIndex = 30;
            this.label1.Text = "Please specify a folder which contains SQL RTM setup source and extracted SP/CU f" +
    "iles:";
            // 
            // btnFolderBrowse
            // 
            this.btnFolderBrowse.Location = new System.Drawing.Point(1110, 42);
            this.btnFolderBrowse.Margin = new System.Windows.Forms.Padding(6);
            this.btnFolderBrowse.Name = "btnFolderBrowse";
            this.btnFolderBrowse.Size = new System.Drawing.Size(120, 46);
            this.btnFolderBrowse.TabIndex = 32;
            this.btnFolderBrowse.Text = "Browse";
            this.btnFolderBrowse.UseVisualStyleBackColor = true;
            this.btnFolderBrowse.Click += new System.EventHandler(this.btnFolderBrowse_Click);
            // 
            // btnLog
            // 
            this.btnLog.Location = new System.Drawing.Point(1872, 22);
            this.btnLog.Margin = new System.Windows.Forms.Padding(6);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(120, 46);
            this.btnLog.TabIndex = 36;
            this.btnLog.Text = "Log";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FixIt});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Location = new System.Drawing.Point(24, 203);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1971, 1056);
            this.dataGridView1.TabIndex = 35;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            // 
            // FixIt
            // 
            this.FixIt.HeaderText = "Action";
            this.FixIt.Name = "FixIt";
            this.FixIt.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FixIt.Text = "Fix It";
            this.FixIt.Width = 78;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySelectedCellsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(308, 40);
            // 
            // copySelectedCellsToolStripMenuItem
            // 
            this.copySelectedCellsToolStripMenuItem.Name = "copySelectedCellsToolStripMenuItem";
            this.copySelectedCellsToolStripMenuItem.Size = new System.Drawing.Size(307, 36);
            this.copySelectedCellsToolStripMenuItem.Text = "Copy selected cell(s)";
            this.copySelectedCellsToolStripMenuItem.Click += new System.EventHandler(this.copySelectedCellsToolStripMenuItem_Click);
            // 
            // btnFixAll
            // 
            this.btnFixAll.Location = new System.Drawing.Point(269, 112);
            this.btnFixAll.Margin = new System.Windows.Forms.Padding(6);
            this.btnFixAll.Name = "btnFixAll";
            this.btnFixAll.Size = new System.Drawing.Size(221, 56);
            this.btnFixAll.TabIndex = 34;
            this.btnFixAll.Text = "Fix All";
            this.btnFixAll.UseVisualStyleBackColor = true;
            this.btnFixAll.Click += new System.EventHandler(this.btnScanFix_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(24, 112);
            this.btnScan.Margin = new System.Windows.Forms.Padding(6);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(197, 56);
            this.btnScan.TabIndex = 33;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // rbMissingOrMismatched
            // 
            this.rbMissingOrMismatched.AutoSize = true;
            this.rbMissingOrMismatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbMissingOrMismatched.Location = new System.Drawing.Point(147, 22);
            this.rbMissingOrMismatched.Margin = new System.Windows.Forms.Padding(6);
            this.rbMissingOrMismatched.Name = "rbMissingOrMismatched";
            this.rbMissingOrMismatched.Size = new System.Drawing.Size(384, 35);
            this.rbMissingOrMismatched.TabIndex = 9;
            this.rbMissingOrMismatched.Text = "Missing or Mismatched Only";
            this.rbMissingOrMismatched.UseVisualStyleBackColor = true;
            this.rbMissingOrMismatched.CheckedChanged += new System.EventHandler(this.rbMissingOrMismatched_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAll.Location = new System.Drawing.Point(30, 22);
            this.rbAll.Margin = new System.Windows.Forms.Padding(6);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(75, 35);
            this.rbAll.TabIndex = 8;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // lbTotal
            // 
            this.lbTotal.AutoSize = true;
            this.lbTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotal.Location = new System.Drawing.Point(17, 24);
            this.lbTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(83, 31);
            this.lbTotal.TabIndex = 48;
            this.lbTotal.Text = "Total:";
            // 
            // lbMismatched
            // 
            this.lbMismatched.AutoSize = true;
            this.lbMismatched.BackColor = System.Drawing.Color.Yellow;
            this.lbMismatched.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMismatched.Location = new System.Drawing.Point(503, 24);
            this.lbMismatched.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbMismatched.Name = "lbMismatched";
            this.lbMismatched.Size = new System.Drawing.Size(168, 31);
            this.lbMismatched.TabIndex = 47;
            this.lbMismatched.Text = "Mismatched:";
            // 
            // lbMissing
            // 
            this.lbMissing.AutoSize = true;
            this.lbMissing.BackColor = System.Drawing.Color.Red;
            this.lbMissing.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMissing.Location = new System.Drawing.Point(313, 24);
            this.lbMissing.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbMissing.Name = "lbMissing";
            this.lbMissing.Size = new System.Drawing.Size(114, 31);
            this.lbMissing.TabIndex = 46;
            this.lbMissing.Text = "Missing:";
            // 
            // lbOK
            // 
            this.lbOK.AutoSize = true;
            this.lbOK.BackColor = System.Drawing.SystemColors.Window;
            this.lbOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbOK.Location = new System.Drawing.Point(176, 24);
            this.lbOK.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbOK.Name = "lbOK";
            this.lbOK.Size = new System.Drawing.Size(61, 31);
            this.lbOK.TabIndex = 45;
            this.lbOK.Text = "OK:";
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(24, 1273);
            this.lbInfo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(18, 25);
            this.lbInfo.TabIndex = 44;
            this.lbInfo.Text = ".";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbMissingOrMismatched);
            this.groupBox1.Controls.Add(this.rbAll);
            this.groupBox1.Location = new System.Drawing.Point(1323, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(626, 69);
            this.groupBox1.TabIndex = 49;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbTotal);
            this.groupBox2.Controls.Add(this.lbOK);
            this.groupBox2.Controls.Add(this.lbMissing);
            this.groupBox2.Controls.Add(this.lbMismatched);
            this.groupBox2.Location = new System.Drawing.Point(547, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(726, 69);
            this.groupBox2.TabIndex = 50;
            this.groupBox2.TabStop = false;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(1722, 22);
            this.btnExport.Margin = new System.Windows.Forms.Padding(6);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(120, 46);
            this.btnExport.TabIndex = 51;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2025, 1312);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.tbSQLMediaPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFolderBrowse);
            this.Controls.Add(this.btnLog);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnFixAll);
            this.Controls.Add(this.btnScan);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "FixSQLMSI - Fix SQL Setup missing cached MSI/MSP files";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSQLMediaPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnFolderBrowse;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnFixAll;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.RadioButton rbMissingOrMismatched;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copySelectedCellsToolStripMenuItem;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Label lbMismatched;
        private System.Windows.Forms.Label lbMissing;
        private System.Windows.Forms.Label lbOK;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.DataGridViewButtonColumn FixIt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnExport;
    }
}

