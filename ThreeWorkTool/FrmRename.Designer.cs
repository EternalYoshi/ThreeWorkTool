namespace ThreeWorkTool
{
    partial class FrmRename
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
            this.btnRnOK = new System.Windows.Forms.Button();
            this.btnRnCancel = new System.Windows.Forms.Button();
            this.txtRename = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnRnOK
            // 
            this.btnRnOK.Location = new System.Drawing.Point(13, 40);
            this.btnRnOK.Name = "btnRnOK";
            this.btnRnOK.Size = new System.Drawing.Size(75, 23);
            this.btnRnOK.TabIndex = 0;
            this.btnRnOK.Text = "OK";
            this.btnRnOK.UseVisualStyleBackColor = true;
            this.btnRnOK.Click += new System.EventHandler(this.btnROK_Click);
            // 
            // btnRnCancel
            // 
            this.btnRnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRnCancel.Location = new System.Drawing.Point(259, 40);
            this.btnRnCancel.Name = "btnRnCancel";
            this.btnRnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnRnCancel.TabIndex = 1;
            this.btnRnCancel.Text = "Cancel";
            this.btnRnCancel.UseVisualStyleBackColor = true;
            this.btnRnCancel.Click += new System.EventHandler(this.btnRCancel_Click);
            // 
            // txtRename
            // 
            this.txtRename.Location = new System.Drawing.Point(13, 13);
            this.txtRename.Name = "txtRename";
            this.txtRename.Size = new System.Drawing.Size(321, 20);
            this.txtRename.TabIndex = 2;
            // 
            // FrmRename
            // 
            this.AcceptButton = this.btnRnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CancelButton = this.btnRnCancel;
            this.ClientSize = new System.Drawing.Size(346, 69);
            this.Controls.Add(this.txtRename);
            this.Controls.Add(this.btnRnCancel);
            this.Controls.Add(this.btnRnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(362, 108);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(362, 108);
            this.Name = "FrmRename";
            this.ShowInTaskbar = false;
            this.Text = "Node Rename";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRnOK;
        private System.Windows.Forms.Button btnRnCancel;
        private System.Windows.Forms.TextBox txtRename;
    }
}