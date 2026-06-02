namespace ThreeWorkTool
{
    partial class FrmModelViewer
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
                //System.Diagnostics.Debug.WriteLine("FrmModelViewer disposed successfully!");
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.floorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnResetCamera = new System.Windows.Forms.Button();
            this.btnRToggle = new System.Windows.Forms.Button();
            this.GlControl = new OpenTK.GLControl();
            this.pnlBaseRight = new System.Windows.Forms.Panel();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuStrip1.SuspendLayout();
            this.pnlBaseRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.backgroundColorToolStripMenuItem,
            this.floorToolStripMenuItem,
            this.jointsToolStripMenuItem,
            this.polygonsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(895, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cameraControlsToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // cameraControlsToolStripMenuItem
            // 
            this.cameraControlsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.typeAToolStripMenuItem,
            this.typeBToolStripMenuItem});
            this.cameraControlsToolStripMenuItem.Name = "cameraControlsToolStripMenuItem";
            this.cameraControlsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.cameraControlsToolStripMenuItem.Text = "Camera Controls";
            // 
            // typeAToolStripMenuItem
            // 
            this.typeAToolStripMenuItem.Name = "typeAToolStripMenuItem";
            this.typeAToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.typeAToolStripMenuItem.Text = "Type A";
            // 
            // typeBToolStripMenuItem
            // 
            this.typeBToolStripMenuItem.Name = "typeBToolStripMenuItem";
            this.typeBToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.typeBToolStripMenuItem.Text = "Type B";
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(115, 20);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // floorToolStripMenuItem
            // 
            this.floorToolStripMenuItem.Checked = true;
            this.floorToolStripMenuItem.CheckOnClick = true;
            this.floorToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.floorToolStripMenuItem.Name = "floorToolStripMenuItem";
            this.floorToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.floorToolStripMenuItem.Text = "Floor";
            this.floorToolStripMenuItem.Click += new System.EventHandler(this.floorToolStripMenuItem_Click);
            // 
            // jointsToolStripMenuItem
            // 
            this.jointsToolStripMenuItem.CheckOnClick = true;
            this.jointsToolStripMenuItem.Name = "jointsToolStripMenuItem";
            this.jointsToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.jointsToolStripMenuItem.Text = "Joints";
            this.jointsToolStripMenuItem.Click += new System.EventHandler(this.jointsToolStripMenuItem_Click);
            // 
            // polygonsToolStripMenuItem
            // 
            this.polygonsToolStripMenuItem.Name = "polygonsToolStripMenuItem";
            this.polygonsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.polygonsToolStripMenuItem.Text = "Polygons";
            this.polygonsToolStripMenuItem.Click += new System.EventHandler(this.polygonsToolStripMenuItem_Click);
            // 
            // btnResetCamera
            // 
            this.btnResetCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetCamera.Location = new System.Drawing.Point(780, 0);
            this.btnResetCamera.Name = "btnResetCamera";
            this.btnResetCamera.Size = new System.Drawing.Size(115, 23);
            this.btnResetCamera.TabIndex = 1;
            this.btnResetCamera.Text = "Reset Camera";
            this.btnResetCamera.UseVisualStyleBackColor = true;
            this.btnResetCamera.Click += new System.EventHandler(this.btnResetCamera_Click);
            // 
            // btnRToggle
            // 
            this.btnRToggle.AutoSize = true;
            this.btnRToggle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRToggle.Location = new System.Drawing.Point(0, 0);
            this.btnRToggle.Name = "btnRToggle";
            this.btnRToggle.Size = new System.Drawing.Size(23, 589);
            this.btnRToggle.TabIndex = 5;
            this.btnRToggle.TabStop = false;
            this.btnRToggle.Text = "<\r\n<\r\n<\r\n<\r\n<\r\n";
            this.btnRToggle.UseVisualStyleBackColor = true;
            this.btnRToggle.Click += new System.EventHandler(this.btnRToggle_Click);
            // 
            // GlControl
            // 
            this.GlControl.BackColor = System.Drawing.Color.Black;
            this.GlControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GlControl.Location = new System.Drawing.Point(0, 24);
            this.GlControl.Name = "GlControl";
            this.GlControl.Size = new System.Drawing.Size(895, 589);
            this.GlControl.TabIndex = 6;
            this.GlControl.VSync = false;
            // 
            // pnlBaseRight
            // 
            this.pnlBaseRight.Controls.Add(this.btnRToggle);
            this.pnlBaseRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBaseRight.Location = new System.Drawing.Point(872, 24);
            this.pnlBaseRight.Name = "pnlBaseRight";
            this.pnlBaseRight.Size = new System.Drawing.Size(23, 589);
            this.pnlBaseRight.TabIndex = 7;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FrmModelViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(895, 613);
            this.Controls.Add(this.pnlBaseRight);
            this.Controls.Add(this.GlControl);
            this.Controls.Add(this.btnResetCamera);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmModelViewer";
            this.Text = "FrmModelViewercs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmModelViewer_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlBaseRight.ResumeLayout(false);
            this.pnlBaseRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem floorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem typeAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem typeBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem polygonsToolStripMenuItem;
        private System.Windows.Forms.Button btnResetCamera;
        private System.Windows.Forms.Button btnRToggle;
        private System.Windows.Forms.Panel pnlBaseRight;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        public OpenTK.GLControl GlControl;
    }
}