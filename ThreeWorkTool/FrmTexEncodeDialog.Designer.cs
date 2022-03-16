namespace ThreeWorkTool
{
    partial class FrmTexEncodeDialog
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
            this.lblTexC = new System.Windows.Forms.Label();
            this.txtTexConvFile = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.SplContTex = new System.Windows.Forms.SplitContainer();
            this.PicBoxTex = new System.Windows.Forms.PictureBox();
            this.grpBoxTexConv3 = new System.Windows.Forms.GroupBox();
            this.grpBoxTexConv2 = new System.Windows.Forms.GroupBox();
            this.lblTexTypeSelect = new System.Windows.Forms.Label();
            this.cmBoxTextureType = new System.Windows.Forms.ComboBox();
            this.btnTexCancel = new System.Windows.Forms.Button();
            this.btnTexOK = new System.Windows.Forms.Button();
            this.grpBoxTexConv1 = new System.Windows.Forms.GroupBox();
            this.lblPixelFormat = new System.Windows.Forms.Label();
            this.lblMips = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblPixelFormatDesc = new System.Windows.Forms.Label();
            this.lblMipsDesc = new System.Windows.Forms.Label();
            this.lblXDesc = new System.Windows.Forms.Label();
            this.lblYDesc = new System.Windows.Forms.Label();
            this.btnInvertGreen = new System.Windows.Forms.Button();
            this.btnRedAlphaSwap = new System.Windows.Forms.Button();
            this.lblMoreOptions = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SplContTex)).BeginInit();
            this.SplContTex.Panel1.SuspendLayout();
            this.SplContTex.Panel2.SuspendLayout();
            this.SplContTex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxTex)).BeginInit();
            this.grpBoxTexConv3.SuspendLayout();
            this.grpBoxTexConv1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTexC
            // 
            this.lblTexC.AutoSize = true;
            this.lblTexC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTexC.Location = new System.Drawing.Point(4, 4);
            this.lblTexC.Name = "lblTexC";
            this.lblTexC.Size = new System.Drawing.Size(78, 13);
            this.lblTexC.TabIndex = 0;
            this.lblTexC.Text = "Texture File:";
            // 
            // txtTexConvFile
            // 
            this.txtTexConvFile.Enabled = false;
            this.txtTexConvFile.Location = new System.Drawing.Point(80, 2);
            this.txtTexConvFile.Name = "txtTexConvFile";
            this.txtTexConvFile.Size = new System.Drawing.Size(361, 20);
            this.txtTexConvFile.TabIndex = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(752, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // SplContTex
            // 
            this.SplContTex.Location = new System.Drawing.Point(0, 28);
            this.SplContTex.Name = "SplContTex";
            // 
            // SplContTex.Panel1
            // 
            this.SplContTex.Panel1.Controls.Add(this.PicBoxTex);
            // 
            // SplContTex.Panel2
            // 
            this.SplContTex.Panel2.Controls.Add(this.grpBoxTexConv3);
            this.SplContTex.Panel2.Controls.Add(this.grpBoxTexConv2);
            this.SplContTex.Panel2.Controls.Add(this.lblTexTypeSelect);
            this.SplContTex.Panel2.Controls.Add(this.cmBoxTextureType);
            this.SplContTex.Panel2.Controls.Add(this.btnTexCancel);
            this.SplContTex.Panel2.Controls.Add(this.btnTexOK);
            this.SplContTex.Panel2.Controls.Add(this.grpBoxTexConv1);
            this.SplContTex.Size = new System.Drawing.Size(752, 582);
            this.SplContTex.SplitterDistance = 527;
            this.SplContTex.TabIndex = 3;
            // 
            // PicBoxTex
            // 
            this.PicBoxTex.Location = new System.Drawing.Point(3, 3);
            this.PicBoxTex.Name = "PicBoxTex";
            this.PicBoxTex.Size = new System.Drawing.Size(521, 576);
            this.PicBoxTex.TabIndex = 0;
            this.PicBoxTex.TabStop = false;
            // 
            // grpBoxTexConv3
            // 
            this.grpBoxTexConv3.Controls.Add(this.lblMoreOptions);
            this.grpBoxTexConv3.Controls.Add(this.btnRedAlphaSwap);
            this.grpBoxTexConv3.Controls.Add(this.btnInvertGreen);
            this.grpBoxTexConv3.Location = new System.Drawing.Point(13, 380);
            this.grpBoxTexConv3.Name = "grpBoxTexConv3";
            this.grpBoxTexConv3.Size = new System.Drawing.Size(196, 153);
            this.grpBoxTexConv3.TabIndex = 6;
            this.grpBoxTexConv3.TabStop = false;
            // 
            // grpBoxTexConv2
            // 
            this.grpBoxTexConv2.Location = new System.Drawing.Point(13, 179);
            this.grpBoxTexConv2.Name = "grpBoxTexConv2";
            this.grpBoxTexConv2.Size = new System.Drawing.Size(196, 194);
            this.grpBoxTexConv2.TabIndex = 5;
            this.grpBoxTexConv2.TabStop = false;
            this.grpBoxTexConv2.Text = "Type Description";
            // 
            // lblTexTypeSelect
            // 
            this.lblTexTypeSelect.AutoSize = true;
            this.lblTexTypeSelect.Location = new System.Drawing.Point(10, 135);
            this.lblTexTypeSelect.Name = "lblTexTypeSelect";
            this.lblTexTypeSelect.Size = new System.Drawing.Size(103, 13);
            this.lblTexTypeSelect.TabIndex = 4;
            this.lblTexTypeSelect.Text = "Select Texture Type";
            // 
            // cmBoxTextureType
            // 
            this.cmBoxTextureType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmBoxTextureType.FormattingEnabled = true;
            this.cmBoxTextureType.Items.AddRange(new object[] {
            "DXT1/BC1",
            "DXT5/BC3",
            "BC4_UNORM/Metalic/Specular Map",
            "BC5/Normal Map",
            "????/Toon Shader Picture",
            "LAB Color/Problematic Portrait Picture"});
            this.cmBoxTextureType.Location = new System.Drawing.Point(13, 151);
            this.cmBoxTextureType.Name = "cmBoxTextureType";
            this.cmBoxTextureType.Size = new System.Drawing.Size(196, 21);
            this.cmBoxTextureType.TabIndex = 3;
            this.cmBoxTextureType.SelectedIndexChanged += new System.EventHandler(this.cmBoxTextureType_SelectedIndexChanged);
            // 
            // btnTexCancel
            // 
            this.btnTexCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnTexCancel.Location = new System.Drawing.Point(129, 539);
            this.btnTexCancel.Name = "btnTexCancel";
            this.btnTexCancel.Size = new System.Drawing.Size(80, 30);
            this.btnTexCancel.TabIndex = 2;
            this.btnTexCancel.Text = "Cancel";
            this.btnTexCancel.UseVisualStyleBackColor = true;
            this.btnTexCancel.Click += new System.EventHandler(this.btnTexCancel_Click);
            // 
            // btnTexOK
            // 
            this.btnTexOK.Location = new System.Drawing.Point(13, 539);
            this.btnTexOK.Name = "btnTexOK";
            this.btnTexOK.Size = new System.Drawing.Size(80, 30);
            this.btnTexOK.TabIndex = 1;
            this.btnTexOK.Text = "OK";
            this.btnTexOK.UseVisualStyleBackColor = true;
            this.btnTexOK.Click += new System.EventHandler(this.btnTexOK_Click);
            // 
            // grpBoxTexConv1
            // 
            this.grpBoxTexConv1.Controls.Add(this.lblPixelFormat);
            this.grpBoxTexConv1.Controls.Add(this.lblMips);
            this.grpBoxTexConv1.Controls.Add(this.lblX);
            this.grpBoxTexConv1.Controls.Add(this.lblY);
            this.grpBoxTexConv1.Controls.Add(this.lblPixelFormatDesc);
            this.grpBoxTexConv1.Controls.Add(this.lblMipsDesc);
            this.grpBoxTexConv1.Controls.Add(this.lblXDesc);
            this.grpBoxTexConv1.Controls.Add(this.lblYDesc);
            this.grpBoxTexConv1.Location = new System.Drawing.Point(3, 3);
            this.grpBoxTexConv1.Name = "grpBoxTexConv1";
            this.grpBoxTexConv1.Size = new System.Drawing.Size(215, 118);
            this.grpBoxTexConv1.TabIndex = 0;
            this.grpBoxTexConv1.TabStop = false;
            this.grpBoxTexConv1.Text = "Data";
            // 
            // lblPixelFormat
            // 
            this.lblPixelFormat.AutoSize = true;
            this.lblPixelFormat.Location = new System.Drawing.Point(97, 95);
            this.lblPixelFormat.Name = "lblPixelFormat";
            this.lblPixelFormat.Size = new System.Drawing.Size(0, 13);
            this.lblPixelFormat.TabIndex = 7;
            // 
            // lblMips
            // 
            this.lblMips.AutoSize = true;
            this.lblMips.Location = new System.Drawing.Point(97, 70);
            this.lblMips.Name = "lblMips";
            this.lblMips.Size = new System.Drawing.Size(0, 13);
            this.lblMips.TabIndex = 6;
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(97, 45);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(0, 13);
            this.lblX.TabIndex = 5;
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(97, 20);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(0, 13);
            this.lblY.TabIndex = 4;
            // 
            // lblPixelFormatDesc
            // 
            this.lblPixelFormatDesc.AutoSize = true;
            this.lblPixelFormatDesc.Location = new System.Drawing.Point(7, 95);
            this.lblPixelFormatDesc.Name = "lblPixelFormatDesc";
            this.lblPixelFormatDesc.Size = new System.Drawing.Size(64, 13);
            this.lblPixelFormatDesc.TabIndex = 3;
            this.lblPixelFormatDesc.Text = "Pixel Format";
            // 
            // lblMipsDesc
            // 
            this.lblMipsDesc.AutoSize = true;
            this.lblMipsDesc.Location = new System.Drawing.Point(7, 70);
            this.lblMipsDesc.Name = "lblMipsDesc";
            this.lblMipsDesc.Size = new System.Drawing.Size(32, 13);
            this.lblMipsDesc.TabIndex = 2;
            this.lblMipsDesc.Text = "Mips:";
            // 
            // lblXDesc
            // 
            this.lblXDesc.AutoSize = true;
            this.lblXDesc.Location = new System.Drawing.Point(7, 45);
            this.lblXDesc.Name = "lblXDesc";
            this.lblXDesc.Size = new System.Drawing.Size(38, 13);
            this.lblXDesc.TabIndex = 1;
            this.lblXDesc.Text = "Width:";
            // 
            // lblYDesc
            // 
            this.lblYDesc.AutoSize = true;
            this.lblYDesc.Location = new System.Drawing.Point(7, 20);
            this.lblYDesc.Name = "lblYDesc";
            this.lblYDesc.Size = new System.Drawing.Size(43, 13);
            this.lblYDesc.TabIndex = 0;
            this.lblYDesc.Text = "Length:";
            // 
            // btnInvertGreen
            // 
            this.btnInvertGreen.Location = new System.Drawing.Point(0, 47);
            this.btnInvertGreen.Name = "btnInvertGreen";
            this.btnInvertGreen.Size = new System.Drawing.Size(122, 23);
            this.btnInvertGreen.TabIndex = 0;
            this.btnInvertGreen.Text = "Invert Green Channel";
            this.btnInvertGreen.UseVisualStyleBackColor = true;
            this.btnInvertGreen.Click += new System.EventHandler(this.btnInvertGreen_Click);
            // 
            // btnRedAlphaSwap
            // 
            this.btnRedAlphaSwap.Location = new System.Drawing.Point(0, 124);
            this.btnRedAlphaSwap.Name = "btnRedAlphaSwap";
            this.btnRedAlphaSwap.Size = new System.Drawing.Size(122, 23);
            this.btnRedAlphaSwap.TabIndex = 1;
            this.btnRedAlphaSwap.Text = "Red/Alpha Swap";
            this.btnRedAlphaSwap.UseVisualStyleBackColor = true;
            this.btnRedAlphaSwap.Click += new System.EventHandler(this.btnRedAlphaSwap_Click);
            // 
            // lblMoreOptions
            // 
            this.lblMoreOptions.AutoSize = true;
            this.lblMoreOptions.Location = new System.Drawing.Point(3, 16);
            this.lblMoreOptions.Name = "lblMoreOptions";
            this.lblMoreOptions.Size = new System.Drawing.Size(74, 13);
            this.lblMoreOptions.TabIndex = 3;
            this.lblMoreOptions.Text = "Quick Options";
            // 
            // FrmTexEncodeDialog
            // 
            this.AcceptButton = this.btnTexOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.CancelButton = this.btnTexCancel;
            this.ClientSize = new System.Drawing.Size(752, 609);
            this.Controls.Add(this.SplContTex);
            this.Controls.Add(this.txtTexConvFile);
            this.Controls.Add(this.lblTexC);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTexEncodeDialog";
            this.ShowInTaskbar = false;
            this.Text = "Texture Encoder";
            this.SplContTex.Panel1.ResumeLayout(false);
            this.SplContTex.Panel2.ResumeLayout(false);
            this.SplContTex.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplContTex)).EndInit();
            this.SplContTex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicBoxTex)).EndInit();
            this.grpBoxTexConv3.ResumeLayout(false);
            this.grpBoxTexConv3.PerformLayout();
            this.grpBoxTexConv1.ResumeLayout(false);
            this.grpBoxTexConv1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTexC;
        private System.Windows.Forms.TextBox txtTexConvFile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.SplitContainer SplContTex;
        private System.Windows.Forms.PictureBox PicBoxTex;
        private System.Windows.Forms.GroupBox grpBoxTexConv1;
        private System.Windows.Forms.Label lblMipsDesc;
        private System.Windows.Forms.Label lblXDesc;
        private System.Windows.Forms.Label lblYDesc;
        private System.Windows.Forms.Label lblPixelFormatDesc;
        private System.Windows.Forms.Button btnTexCancel;
        private System.Windows.Forms.Button btnTexOK;
        private System.Windows.Forms.Label lblPixelFormat;
        private System.Windows.Forms.Label lblMips;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.ComboBox cmBoxTextureType;
        private System.Windows.Forms.Label lblTexTypeSelect;
        private System.Windows.Forms.GroupBox grpBoxTexConv2;
        private System.Windows.Forms.GroupBox grpBoxTexConv3;
        private System.Windows.Forms.Label lblMoreOptions;
        private System.Windows.Forms.Button btnRedAlphaSwap;
        private System.Windows.Forms.Button btnInvertGreen;
    }
}