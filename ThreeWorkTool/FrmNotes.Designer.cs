namespace ThreeWorkTool
{
    partial class FrmNotes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNotes));
            this.txtNotes = new System.Windows.Forms.RichTextBox();
            this.btnNoteOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtNotes
            // 
            this.txtNotes.BackColor = System.Drawing.SystemColors.ControlText;
            this.txtNotes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNotes.ForeColor = System.Drawing.SystemColors.Window;
            this.txtNotes.Location = new System.Drawing.Point(-1, 0);
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ReadOnly = true;
            this.txtNotes.Size = new System.Drawing.Size(655, 584);
            this.txtNotes.TabIndex = 0;
            this.txtNotes.Text = resources.GetString("txtNotes.Text");
            // 
            // btnNoteOK
            // 
            this.btnNoteOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNoteOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnNoteOK.Location = new System.Drawing.Point(513, 590);
            this.btnNoteOK.Name = "btnNoteOK";
            this.btnNoteOK.Size = new System.Drawing.Size(129, 27);
            this.btnNoteOK.TabIndex = 1;
            this.btnNoteOK.Text = "OK";
            this.btnNoteOK.UseVisualStyleBackColor = true;
            this.btnNoteOK.Click += new System.EventHandler(this.btnNoteOK_Click);
            // 
            // FrmNotes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(654, 623);
            this.Controls.Add(this.btnNoteOK);
            this.Controls.Add(this.txtNotes);
            this.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmNotes";
            this.Text = "Notes And Advice";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtNotes;
        private System.Windows.Forms.Button btnNoteOK;
    }
}