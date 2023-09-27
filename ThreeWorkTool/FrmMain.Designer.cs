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
            this.MenuNewArchive = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.MenuRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.emptyListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.MenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuUseManifest = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuExportAllTexAsDDS = new System.Windows.Forms.ToolStripMenuItem();
            this.manifestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findAndReplaceInAllFileNamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNotesAndAdvice = new System.Windows.Forms.ToolStripMenuItem();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.txtBoxCurrentFile = new System.Windows.Forms.TextBox();
            this.pGrdMain = new System.Windows.Forms.PropertyGrid();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.TreeSource = new ThreeWorkTool.ThreeSourceTree();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pnlNew = new System.Windows.Forms.Panel();
            this.pnlAudioPlayer = new System.Windows.Forms.Panel();
            this.lblCurrentTime = new System.Windows.Forms.Label();
            this.lblSoundLength = new System.Windows.Forms.Label();
            this.trckBarAudioPlayerSeeker = new System.Windows.Forms.TrackBar();
            this.txtAudioLoopToggle = new System.Windows.Forms.CheckBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlayPause = new System.Windows.Forms.Button();
            this.txtRPList = new System.Windows.Forms.TextBox();
            this.picBoxA = new System.Windows.Forms.PictureBox();
            this.MusicTimer = new System.Windows.Forms.Timer(this.components);
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
            this.pnlAudioPlayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trckBarAudioPlayerSeeker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxA)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuEdit,
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
            this.MenuNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNewArchive});
            this.MenuNew.Name = "MenuNew";
            this.MenuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.MenuNew.Size = new System.Drawing.Size(186, 22);
            this.MenuNew.Text = "New";
            // 
            // MenuNewArchive
            // 
            this.MenuNewArchive.Name = "MenuNewArchive";
            this.MenuNewArchive.Size = new System.Drawing.Size(114, 22);
            this.MenuNewArchive.Text = "Archive";
            this.MenuNewArchive.Click += new System.EventHandler(this.MenuNewArchive_Click);
            // 
            // MenuOpen
            // 
            this.MenuOpen.Name = "MenuOpen";
            this.MenuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.MenuOpen.Size = new System.Drawing.Size(186, 22);
            this.MenuOpen.Text = "Open";
            this.MenuOpen.Click += new System.EventHandler(this.MenuOpen_Click);
            // 
            // MenuSave
            // 
            this.MenuSave.Name = "MenuSave";
            this.MenuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.MenuSave.Size = new System.Drawing.Size(186, 22);
            this.MenuSave.Text = "Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.MenuSaveAs.Size = new System.Drawing.Size(186, 22);
            this.MenuSaveAs.Text = "Save As";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuSaveAs_Click);
            // 
            // MenuClose
            // 
            this.MenuClose.Name = "MenuClose";
            this.MenuClose.Size = new System.Drawing.Size(186, 22);
            this.MenuClose.Text = "Close";
            this.MenuClose.Click += new System.EventHandler(this.MenuClose_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(183, 6);
            // 
            // MenuRecentFiles
            // 
            this.MenuRecentFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.emptyListToolStripMenuItem,
            this.toolStripSeparator1});
            this.MenuRecentFiles.Name = "MenuRecentFiles";
            this.MenuRecentFiles.Size = new System.Drawing.Size(186, 22);
            this.MenuRecentFiles.Text = "Recent Files";
            this.MenuRecentFiles.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuRecentFiles_DropDownItemClicked);
            // 
            // emptyListToolStripMenuItem
            // 
            this.emptyListToolStripMenuItem.Name = "emptyListToolStripMenuItem";
            this.emptyListToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.emptyListToolStripMenuItem.Text = "Clear List";
            this.emptyListToolStripMenuItem.Click += new System.EventHandler(this.emptyListToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // exitToolStripMenuItem2
            // 
            this.exitToolStripMenuItem2.Name = "exitToolStripMenuItem2";
            this.exitToolStripMenuItem2.Size = new System.Drawing.Size(183, 6);
            // 
            // MenuExit
            // 
            this.MenuExit.Name = "MenuExit";
            this.MenuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.MenuExit.Size = new System.Drawing.Size(186, 22);
            this.MenuExit.Text = "Exit";
            this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
            // 
            // MenuEdit
            // 
            this.MenuEdit.Enabled = false;
            this.MenuEdit.Name = "MenuEdit";
            this.MenuEdit.Size = new System.Drawing.Size(39, 20);
            this.MenuEdit.Text = "&Edit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuSettings,
            this.manifestToolStripMenuItem,
            this.findAndReplaceInAllFileNamesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // MenuSettings
            // 
            this.MenuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuUseManifest,
            this.MenuExportAllTexAsDDS});
            this.MenuSettings.Name = "MenuSettings";
            this.MenuSettings.Size = new System.Drawing.Size(255, 22);
            this.MenuSettings.Text = "Settings";
            this.MenuSettings.Click += new System.EventHandler(this.MenuSettings_Click);
            // 
            // MenuUseManifest
            // 
            this.MenuUseManifest.Name = "MenuUseManifest";
            this.MenuUseManifest.Size = new System.Drawing.Size(279, 22);
            this.MenuUseManifest.Text = "Use Manifest";
            this.MenuUseManifest.Click += new System.EventHandler(this.useManifestToolStripMenuItem_Click);
            // 
            // MenuExportAllTexAsDDS
            // 
            this.MenuExportAllTexAsDDS.Name = "MenuExportAllTexAsDDS";
            this.MenuExportAllTexAsDDS.Size = new System.Drawing.Size(279, 22);
            this.MenuExportAllTexAsDDS.Text = "Export All: Export .tex Files as .DDS files";
            this.MenuExportAllTexAsDDS.Click += new System.EventHandler(this.exportAllExporttexFilesAsDDSFilesToolStripMenuItem_Click);
            // 
            // manifestToolStripMenuItem
            // 
            this.manifestToolStripMenuItem.Name = "manifestToolStripMenuItem";
            this.manifestToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.manifestToolStripMenuItem.Text = "Manifest";
            this.manifestToolStripMenuItem.Click += new System.EventHandler(this.manifestToolStripMenuItem_Click);
            // 
            // findAndReplaceInAllFileNamesToolStripMenuItem
            // 
            this.findAndReplaceInAllFileNamesToolStripMenuItem.Name = "findAndReplaceInAllFileNamesToolStripMenuItem";
            this.findAndReplaceInAllFileNamesToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
            this.findAndReplaceInAllFileNamesToolStripMenuItem.Text = "Find and Replace In All File Names";
            this.findAndReplaceInAllFileNamesToolStripMenuItem.Click += new System.EventHandler(this.findAndReplaceInAllFileNamesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuAbout,
            this.MenuNotesAndAdvice});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // MenuAbout
            // 
            this.MenuAbout.Name = "MenuAbout";
            this.MenuAbout.Size = new System.Drawing.Size(169, 22);
            this.MenuAbout.Text = "About";
            this.MenuAbout.Click += new System.EventHandler(this.MenuAbout_Click);
            // 
            // MenuNotesAndAdvice
            // 
            this.MenuNotesAndAdvice.Name = "MenuNotesAndAdvice";
            this.MenuNotesAndAdvice.Size = new System.Drawing.Size(169, 22);
            this.MenuNotesAndAdvice.Text = "Notes And Advice";
            this.MenuNotesAndAdvice.Click += new System.EventHandler(this.MenuNotesAndAdvice_Click);
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
            this.txtBoxCurrentFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.pGrdMain.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pGrdMain_PropertyValueChanged);
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
            this.imageList1.Images.SetKeyName(17, "Rpl.png");
            this.imageList1.Images.SetKeyName(18, "M3A.png");
            this.imageList1.Images.SetKeyName(19, "CST.png");
            this.imageList1.Images.SetKeyName(20, "CHN.png");
            this.imageList1.Images.SetKeyName(21, "CCL.png");
            this.imageList1.Images.SetKeyName(22, "Bone.png");
            this.imageList1.Images.SetKeyName(23, "GEM.png");
            this.imageList1.Images.SetKeyName(24, "RIF.png");
            this.imageList1.Images.SetKeyName(25, "lsh.png");
            this.imageList1.Images.SetKeyName(26, "SLO.png");
            this.imageList1.Images.SetKeyName(27, "STQR.png");
            this.imageList1.Images.SetKeyName(28, "ANM.png");
            this.imageList1.Images.SetKeyName(29, "CBA.png");
            this.imageList1.Images.SetKeyName(30, "SBKR.png");
            this.imageList1.Images.SetKeyName(31, "SRQR.png");
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
            this.TreeSource.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeSource_AfterSelect);
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
            this.pnlNew.Controls.Add(this.pnlAudioPlayer);
            this.pnlNew.Controls.Add(this.txtRPList);
            this.pnlNew.Controls.Add(this.picBoxA);
            this.pnlNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNew.Location = new System.Drawing.Point(0, 0);
            this.pnlNew.Name = "pnlNew";
            this.pnlNew.Size = new System.Drawing.Size(626, 264);
            this.pnlNew.TabIndex = 0;
            // 
            // pnlAudioPlayer
            // 
            this.pnlAudioPlayer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAudioPlayer.Controls.Add(this.lblCurrentTime);
            this.pnlAudioPlayer.Controls.Add(this.lblSoundLength);
            this.pnlAudioPlayer.Controls.Add(this.trckBarAudioPlayerSeeker);
            this.pnlAudioPlayer.Controls.Add(this.txtAudioLoopToggle);
            this.pnlAudioPlayer.Controls.Add(this.btnStop);
            this.pnlAudioPlayer.Controls.Add(this.btnPlayPause);
            this.pnlAudioPlayer.Location = new System.Drawing.Point(453, 3);
            this.pnlAudioPlayer.Name = "pnlAudioPlayer";
            this.pnlAudioPlayer.Size = new System.Drawing.Size(170, 258);
            this.pnlAudioPlayer.TabIndex = 2;
            this.pnlAudioPlayer.Visible = false;
            // 
            // lblCurrentTime
            // 
            this.lblCurrentTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentTime.AutoSize = true;
            this.lblCurrentTime.Location = new System.Drawing.Point(44, 130);
            this.lblCurrentTime.Name = "lblCurrentTime";
            this.lblCurrentTime.Size = new System.Drawing.Size(43, 13);
            this.lblCurrentTime.TabIndex = 6;
            this.lblCurrentTime.Text = "5:34.66";
            this.lblCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCurrentTime.Visible = false;
            // 
            // lblSoundLength
            // 
            this.lblSoundLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSoundLength.AutoSize = true;
            this.lblSoundLength.Location = new System.Drawing.Point(83, 130);
            this.lblSoundLength.Name = "lblSoundLength";
            this.lblSoundLength.Size = new System.Drawing.Size(54, 13);
            this.lblSoundLength.TabIndex = 5;
            this.lblSoundLength.Text = "/00:00.00";
            this.lblSoundLength.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trckBarAudioPlayerSeeker
            // 
            this.trckBarAudioPlayerSeeker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.trckBarAudioPlayerSeeker.Location = new System.Drawing.Point(3, 98);
            this.trckBarAudioPlayerSeeker.Name = "trckBarAudioPlayerSeeker";
            this.trckBarAudioPlayerSeeker.Size = new System.Drawing.Size(164, 45);
            this.trckBarAudioPlayerSeeker.TabIndex = 4;
            this.trckBarAudioPlayerSeeker.TickFrequency = 2;
            this.trckBarAudioPlayerSeeker.Visible = false;
            // 
            // txtAudioLoopToggle
            // 
            this.txtAudioLoopToggle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.txtAudioLoopToggle.AutoSize = true;
            this.txtAudioLoopToggle.Location = new System.Drawing.Point(3, 203);
            this.txtAudioLoopToggle.Name = "txtAudioLoopToggle";
            this.txtAudioLoopToggle.Size = new System.Drawing.Size(50, 17);
            this.txtAudioLoopToggle.TabIndex = 3;
            this.txtAudioLoopToggle.Text = "Loop";
            this.txtAudioLoopToggle.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStop.Location = new System.Drawing.Point(86, 226);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlayPause
            // 
            this.btnPlayPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPlayPause.Location = new System.Drawing.Point(3, 226);
            this.btnPlayPause.Name = "btnPlayPause";
            this.btnPlayPause.Size = new System.Drawing.Size(75, 23);
            this.btnPlayPause.TabIndex = 0;
            this.btnPlayPause.Text = "Play/Pause";
            this.btnPlayPause.UseVisualStyleBackColor = true;
            this.btnPlayPause.Click += new System.EventHandler(this.btnPlayPause_Click);
            // 
            // txtRPList
            // 
            this.txtRPList.AcceptsReturn = true;
            this.txtRPList.AcceptsTab = true;
            this.txtRPList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRPList.Location = new System.Drawing.Point(4, 0);
            this.txtRPList.Multiline = true;
            this.txtRPList.Name = "txtRPList";
            this.txtRPList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRPList.Size = new System.Drawing.Size(181, 258);
            this.txtRPList.TabIndex = 1;
            this.txtRPList.Visible = false;
            this.txtRPList.TextChanged += new System.EventHandler(this.TxtRPList_TextChanged);
            this.txtRPList.Leave += new System.EventHandler(this.TxtRPList_Leave);
            // 
            // picBoxA
            // 
            this.picBoxA.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.picBoxA.Location = new System.Drawing.Point(191, 2);
            this.picBoxA.Name = "picBoxA";
            this.picBoxA.Size = new System.Drawing.Size(256, 256);
            this.picBoxA.TabIndex = 1;
            this.picBoxA.TabStop = false;
            // 
            // MusicTimer
            // 
            this.MusicTimer.Enabled = true;
            this.MusicTimer.Tick += new System.EventHandler(this.MusicTimer_Tick);
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
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(384, 256);
            this.Name = "FrmMainThree";
            this.Text = "ThreeWork Tool Not Quite V0.7";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMainThree_FormClosing);
            this.Load += new System.EventHandler(this.FrmMainThree_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmMainThree_KeyDown);
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
            this.pnlNew.PerformLayout();
            this.pnlAudioPlayer.ResumeLayout(false);
            this.pnlAudioPlayer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trckBarAudioPlayerSeeker)).EndInit();
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
        private System.Windows.Forms.ToolStripMenuItem MenuEdit;
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
        private System.Windows.Forms.ToolStripMenuItem MenuSettings;
        private System.Windows.Forms.ToolStripMenuItem MenuNotesAndAdvice;
        private System.Windows.Forms.ToolStripMenuItem emptyListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manifestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuUseManifest;
        private System.Windows.Forms.ToolStripMenuItem MenuNewArchive;
        public System.Windows.Forms.TextBox txtRPList;
        private System.Windows.Forms.ToolStripMenuItem findAndReplaceInAllFileNamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuExportAllTexAsDDS;
        private System.Windows.Forms.Panel pnlAudioPlayer;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnPlayPause;
        private System.Windows.Forms.CheckBox txtAudioLoopToggle;
        private System.Windows.Forms.TrackBar trckBarAudioPlayerSeeker;
        private System.Windows.Forms.Label lblSoundLength;
        private System.Windows.Forms.Label lblCurrentTime;
        private System.Windows.Forms.Timer MusicTimer;
    }
}

