namespace ThreeWorkTool
{
    partial class FrmManifestEditor
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
            this.txtManifest = new System.Windows.Forms.RichTextBox();
            this.btnDontUseManifest = new System.Windows.Forms.Button();
            this.btnUseManifest = new System.Windows.Forms.Button();
            this.btnRefreshList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtManifest
            // 
            this.txtManifest.BackColor = System.Drawing.SystemColors.ControlText;
            this.txtManifest.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtManifest.ForeColor = System.Drawing.SystemColors.Window;
            this.txtManifest.Location = new System.Drawing.Point(12, 12);
            this.txtManifest.Name = "txtManifest";
            this.txtManifest.Size = new System.Drawing.Size(807, 578);
            this.txtManifest.TabIndex = 0;
            this.txtManifest.Text = "";
            // 
            // btnDontUseManifest
            // 
            this.btnDontUseManifest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDontUseManifest.Location = new System.Drawing.Point(644, 596);
            this.btnDontUseManifest.Name = "btnDontUseManifest";
            this.btnDontUseManifest.Size = new System.Drawing.Size(175, 28);
            this.btnDontUseManifest.TabIndex = 1;
            this.btnDontUseManifest.Text = "Cancel";
            this.btnDontUseManifest.UseVisualStyleBackColor = true;
            this.btnDontUseManifest.Click += new System.EventHandler(this.btnDontUseManifest_Click);
            // 
            // btnUseManifest
            // 
            this.btnUseManifest.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUseManifest.Location = new System.Drawing.Point(12, 596);
            this.btnUseManifest.Name = "btnUseManifest";
            this.btnUseManifest.Size = new System.Drawing.Size(175, 28);
            this.btnUseManifest.TabIndex = 2;
            this.btnUseManifest.Text = "OK";
            this.btnUseManifest.UseVisualStyleBackColor = true;
            this.btnUseManifest.Click += new System.EventHandler(this.btnUseManifest_Click);
            // 
            // btnRefreshList
            // 
            this.btnRefreshList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnRefreshList.Location = new System.Drawing.Point(321, 596);
            this.btnRefreshList.Name = "btnRefreshList";
            this.btnRefreshList.Size = new System.Drawing.Size(175, 28);
            this.btnRefreshList.TabIndex = 3;
            this.btnRefreshList.Text = "Refresh File List";
            this.btnRefreshList.UseVisualStyleBackColor = true;
            this.btnRefreshList.Click += new System.EventHandler(this.btnRefreshList_Click);
            // 
            // FrmManifestEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(831, 631);
            this.Controls.Add(this.btnRefreshList);
            this.Controls.Add(this.btnUseManifest);
            this.Controls.Add(this.btnDontUseManifest);
            this.Controls.Add(this.txtManifest);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmManifestEditor";
            this.Text = "Manifest Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtManifest;
        private System.Windows.Forms.Button btnDontUseManifest;
        private System.Windows.Forms.Button btnUseManifest;
        private System.Windows.Forms.Button btnRefreshList;
    }
}