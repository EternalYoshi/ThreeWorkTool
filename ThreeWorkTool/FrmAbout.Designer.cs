namespace ThreeWorkTool
{
    partial class FrmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            this.btnAboutOK = new System.Windows.Forms.Button();
            this.lblAboutText = new System.Windows.Forms.Label();
            this.lblURLText = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // btnAboutOK
            // 
            this.btnAboutOK.Location = new System.Drawing.Point(294, 170);
            this.btnAboutOK.Name = "btnAboutOK";
            this.btnAboutOK.Size = new System.Drawing.Size(75, 23);
            this.btnAboutOK.TabIndex = 0;
            this.btnAboutOK.Text = "OK";
            this.btnAboutOK.UseVisualStyleBackColor = true;
            this.btnAboutOK.Click += new System.EventHandler(this.btnAboutOK_Click);
            // 
            // lblAboutText
            // 
            this.lblAboutText.AutoSize = true;
            this.lblAboutText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAboutText.Location = new System.Drawing.Point(12, 9);
            this.lblAboutText.Name = "lblAboutText";
            this.lblAboutText.Size = new System.Drawing.Size(361, 136);
            this.lblAboutText.TabIndex = 1;
            this.lblAboutText.Text = resources.GetString("lblAboutText.Text");
            // 
            // lblURLText
            // 
            this.lblURLText.AutoSize = true;
            this.lblURLText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblURLText.LinkVisited = true;
            this.lblURLText.Location = new System.Drawing.Point(12, 170);
            this.lblURLText.Name = "lblURLText";
            this.lblURLText.Size = new System.Drawing.Size(262, 15);
            this.lblURLText.TabIndex = 2;
            this.lblURLText.TabStop = true;
            this.lblURLText.Text = "https://github.com/EternalYoshi/ThreeWorkTool";
            this.lblURLText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblURLText_LinkClicked);
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(381, 205);
            this.Controls.Add(this.lblURLText);
            this.Controls.Add(this.lblAboutText);
            this.Controls.Add(this.btnAboutOK);
            this.Name = "FrmAbout";
            this.Text = "About";
            this.Load += new System.EventHandler(this.FrmAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAboutOK;
        private System.Windows.Forms.Label lblAboutText;
        private System.Windows.Forms.LinkLabel lblURLText;
    }
}