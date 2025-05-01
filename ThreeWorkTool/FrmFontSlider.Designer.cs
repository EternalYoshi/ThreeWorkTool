namespace ThreeWorkTool
{
    partial class FrmFontSlider
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
            this.btnFontSliderClose = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblFontSlider = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFontSliderClose
            // 
            this.btnFontSliderClose.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnFontSliderClose.Location = new System.Drawing.Point(189, 103);
            this.btnFontSliderClose.Name = "btnFontSliderClose";
            this.btnFontSliderClose.Size = new System.Drawing.Size(75, 23);
            this.btnFontSliderClose.TabIndex = 0;
            this.btnFontSliderClose.Text = "OK";
            this.btnFontSliderClose.UseVisualStyleBackColor = true;
            this.btnFontSliderClose.Click += new System.EventHandler(this.btnFontSliderClose_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 4;
            this.trackBar1.Location = new System.Drawing.Point(12, 43);
            this.trackBar1.Maximum = 38;
            this.trackBar1.Minimum = 6;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(440, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.Value = 6;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // lblFontSlider
            // 
            this.lblFontSlider.AutoSize = true;
            this.lblFontSlider.Location = new System.Drawing.Point(13, 13);
            this.lblFontSlider.Name = "lblFontSlider";
            this.lblFontSlider.Size = new System.Drawing.Size(80, 13);
            this.lblFontSlider.TabIndex = 2;
            this.lblFontSlider.Text = "Font Size Slider";
            // 
            // FrmFontSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(465, 147);
            this.Controls.Add(this.lblFontSlider);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btnFontSliderClose);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Name = "FrmFontSlider";
            this.Text = "FrmFontSlider";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFontSliderClose;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label lblFontSlider;
    }
}