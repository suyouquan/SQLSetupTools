namespace FixMissingMSI
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.FixIt = new System.Windows.Forms.DataGridViewButtonColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copySelectedCellsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rbMissingOrMismatched = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.lbTotal = new System.Windows.Forms.Label();
            this.lbMismatched = new System.Windows.Forms.Label();
            this.lbMissing = new System.Windows.Forms.Label();
            this.lbOK = new System.Windows.Forms.Label();
            this.lbInfo = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixAllToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FixIt});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1845, 998);
            this.dataGridView1.TabIndex = 35;
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
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
            // rbMissingOrMismatched
            // 
            this.rbMissingOrMismatched.AutoSize = true;
            this.rbMissingOrMismatched.Location = new System.Drawing.Point(1160, 12);
            this.rbMissingOrMismatched.Margin = new System.Windows.Forms.Padding(6);
            this.rbMissingOrMismatched.Name = "rbMissingOrMismatched";
            this.rbMissingOrMismatched.Size = new System.Drawing.Size(314, 29);
            this.rbMissingOrMismatched.TabIndex = 9;
            this.rbMissingOrMismatched.Text = "Missing or Mismatched Only";
            this.rbMissingOrMismatched.UseVisualStyleBackColor = true;
            this.rbMissingOrMismatched.CheckedChanged += new System.EventHandler(this.rbMissingOrMismatched_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(1055, 12);
            this.rbAll.Margin = new System.Windows.Forms.Padding(6);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(67, 29);
            this.rbAll.TabIndex = 8;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            // 
            // lbTotal
            // 
            this.lbTotal.AutoSize = true;
            this.lbTotal.Location = new System.Drawing.Point(353, 6);
            this.lbTotal.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Padding = new System.Windows.Forms.Padding(6);
            this.lbTotal.Size = new System.Drawing.Size(78, 37);
            this.lbTotal.TabIndex = 48;
            this.lbTotal.Text = "Total:";
            // 
            // lbMismatched
            // 
            this.lbMismatched.AutoSize = true;
            this.lbMismatched.BackColor = System.Drawing.Color.Yellow;
            this.lbMismatched.Location = new System.Drawing.Point(767, 6);
            this.lbMismatched.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbMismatched.Name = "lbMismatched";
            this.lbMismatched.Padding = new System.Windows.Forms.Padding(6);
            this.lbMismatched.Size = new System.Drawing.Size(146, 37);
            this.lbMismatched.TabIndex = 47;
            this.lbMismatched.Text = "Mismatched:";
            // 
            // lbMissing
            // 
            this.lbMissing.AutoSize = true;
            this.lbMissing.BackColor = System.Drawing.Color.Red;
            this.lbMissing.Location = new System.Drawing.Point(613, 6);
            this.lbMissing.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbMissing.Name = "lbMissing";
            this.lbMissing.Padding = new System.Windows.Forms.Padding(6);
            this.lbMissing.Size = new System.Drawing.Size(104, 37);
            this.lbMissing.TabIndex = 46;
            this.lbMissing.Text = "Missing:";
            // 
            // lbOK
            // 
            this.lbOK.AutoSize = true;
            this.lbOK.BackColor = System.Drawing.SystemColors.Window;
            this.lbOK.Location = new System.Drawing.Point(492, 6);
            this.lbOK.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbOK.Name = "lbOK";
            this.lbOK.Padding = new System.Windows.Forms.Padding(6);
            this.lbOK.Size = new System.Drawing.Size(60, 37);
            this.lbOK.TabIndex = 45;
            this.lbOK.Text = "OK:";
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Enabled = false;
            this.lbInfo.Location = new System.Drawing.Point(357, 10);
            this.lbInfo.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(47, 25);
            this.lbInfo.TabIndex = 44;
            this.lbInfo.Text = "Info";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.findToolStripMenuItem,
            this.fixAllToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1845, 40);
            this.menuStrip1.TabIndex = 52;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scanToolStripMenuItem,
            this.toolStripSeparator1,
            this.exportToolStripMenuItem,
            this.toolStripSeparator2,
            this.logToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(64, 36);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // scanToolStripMenuItem
            // 
            this.scanToolStripMenuItem.Name = "scanToolStripMenuItem";
            this.scanToolStripMenuItem.Size = new System.Drawing.Size(181, 38);
            this.scanToolStripMenuItem.Text = "Scan";
            this.scanToolStripMenuItem.Click += new System.EventHandler(this.scanToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(181, 38);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(181, 38);
            this.logToolStripMenuItem.Text = "Log";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(178, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(181, 38);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem1,
            this.toolStripSeparator4,
            this.findNextToolStripMenuItem});
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(73, 36);
            this.findToolStripMenuItem.Text = "Find";
            // 
            // findToolStripMenuItem1
            // 
            this.findToolStripMenuItem1.Name = "findToolStripMenuItem1";
            this.findToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findToolStripMenuItem1.Size = new System.Drawing.Size(257, 38);
            this.findToolStripMenuItem1.Text = "Find";
            this.findToolStripMenuItem1.Click += new System.EventHandler(this.findToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(254, 6);
            // 
            // findNextToolStripMenuItem
            // 
            this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
            this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.findNextToolStripMenuItem.Size = new System.Drawing.Size(257, 38);
            this.findNextToolStripMenuItem.Text = "Find Next";
            this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
            // 
            // fixAllToolStripMenuItem
            // 
            this.fixAllToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fixAllToolStripMenuItem1});
            this.fixAllToolStripMenuItem.Name = "fixAllToolStripMenuItem";
            this.fixAllToolStripMenuItem.Size = new System.Drawing.Size(56, 36);
            this.fixAllToolStripMenuItem.Text = "Fix";
            // 
            // fixAllToolStripMenuItem1
            // 
            this.fixAllToolStripMenuItem1.Name = "fixAllToolStripMenuItem1";
            this.fixAllToolStripMenuItem1.Size = new System.Drawing.Size(177, 38);
            this.fixAllToolStripMenuItem1.Text = "Fix All";
            this.fixAllToolStripMenuItem1.Click += new System.EventHandler(this.fixAllToolStripMenuItem1_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.manualToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(77, 36);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // manualToolStripMenuItem
            // 
            this.manualToolStripMenuItem.Name = "manualToolStripMenuItem";
            this.manualToolStripMenuItem.Size = new System.Drawing.Size(194, 38);
            this.manualToolStripMenuItem.Text = "Manual";
            this.manualToolStripMenuItem.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(191, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(194, 38);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbMissingOrMismatched);
            this.panel1.Controls.Add(this.lbTotal);
            this.panel1.Controls.Add(this.rbAll);
            this.panel1.Controls.Add(this.lbOK);
            this.panel1.Controls.Add(this.lbMismatched);
            this.panel1.Controls.Add(this.lbMissing);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1845, 79);
            this.panel1.TabIndex = 53;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 40);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(1845, 1081);
            this.splitContainer1.SplitterDistance = 79;
            this.splitContainer1.TabIndex = 55;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1845, 1121);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "FixMissingMSI - Fix missing cached MSI/MSP files";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
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
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scanToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manualToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem fixAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixAllToolStripMenuItem1;
    }
}

