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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLoading));
            this.lblPlsWait = new System.Windows.Forms.Label();
            this.prgLoading = new System.Windows.Forms.ProgressBar();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.lblFileCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPlsWait
            // 
            resources.ApplyResources(this.lblPlsWait, "lblPlsWait");
            this.lblPlsWait.Name = "lblPlsWait";
            // 
            // prgLoading
            // 
            resources.ApplyResources(this.prgLoading, "prgLoading");
            this.prgLoading.Name = "prgLoading";
            this.prgLoading.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // lblCurrentFile
            // 
            resources.ApplyResources(this.lblCurrentFile, "lblCurrentFile");
            this.lblCurrentFile.Name = "lblCurrentFile";
            // 
            // lblFileCount
            // 
            resources.ApplyResources(this.lblFileCount, "lblFileCount");
            this.lblFileCount.Name = "lblFileCount";
            // 
            // FrmLoading
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblFileCount);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.prgLoading);
            this.Controls.Add(this.lblPlsWait);
            this.Name = "FrmLoading";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPlsWait;
        private System.Windows.Forms.ProgressBar prgLoading;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Label lblFileCount;
    }
}