namespace ThreeWorkTool
{
    partial class FrmMainThree
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMainThree));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.MenuRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.txtBoxCurrentFile = new System.Windows.Forms.TextBox();
            this.pGrdMain = new System.Windows.Forms.PropertyGrid();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TreeSource = new ThreeWorkTool.ThreeSourceTree();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pnlNew = new System.Windows.Forms.Panel();
            this.picBoxA = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.pnlNew.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxA)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(945, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNew,
            this.MenuOpen,
            this.MenuSave,
            this.MenuSaveAs,
            this.MenuClose,
            this.exitToolStripMenuItem,
            this.MenuRecentFiles,
            this.exitToolStripMenuItem2,
            this.MenuExit});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "&File";
            // 
            // MenuNew
            // 
            this.MenuNew.Name = "MenuNew";
            this.MenuNew.Size = new System.Drawing.Size(136, 22);
            this.MenuNew.Text = "New";
            // 
            // MenuOpen
            // 
            this.MenuOpen.Name = "MenuOpen";
            this.MenuOpen.Size = new System.Drawing.Size(136, 22);
            this.MenuOpen.Text = "Open";
            this.MenuOpen.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // MenuSave
            // 
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.Size = new System.Drawing.Size(136, 22);
            this.MenuSave.Text = "Save";
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.Size = new System.Drawing.Size(136, 22);
            this.MenuSaveAs.Text = "Save As";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // MenuClose
            // 
            this.MenuClose.Name = "MenuClose";
            this.MenuClose.Size = new System.Drawing.Size(136, 22);
            this.MenuClose.Text = "Close";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 6);
            // 
            // MenuRecentFiles
            // 
            this.MenuRecentFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1});
            this.MenuRecentFiles.Name = "MenuRecentFiles";
            this.MenuRecentFiles.Size = new System.Drawing.Size(136, 22);
            this.MenuRecentFiles.Text = "Recent Files";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(57, 6);
            // 
            // exitToolStripMenuItem2
            // 
            this.exitToolStripMenuItem2.Name = "exitToolStripMenuItem2";
            this.exitToolStripMenuItem2.Size = new System.Drawing.Size(133, 6);
            // 
            // MenuExit
            // 
            this.MenuExit.Name = "MenuExit";
            this.MenuExit.Size = new System.Drawing.Size(136, 22);
            this.MenuExit.Text = "Exit";
            this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // MenuAbout
            // 
            this.MenuAbout.Name = "MenuAbout";
            this.MenuAbout.Size = new System.Drawing.Size(107, 22);
            this.MenuAbout.Text = "About";
            this.MenuAbout.Click += new System.EventHandler(this.MenuAbout_Click);
            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentFile.Location = new System.Drawing.Point(320, 4);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(98, 17);
            this.lblCurrentFile.TabIndex = 1;
            this.lblCurrentFile.Text = "Current File:";
            // 
            // txtBoxCurrentFile
            // 
            this.txtBoxCurrentFile.Enabled = false;
            this.txtBoxCurrentFile.Location = new System.Drawing.Point(425, 2);
            this.txtBoxCurrentFile.Name = "txtBoxCurrentFile";
            this.txtBoxCurrentFile.Size = new System.Drawing.Size(508, 20);
            this.txtBoxCurrentFile.TabIndex = 2;
            // 
            // pGrdMain
            // 
            this.pGrdMain.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pGrdMain.CategoryForeColor = System.Drawing.SystemColors.Control;
            this.pGrdMain.CategorySplitterColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pGrdMain.CommandsDisabledLinkColor = System.Drawing.Color.Black;
            this.pGrdMain.CommandsForeColor = System.Drawing.SystemColors.Control;
            this.pGrdMain.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.pGrdMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pGrdMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pGrdMain.HelpBackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pGrdMain.HelpForeColor = System.Drawing.SystemColors.Control;
            this.pGrdMain.HelpVisible = false;
            this.pGrdMain.LineColor = System.Drawing.SystemColors.HotTrack;
            this.pGrdMain.Location = new System.Drawing.Point(0, 0);
            this.pGrdMain.Name = "pGrdMain";
            this.pGrdMain.Size = new System.Drawing.Size(626, 256);
            this.pGrdMain.TabIndex = 4;
            this.pGrdMain.ViewBackColor = System.Drawing.SystemColors.WindowText;
            this.pGrdMain.ViewForeColor = System.Drawing.SystemColors.Window;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Bitmap1.bmp");
            this.imageList1.Images.SetKeyName(1, "Arc.png");
            this.imageList1.Images.SetKeyName(2, "FOLDER.png");
            this.imageList1.Images.SetKeyName(3, "REDIRECTBASE copy.png");
            this.imageList1.Images.SetKeyName(4, "ATI.png");
            this.imageList1.Images.SetKeyName(5, "CAC.png");
            this.imageList1.Images.SetKeyName(6, "CHS.png");
            this.imageList1.Images.SetKeyName(7, "CLI.png");
            this.imageList1.Images.SetKeyName(8, "EFL.png");
            this.imageList1.Images.SetKeyName(9, "LMT.png");
            this.imageList1.Images.SetKeyName(10, "Mis.png");
            this.imageList1.Images.SetKeyName(11, "MOD.png");
            this.imageList1.Images.SetKeyName(12, "MRL.png");
            this.imageList1.Images.SetKeyName(13, "MSD.png");
            this.imageList1.Images.SetKeyName(14, "SHT.png");
            this.imageList1.Images.SetKeyName(15, "Tex.png");
            this.imageList1.Images.SetKeyName(16, "Unknown.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TreeSource);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(945, 524);
            this.splitContainer1.SplitterDistance = 315;
            this.splitContainer1.TabIndex = 8;
            // 
            // TreeSource
            // 
            this.TreeSource.archivefile = null;
            this.TreeSource.BackColor = System.Drawing.SystemColors.MenuText;
            this.TreeSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TreeSource.ForeColor = System.Drawing.SystemColors.Window;
            this.TreeSource.HideSelection = false;
            this.TreeSource.ImageIndex = 0;
            this.TreeSource.ImageList = this.imageList1;
            this.TreeSource.ItemHeight = 24;
            this.TreeSource.Location = new System.Drawing.Point(0, 0);
            this.TreeSource.Name = "TreeSource";
            this.TreeSource.SelectedImageIndex = 16;
            this.TreeSource.Size = new System.Drawing.Size(315, 524);
            this.TreeSource.TabIndex = 6;
            this.TreeSource.SelectionChanged += new System.EventHandler(this.TreeSource_SelectionChanged);
            this.TreeSource.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeSource_NodeMouseClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pGrdMain);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pnlNew);
            this.splitContainer2.Size = new System.Drawing.Size(626, 524);
            this.splitContainer2.SplitterDistance = 256;
            this.splitContainer2.TabIndex = 6;
            // 
            // pnlNew
            // 
            this.pnlNew.Controls.Add(this.picBoxA);
            this.pnlNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNew.Location = new System.Drawing.Point(0, 0);
            this.pnlNew.Name = "pnlNew";
            this.pnlNew.Size = new System.Drawing.Size(626, 264);
            this.pnlNew.TabIndex = 0;
            // 
            // picBoxA
            // 
            this.picBoxA.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.picBoxA.Location = new System.Drawing.Point(191, 2);
            this.picBoxA.Name = "picBoxA";
            this.picBoxA.Size = new System.Drawing.Size(256, 256);
            this.picBoxA.TabIndex = 1;
            this.picBoxA.TabStop = false;
            // 
            // FrmMainThree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 548);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.txtBoxCurrentFile);
            this.Controls.Add(this.lblCurrentFile);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(384, 256);
            this.Name = "FrmMainThree";
            this.Text = "ThreeWork Tool V0.1X";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMainThree_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.pnlNew.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picBoxA)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuNew;
        private System.Windows.Forms.ToolStripMenuItem MenuOpen;
        private System.Windows.Forms.ToolStripMenuItem MenuSave;
        private System.Windows.Forms.ToolStripMenuItem MenuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem MenuClose;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuRecentFiles;
        private System.Windows.Forms.ToolStripSeparator exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator exitToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem MenuExit;
        private System.Windows.Forms.ToolStripMenuItem MenuAbout;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.TextBox txtBoxCurrentFile;
        private System.Windows.Forms.PropertyGrid pGrdMain;
        public System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public ThreeSourceTree TreeSource;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel pnlNew;
        private System.Windows.Forms.PictureBox picBoxA;
    }
}

