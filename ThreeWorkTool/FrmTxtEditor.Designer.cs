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
            this.txtFind = new System.Windows.Forms.TextBox();
            this.btnFindUp = new System.Windows.Forms.Button();
            this.lblFind = new System.Windows.Forms.Label();
            this.btnLineJump = new System.Windows.Forms.Button();
            this.txtLineNumber = new System.Windows.Forms.TextBox();
            this.txtMSDBoxV2 = new ScintillaNET.Scintilla();
            this.checkBoxCaseSensitive = new System.Windows.Forms.CheckBox();
            this.btnFindDown = new System.Windows.Forms.Button();
            this.lblMSDReminder = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(12, 3);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(180, 20);
            this.txtFind.TabIndex = 1;
            // 
            // btnFindUp
            // 
            this.btnFindUp.Location = new System.Drawing.Point(199, 2);
            this.btnFindUp.Name = "btnFindUp";
            this.btnFindUp.Size = new System.Drawing.Size(82, 23);
            this.btnFindUp.TabIndex = 2;
            this.btnFindUp.Text = "Search Up";
            this.btnFindUp.UseVisualStyleBackColor = true;
            this.btnFindUp.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // lblFind
            // 
            this.lblFind.AutoSize = true;
            this.lblFind.ForeColor = System.Drawing.Color.Snow;
            this.lblFind.Location = new System.Drawing.Point(281, 7);
            this.lblFind.Name = "lblFind";
            this.lblFind.Size = new System.Drawing.Size(0, 13);
            this.lblFind.TabIndex = 3;
            // 
            // btnLineJump
            // 
            this.btnLineJump.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLineJump.Location = new System.Drawing.Point(730, 2);
            this.btnLineJump.Name = "btnLineJump";
            this.btnLineJump.Size = new System.Drawing.Size(94, 23);
            this.btnLineJump.TabIndex = 5;
            this.btnLineJump.Text = "Jump To Line";
            this.btnLineJump.UseVisualStyleBackColor = true;
            this.btnLineJump.Click += new System.EventHandler(this.btnLineJump_Click);
            // 
            // txtLineNumber
            // 
            this.txtLineNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLineNumber.Location = new System.Drawing.Point(667, 3);
            this.txtLineNumber.Name = "txtLineNumber";
            this.txtLineNumber.Size = new System.Drawing.Size(57, 20);
            this.txtLineNumber.TabIndex = 4;
            // 
            // txtMSDBoxV2
            // 
            this.txtMSDBoxV2.AutocompleteListSelectedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.txtMSDBoxV2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.txtMSDBoxV2.LexerName = null;
            this.txtMSDBoxV2.Location = new System.Drawing.Point(12, 54);
            this.txtMSDBoxV2.Name = "txtMSDBoxV2";
            this.txtMSDBoxV2.Size = new System.Drawing.Size(812, 540);
            this.txtMSDBoxV2.TabIndex = 6;
            this.txtMSDBoxV2.TextChanged += new System.EventHandler(this.txtMSDBoxV2_TextChanged);
            this.txtMSDBoxV2.Click += new System.EventHandler(this.txtMSDBoxV2_Click);
            // 
            // checkBoxCaseSensitive
            // 
            this.checkBoxCaseSensitive.AutoSize = true;
            this.checkBoxCaseSensitive.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBoxCaseSensitive.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.checkBoxCaseSensitive.Location = new System.Drawing.Point(12, 31);
            this.checkBoxCaseSensitive.Name = "checkBoxCaseSensitive";
            this.checkBoxCaseSensitive.Size = new System.Drawing.Size(120, 21);
            this.checkBoxCaseSensitive.TabIndex = 7;
            this.checkBoxCaseSensitive.Text = "Case Sensitive";
            this.checkBoxCaseSensitive.UseVisualStyleBackColor = true;
            this.checkBoxCaseSensitive.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // btnFindDown
            // 
            this.btnFindDown.Location = new System.Drawing.Point(199, 29);
            this.btnFindDown.Name = "btnFindDown";
            this.btnFindDown.Size = new System.Drawing.Size(82, 23);
            this.btnFindDown.TabIndex = 8;
            this.btnFindDown.Text = "Search Down";
            this.btnFindDown.UseVisualStyleBackColor = true;
            this.btnFindDown.Click += new System.EventHandler(this.btnFindDown_Click);
            // 
            // lblMSDReminder
            // 
            this.lblMSDReminder.AutoSize = true;
            this.lblMSDReminder.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblMSDReminder.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.lblMSDReminder.Location = new System.Drawing.Point(287, 32);
            this.lblMSDReminder.Name = "lblMSDReminder";
            this.lblMSDReminder.Size = new System.Drawing.Size(435, 17);
            this.lblMSDReminder.TabIndex = 9;
            this.lblMSDReminder.Text = "Remember! MSD Indices internally start counting from 0 and NOT 1.";
            // 
            // FrmTxtEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(836, 602);
            this.Controls.Add(this.lblMSDReminder);
            this.Controls.Add(this.btnFindDown);
            this.Controls.Add(this.checkBoxCaseSensitive);
            this.Controls.Add(this.txtMSDBoxV2);
            this.Controls.Add(this.txtLineNumber);
            this.Controls.Add(this.btnLineJump);
            this.Controls.Add(this.lblFind);
            this.Controls.Add(this.btnFindUp);
            this.Controls.Add(this.txtFind);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTxtEditor";
            this.Text = "MSD Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTxtEditor_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmTxtEditor_FormClosed);
            this.Load += new System.EventHandler(this.FrmTxtEditor_Load);
            this.TextChanged += new System.EventHandler(this.FrmTxtEditor_TextChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnFindUp;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Button btnLineJump;
        private System.Windows.Forms.TextBox txtLineNumber;
        private ScintillaNET.Scintilla txtMSDBoxV2;
        private System.Windows.Forms.CheckBox checkBoxCaseSensitive;
        private System.Windows.Forms.Button btnFindDown;
        private System.Windows.Forms.Label lblMSDReminder;
    }
}