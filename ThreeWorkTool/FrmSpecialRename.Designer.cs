namespace ThreeWorkTool
{
    partial class FrmSpecialRename
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
            this.cmbSpcRename = new System.Windows.Forms.ComboBox();
            this.lvlFrmSpecialRename = new System.Windows.Forms.Label();
            this.btnSpcRenameCancel = new System.Windows.Forms.Button();
            this.btnSpcRenameOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbSpcRename
            // 
            this.cmbSpcRename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpcRename.FormattingEnabled = true;
            this.cmbSpcRename.Location = new System.Drawing.Point(12, 51);
            this.cmbSpcRename.Name = "cmbSpcRename";
            this.cmbSpcRename.Size = new System.Drawing.Size(285, 21);
            this.cmbSpcRename.TabIndex = 0;
            this.cmbSpcRename.Enter += new System.EventHandler(this.comboBox1_Enter);
            // 
            // lvlFrmSpecialRename
            // 
            this.lvlFrmSpecialRename.AutoSize = true;
            this.lvlFrmSpecialRename.ForeColor = System.Drawing.SystemColors.Control;
            this.lvlFrmSpecialRename.Location = new System.Drawing.Point(94, 19);
            this.lvlFrmSpecialRename.Name = "lvlFrmSpecialRename";
            this.lvlFrmSpecialRename.Size = new System.Drawing.Size(124, 13);
            this.lvlFrmSpecialRename.TabIndex = 1;
            this.lvlFrmSpecialRename.Text = "Choose a Material name.";
            this.lvlFrmSpecialRename.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnSpcRenameCancel
            // 
            this.btnSpcRenameCancel.Location = new System.Drawing.Point(222, 106);
            this.btnSpcRenameCancel.Name = "btnSpcRenameCancel";
            this.btnSpcRenameCancel.Size = new System.Drawing.Size(75, 23);
            this.btnSpcRenameCancel.TabIndex = 2;
            this.btnSpcRenameCancel.Text = "Cancel";
            this.btnSpcRenameCancel.UseVisualStyleBackColor = true;
            this.btnSpcRenameCancel.Click += new System.EventHandler(this.btnSpcRenameCancel_Click);
            // 
            // btnSpcRenameOK
            // 
            this.btnSpcRenameOK.Location = new System.Drawing.Point(12, 106);
            this.btnSpcRenameOK.Name = "btnSpcRenameOK";
            this.btnSpcRenameOK.Size = new System.Drawing.Size(75, 23);
            this.btnSpcRenameOK.TabIndex = 3;
            this.btnSpcRenameOK.Text = "OK";
            this.btnSpcRenameOK.UseVisualStyleBackColor = true;
            this.btnSpcRenameOK.Click += new System.EventHandler(this.btnSpcRenameOK_Click);
            // 
            // FrmSpecialRename
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(310, 141);
            this.Controls.Add(this.btnSpcRenameOK);
            this.Controls.Add(this.btnSpcRenameCancel);
            this.Controls.Add(this.lvlFrmSpecialRename);
            this.Controls.Add(this.cmbSpcRename);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSpecialRename";
            this.ShowInTaskbar = false;
            this.Text = "FrmSpecialRename";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSpcRename;
        private System.Windows.Forms.Label lvlFrmSpecialRename;
        private System.Windows.Forms.Button btnSpcRenameCancel;
        private System.Windows.Forms.Button btnSpcRenameOK;
    }
}