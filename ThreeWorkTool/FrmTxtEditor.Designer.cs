namespace ThreeWorkTool
{
    partial class FrmTxtEditor
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
            this.txtMSDBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtMSDBox
            // 
            this.txtMSDBox.Location = new System.Drawing.Point(12, 12);
            this.txtMSDBox.Name = "txtMSDBox";
            this.txtMSDBox.Size = new System.Drawing.Size(776, 559);
            this.txtMSDBox.TabIndex = 0;
            this.txtMSDBox.Text = "";
            this.txtMSDBox.WordWrap = false;
            // 
            // FrmTxtEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 583);
            this.Controls.Add(this.txtMSDBox);
            this.Name = "FrmTxtEditor";
            this.Text = "MSD Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtMSDBox;
    }
}