namespace ThreeWorkTool
{
    partial class FrmLoading
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
            this.lblPlsWait = new System.Windows.Forms.Label();
            this.prgLoading = new System.Windows.Forms.ProgressBar();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.lblCurFile = new System.Windows.Forms.Label();
            this.lblCurIndex = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPlsWait
            // 
            this.lblPlsWait.AutoSize = true;
            this.lblPlsWait.Location = new System.Drawing.Point(13, 13);
            this.lblPlsWait.Name = "lblPlsWait";
            this.lblPlsWait.Size = new System.Drawing.Size(112, 13);
            this.lblPlsWait.TabIndex = 0;
            this.lblPlsWait.Text = "Correcting File Order...";
            // 
            // prgLoading
            // 
            this.prgLoading.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.prgLoading.Location = new System.Drawing.Point(12, 116);
            this.prgLoading.Name = "prgLoading";
            this.prgLoading.Size = new System.Drawing.Size(370, 23);
            this.prgLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgLoading.TabIndex = 1;
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Location = new System.Drawing.Point(13, 36);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(77, 13);
            this.lblCurrentFile.TabIndex = 2;
            this.lblCurrentFile.Text = "Now Serving...";
            this.lblCurrentFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblFileCount
            // 
            this.lblFileCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblFileCount.AutoSize = true;
            this.lblFileCount.Location = new System.Drawing.Point(180, 100);
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(33, 13);
            this.lblFileCount.TabIndex = 3;
            this.lblFileCount.Text = "/XXX";
            this.lblFileCount.Visible = false;
            // 
            // lblCurFile
            // 
            this.lblCurFile.AutoSize = true;
            this.lblCurFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCurFile.Location = new System.Drawing.Point(13, 59);
            this.lblCurFile.MaximumSize = new System.Drawing.Size(370, 0);
            this.lblCurFile.Name = "lblCurFile";
            this.lblCurFile.Size = new System.Drawing.Size(22, 13);
            this.lblCurFile.TabIndex = 4;
            this.lblCurFile.Text = ".....";
            this.lblCurFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCurIndex
            // 
            this.lblCurIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblCurIndex.AutoSize = true;
            this.lblCurIndex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCurIndex.Location = new System.Drawing.Point(150, 100);
            this.lblCurIndex.Name = "lblCurIndex";
            this.lblCurIndex.Size = new System.Drawing.Size(28, 13);
            this.lblCurIndex.TabIndex = 5;
            this.lblCurIndex.Text = "XXX";
            this.lblCurIndex.Visible = false;
            // 
            // FrmLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 151);
            this.Controls.Add(this.lblCurIndex);
            this.Controls.Add(this.lblCurFile);
            this.Controls.Add(this.lblFileCount);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.prgLoading);
            this.Controls.Add(this.lblPlsWait);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLoading";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Processing";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPlsWait;
        private System.Windows.Forms.ProgressBar prgLoading;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Label lblFileCount;
        private System.Windows.Forms.Label lblCurFile;
        private System.Windows.Forms.Label lblCurIndex;
    }
}