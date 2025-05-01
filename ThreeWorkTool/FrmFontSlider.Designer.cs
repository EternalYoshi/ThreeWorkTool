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
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarNodeSpacing = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarNodeSpacing)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFontSliderClose
            // 
            this.btnFontSliderClose.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnFontSliderClose.Location = new System.Drawing.Point(376, 167);
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
            this.trackBar1.Location = new System.Drawing.Point(12, 25);
            this.trackBar1.Maximum = 38;
            this.trackBar1.Minimum = 6;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(439, 45);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.Value = 6;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // lblFontSlider
            // 
            this.lblFontSlider.AutoSize = true;
            this.lblFontSlider.Location = new System.Drawing.Point(12, 9);
            this.lblFontSlider.Name = "lblFontSlider";
            this.lblFontSlider.Size = new System.Drawing.Size(80, 13);
            this.lblFontSlider.TabIndex = 2;
            this.lblFontSlider.Text = "Font Size Slider";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Node Spacing/Size Slider";
            // 
            // trackBarNodeSpacing
            // 
            this.trackBarNodeSpacing.LargeChange = 30;
            this.trackBarNodeSpacing.Location = new System.Drawing.Point(12, 89);
            this.trackBarNodeSpacing.Maximum = 64;
            this.trackBarNodeSpacing.Minimum = 12;
            this.trackBarNodeSpacing.Name = "trackBarNodeSpacing";
            this.trackBarNodeSpacing.Size = new System.Drawing.Size(439, 45);
            this.trackBarNodeSpacing.SmallChange = 5;
            this.trackBarNodeSpacing.TabIndex = 4;
            this.trackBarNodeSpacing.TickFrequency = 4;
            this.trackBarNodeSpacing.Value = 24;
            this.trackBarNodeSpacing.Scroll += new System.EventHandler(this.trackBarNodeSpacing_Scroll);
            // 
            // FrmFontSlider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(463, 202);
            this.Controls.Add(this.trackBarNodeSpacing);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFontSlider);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.btnFontSliderClose);
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Name = "FrmFontSlider";
            this.Text = "Font Size & Node Spacing Settings";
            this.Load += new System.EventHandler(this.FrmFontSlider_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarNodeSpacing)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFontSliderClose;
        private System.Windows.Forms.Label lblFontSlider;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TrackBar trackBar1;
        public System.Windows.Forms.TrackBar trackBarNodeSpacing;
    }
}