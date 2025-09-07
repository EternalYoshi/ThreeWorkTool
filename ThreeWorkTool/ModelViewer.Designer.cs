namespace ThreeWorkTool
{
    partial class ModelViewer
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.floorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnl3DView = new System.Windows.Forms.Panel();
            this.lblCameraCoords = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem1,
            this.floorToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // optionsToolStripMenuItem1
            // 
            this.optionsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundColorToolStripMenuItem1});
            this.optionsToolStripMenuItem1.Name = "optionsToolStripMenuItem1";
            this.optionsToolStripMenuItem1.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem1.Text = "Options";
            // 
            // backgroundColorToolStripMenuItem1
            // 
            this.backgroundColorToolStripMenuItem1.Name = "backgroundColorToolStripMenuItem1";
            this.backgroundColorToolStripMenuItem1.Size = new System.Drawing.Size(170, 22);
            this.backgroundColorToolStripMenuItem1.Text = "Background Color";
            this.backgroundColorToolStripMenuItem1.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem1_Click);
            // 
            // floorToolStripMenuItem
            // 
            this.floorToolStripMenuItem.Checked = true;
            this.floorToolStripMenuItem.CheckOnClick = true;
            this.floorToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.floorToolStripMenuItem.Name = "floorToolStripMenuItem";
            this.floorToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.floorToolStripMenuItem.Text = "Floor";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backgroundColorToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
            this.optionsToolStripMenuItem.Text = "Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // pnl3DView
            // 
            this.pnl3DView.BackColor = System.Drawing.Color.Azure;
            this.pnl3DView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl3DView.Location = new System.Drawing.Point(0, 24);
            this.pnl3DView.Name = "pnl3DView";
            this.pnl3DView.Size = new System.Drawing.Size(1008, 537);
            this.pnl3DView.TabIndex = 1;
            this.pnl3DView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnl3DView_MouseDown);
            this.pnl3DView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnl3DView_MouseMove);
            this.pnl3DView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnl3DView_MouseUp);
            this.pnl3DView.Resize += new System.EventHandler(this.pnl3DView_Resize);
            // 
            // lblCameraCoords
            // 
            this.lblCameraCoords.AutoSize = true;
            this.lblCameraCoords.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblCameraCoords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCameraCoords.ForeColor = System.Drawing.Color.Maroon;
            this.lblCameraCoords.Location = new System.Drawing.Point(877, 24);
            this.lblCameraCoords.Name = "lblCameraCoords";
            this.lblCameraCoords.Size = new System.Drawing.Size(131, 20);
            this.lblCameraCoords.TabIndex = 0;
            this.lblCameraCoords.Text = "lblCameraCoords";
            // 
            // ModelViewer
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.LightGray;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.lblCameraCoords);
            this.Controls.Add(this.pnl3DView);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimizeBox = false;
            this.Name = "ModelViewer";
            this.Text = "ModelViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ModelViewer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ModelViewer_FormClosed);
            this.Load += new System.EventHandler(this.ModelViewer_Load);
            this.SizeChanged += new System.EventHandler(this.ModelViewer_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModelViewer_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ModelViewer_KeyUp);
            this.Resize += new System.EventHandler(this.ModelViewer_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem floorToolStripMenuItem;
        private System.Windows.Forms.Panel pnl3DView;
        private System.Windows.Forms.Label lblCameraCoords;
    }
}