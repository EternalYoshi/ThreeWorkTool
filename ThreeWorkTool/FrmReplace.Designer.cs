namespace ThreeWorkTool
{
    partial class FrmReplace
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
            this.txtReplaceFind = new System.Windows.Forms.TextBox();
            this.txtReplaceReplace = new System.Windows.Forms.TextBox();
            this.btnReplaceReplace = new System.Windows.Forms.Button();
            this.btnReplaceReplaceAll = new System.Windows.Forms.Button();
            this.btnReplaceExit = new System.Windows.Forms.Button();
            this.lblReplaceFind = new System.Windows.Forms.Label();
            this.lblReplace = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtReplaceFind
            // 
            this.txtReplaceFind.Location = new System.Drawing.Point(107, 38);
            this.txtReplaceFind.Name = "txtReplaceFind";
            this.txtReplaceFind.Size = new System.Drawing.Size(174, 20);
            this.txtReplaceFind.TabIndex = 0;
            // 
            // txtReplaceReplace
            // 
            this.txtReplaceReplace.Location = new System.Drawing.Point(107, 64);
            this.txtReplaceReplace.Name = "txtReplaceReplace";
            this.txtReplaceReplace.Size = new System.Drawing.Size(174, 20);
            this.txtReplaceReplace.TabIndex = 1;
            this.txtReplaceReplace.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // btnReplaceReplace
            // 
            this.btnReplaceReplace.Location = new System.Drawing.Point(12, 90);
            this.btnReplaceReplace.Name = "btnReplaceReplace";
            this.btnReplaceReplace.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceReplace.TabIndex = 2;
            this.btnReplaceReplace.Text = "Replace";
            this.btnReplaceReplace.UseVisualStyleBackColor = true;
            this.btnReplaceReplace.Click += new System.EventHandler(this.btnReplaceReplace_Click);
            // 
            // btnReplaceReplaceAll
            // 
            this.btnReplaceReplaceAll.Enabled = false;
            this.btnReplaceReplaceAll.Location = new System.Drawing.Point(102, 90);
            this.btnReplaceReplaceAll.Name = "btnReplaceReplaceAll";
            this.btnReplaceReplaceAll.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceReplaceAll.TabIndex = 3;
            this.btnReplaceReplaceAll.Text = "Replace All";
            this.btnReplaceReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceReplaceAll.Visible = false;
            // 
            // btnReplaceExit
            // 
            this.btnReplaceExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnReplaceExit.Location = new System.Drawing.Point(206, 90);
            this.btnReplaceExit.Name = "btnReplaceExit";
            this.btnReplaceExit.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceExit.TabIndex = 4;
            this.btnReplaceExit.Text = "Cancel";
            this.btnReplaceExit.UseVisualStyleBackColor = true;
            this.btnReplaceExit.Click += new System.EventHandler(this.btnReplaceExit_Click);
            // 
            // lblReplaceFind
            // 
            this.lblReplaceFind.AutoSize = true;
            this.lblReplaceFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaceFind.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblReplaceFind.Location = new System.Drawing.Point(12, 38);
            this.lblReplaceFind.Name = "lblReplaceFind";
            this.lblReplaceFind.Size = new System.Drawing.Size(31, 15);
            this.lblReplaceFind.TabIndex = 5;
            this.lblReplaceFind.Text = "Find";
            // 
            // lblReplace
            // 
            this.lblReplace.AutoSize = true;
            this.lblReplace.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplace.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblReplace.Location = new System.Drawing.Point(12, 64);
            this.lblReplace.Name = "lblReplace";
            this.lblReplace.Size = new System.Drawing.Size(53, 15);
            this.lblReplace.TabIndex = 6;
            this.lblReplace.Text = "Replace";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(39, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(222, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "Remember! This is case sensitive.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // FrmReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.CancelButton = this.btnReplaceExit;
            this.ClientSize = new System.Drawing.Size(293, 125);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblReplace);
            this.Controls.Add(this.lblReplaceFind);
            this.Controls.Add(this.btnReplaceExit);
            this.Controls.Add(this.btnReplaceReplaceAll);
            this.Controls.Add(this.btnReplaceReplace);
            this.Controls.Add(this.txtReplaceReplace);
            this.Controls.Add(this.txtReplaceFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmReplace";
            this.ShowInTaskbar = false;
            this.Text = "Replace";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtReplaceFind;
        private System.Windows.Forms.TextBox txtReplaceReplace;
        private System.Windows.Forms.Button btnReplaceReplace;
        private System.Windows.Forms.Button btnReplaceReplaceAll;
        private System.Windows.Forms.Button btnReplaceExit;
        private System.Windows.Forms.Label lblReplaceFind;
        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.Label label1;
    }
}