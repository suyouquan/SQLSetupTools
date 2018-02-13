namespace SQLReg
{
    partial class CleanupWarning
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CleanupWarning));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnIUnderstand = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ckboxShowAgain = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(27, 33);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(1092, 615);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // btnIUnderstand
            // 
            this.btnIUnderstand.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIUnderstand.Location = new System.Drawing.Point(698, 683);
            this.btnIUnderstand.Name = "btnIUnderstand";
            this.btnIUnderstand.Size = new System.Drawing.Size(191, 66);
            this.btnIUnderstand.TabIndex = 1;
            this.btnIUnderstand.Text = "I accept";
            this.btnIUnderstand.UseVisualStyleBackColor = true;
            this.btnIUnderstand.Click += new System.EventHandler(this.btnIUnderstand_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(928, 683);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(191, 66);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ckboxShowAgain
            // 
            this.ckboxShowAgain.AutoSize = true;
            this.ckboxShowAgain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckboxShowAgain.Location = new System.Drawing.Point(27, 656);
            this.ckboxShowAgain.Name = "ckboxShowAgain";
            this.ckboxShowAgain.Size = new System.Drawing.Size(413, 35);
            this.ckboxShowAgain.TabIndex = 3;
            this.ckboxShowAgain.Text = "Don\'t show this warning again.";
            this.ckboxShowAgain.UseVisualStyleBackColor = true;
            this.ckboxShowAgain.Visible = false;
            this.ckboxShowAgain.CheckedChanged += new System.EventHandler(this.ckboxShowAgain_CheckedChanged);
            // 
            // CleanupWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 776);
            this.Controls.Add(this.ckboxShowAgain);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnIUnderstand);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CleanupWarning";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cleanup Warning";
            this.Load += new System.EventHandler(this.CleanupWarning_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnIUnderstand;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox ckboxShowAgain;
    }
}