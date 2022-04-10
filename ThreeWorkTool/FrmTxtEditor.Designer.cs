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
            this.txtFind = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.lblFind = new System.Windows.Forms.Label();
            this.btnLineJump = new System.Windows.Forms.Button();
            this.txtLineNumber = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtMSDBox
            // 
            this.txtMSDBox.Location = new System.Drawing.Point(12, 24);
            this.txtMSDBox.Name = "txtMSDBox";
            this.txtMSDBox.Size = new System.Drawing.Size(812, 574);
            this.txtMSDBox.TabIndex = 0;
            this.txtMSDBox.Text = "";
            this.txtMSDBox.WordWrap = false;
            this.txtMSDBox.TextChanged += new System.EventHandler(this.txtMSDBox_TextChanged);
            // 
            // txtFind
            // 
            this.txtFind.Location = new System.Drawing.Point(12, 3);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(180, 20);
            this.txtFind.TabIndex = 1;
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(199, 2);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 2;
            this.btnFind.Text = "Search";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
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
            // FrmTxtEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(836, 602);
            this.Controls.Add(this.txtLineNumber);
            this.Controls.Add(this.btnLineJump);
            this.Controls.Add(this.lblFind);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.txtFind);
            this.Controls.Add(this.txtMSDBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTxtEditor";
            this.Text = "MSD Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTxtEditor_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmTxtEditor_FormClosed);
            this.TextChanged += new System.EventHandler(this.FrmTxtEditor_TextChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtMSDBox;
        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lblFind;
        private System.Windows.Forms.Button btnLineJump;
        private System.Windows.Forms.TextBox txtLineNumber;
    }
}