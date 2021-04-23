//By Eternal Yoshi.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool
{
    public partial class FrmMainThree : Form
    {

        private static FrmMainThree _instance;
        public static FrmMainThree Instance { get { return _instance == null ? _instance = new FrmMainThree() : _instance; } }


        public FrmMainThree()
        {
            ThreeSourceTree TreeSource = new ThreeSourceTree();
            TreeSource.Name = "ThreeMain";
            TreeSource.Text = "ThreeMain";
            _instance = this;
            InitializeComponent();
        }

        public string[] ArcFileNameListBackup;
        public List<string> subdirs;
        public string CFile;
        public string CExt;
        public int NCount;
        public Int32 CMOA;
        public int CMOB;
        public Int32 CAOA;
        public Int32 CAOB;
        public Int32 CAOAB;
        public string OFilename;
        public string FilePath;
        public string Tempname;
        public string ExFilter;
        public FrmMainThree instance;
        //Variables in Arc stuff.
        public object headerer;
        public object pathlist;
        public object filecmp;
        public object entrylength;
        public object fcount;
        public int fcountint;
        public int tcount;
        public object Fentry;
        public int Arcsize;
        public int foldercount;
        public bool OpenFileModified;
        public List<string> ArcFileList;
        public List<string> RPLNameList;
        public static FrmRename frename;
        public static FrmTexEncodeDialog frmtexencode;
        public string RPLBackup;
        public bool FinishRPLRead;

        //This lets us use the dilogue without having to paste this within each button's function.
        OpenFileDialog OFDialog = new OpenFileDialog();

        //Same as before, but for the save file dialogue.
        SaveFileDialog SFDialog = new SaveFileDialog();

        //Export Dialogue.
        SaveFileDialog EXDialog = new SaveFileDialog();

        //Replace Dialogue.
        OpenFileDialog RPDialog = new OpenFileDialog();

        //Import Into Folder Dialogue.
        OpenFileDialog IMPDialog = new OpenFileDialog();


        #region Menu Stuffs

        public static bool OpenDX(string path)
        {

            if (String.IsNullOrEmpty(path))
                return false;

            if (!File.Exists(path))
            {
                MessageBox.Show("That file doesn't appear to exist.");
                return false;
            }

            //if (!Close()) return false;

            return true;

        }

        private void MenuOpen_Click(object sender, EventArgs e)
        {
            //This is where the alloted file extensions are chosen.
            OFDialog.Filter = "MT Framework Archive| *.arc";
            if (OFDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    NCount = 0;
                    OFilename = OFDialog.FileName;
                    FinishRPLRead = false;
                    FilePath = OFilename;
                    OpenDX(FilePath);

                    OpenFileModified = false;

                    //Fills in the Tree node.
                    CExt = Path.GetExtension(OFDialog.FileName);
                    txtBoxCurrentFile.Text = OFDialog.FileName;

                    //For Arc files. Function Has a more up to date file reading and writing method. More types will be added soon.
                    if (CExt == ".arc")
                    {
                        ArcFill();
                    }                    
                    else
                    {
                      MessageBox.Show("I cannot recognize the input file right now.");
                    }

                    picBoxA.Visible = false;

                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Unable to access the file. Maybe it's already in use by something else?", "Oh no it's an error.");
                    return;
                }

            }
        }

        private void MenuExit_Click(object sender, EventArgs e)
        {
            if (OpenFileModified == true)

            {
                Application.Exit();

            }

            else

            {
                Application.Exit();
            }
        }

        private void MenuSaveAs_Click(object sender, EventArgs e)
        {
            //This has the Save as coding.
            SFDialog.FileName = OFDialog.FileName;
            SFDialog.Filter = "MT Framework Archive| *.arc";
            if (SFDialog.FileName != "")
            {
                if (SFDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {

                        //Literally copies over the open file if not modified.
                        if (OpenFileModified == false)
                        {
                            byte[] OFBArray = File.ReadAllBytes(OFDialog.FileName);
                            using (MemoryStream stream = new MemoryStream())
                            {
                                stream.Write(OFBArray, 0, (int)OFBArray.Length);
                                {

                                }
                                File.WriteAllBytes(SFDialog.FileName, stream.ToArray());
                            }
                        }
                        else
                        {
                            try
                            {
                                using (var fs = new FileStream(SFDialog.FileName, FileMode.Create, FileAccess.Write))
                                {
                                    //Header that has the magic, version number and entry count.
                                    byte[] ArcHeader = {0x41, 0x52, 0x43, 0x00};
                                    byte[] ArcVersion = { 0x07, 0x00 };
                                    int arcentryoffset = 0x04;
                                    fs.Write(ArcHeader, 0,4);

                                    fs.Seek(0x04, SeekOrigin.Begin);
                                    fs.Write(ArcVersion, 0, ArcVersion.Length);

                                    //Goes to top node to begin iteration.
                                    TreeNode tn = FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);
                                    frename.Mainfrm.TreeSource.SelectedNode = tn;

                                    List<TreeNode> Nodes = new List<TreeNode>();
                                    frename.Mainfrm.AddChildren(Nodes, frename.Mainfrm.TreeSource.SelectedNode);

                                    //Determines where to start the compressed data storage based on amount of entries.
                                    int dataoffset = 0x8000;
                                    if (Nodes.Count < 110)
                                    {
                                        dataoffset = 0x2000;
                                    }
                                    else if (Nodes.Count < 200)
                                    {
                                        dataoffset = 0x4000;
                                    }
                                    else
                                    {
                                        dataoffset = 0x8000;
                                    }

                                    int nowcount = 0;
                                    foreach (TreeNode treno in Nodes)
                                    {
                                        if (treno.Tag as string != null && treno.Tag as string == "Folder")
                                        {}
                                        else
                                        {nowcount++;}
                                    }

                                    byte[] EntryTotal = { Convert.ToByte(nowcount) , 0x00};

                                    fs.Write(EntryTotal, 0, EntryTotal.Length);

                                    string exportname = "";
                                    string HashType = "";
                                    int ComSize = 0;
                                    int DecSize = 0;
                                    int DataEntryOffset = 0x8000;
                                    if (Nodes.Count < 110)
                                    {
                                        DataEntryOffset = 0x2000;
                                    }
                                    else if (Nodes.Count < 200)
                                    {
                                        DataEntryOffset = 0x4000;
                                    }
                                    else
                                    {
                                        DataEntryOffset = 0x8000;
                                    }


                                    ArcEntry enty = new ArcEntry();
                                    TextureEntry tenty = new TextureEntry();
                                    ResourcePathListEntry lrpenty = new ResourcePathListEntry();
                                    //This is for the filenames and everything after.
                                    foreach (TreeNode treno in Nodes)
                                    {
                                        //Saving generic files.
                                        if (treno.Tag as ArcEntry != null)
                                        {
                                            enty = treno.Tag as ArcEntry;
                                            exportname = "";

                                            exportname = treno.FullPath;
                                            int inp = (exportname.IndexOf("\\"))+1;
                                            exportname = exportname.Substring(inp, exportname.Length - inp);
                                            /*
                                            foreach (string s in enty.EntryDirs)
                                            {
                                                exportname = exportname + s + "\\";
                                            }
                                            */

                                            //exportname = exportname + enty.TrueName;


                                            int NumberChars = exportname.Length;
                                            byte[] namebuffer = Encoding.ASCII.GetBytes(exportname);
                                            int nblength = namebuffer.Length;

                                            //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                                            byte[] writenamedata = new byte[64];
                                            Array.Clear(writenamedata, 0, writenamedata.Length);


                                            for (int i = 0; i < namebuffer.Length; ++i)
                                            {
                                                writenamedata[i] = namebuffer[i];
                                            }

                                            fs.Write(writenamedata, 0, writenamedata.Length);

                                            //Gotta finish writing the data for the Entries of the arc. First the TypeHash,
                                            //then compressed size, decompressed size, and lastly starting data offset.

                                            //For the typehash.
                                            HashType = ArcEntry.TypeHashFinder(enty);
                                            byte[] HashBrown = new byte[4];
                                            HashBrown = StringToByteArray(HashType);
                                            Array.Reverse(HashBrown);
                                            if(HashBrown.Length < 4)
                                            {
                                                byte[] PartHash = new byte[] { };
                                                PartHash = HashBrown;
                                                Array.Resize(ref HashBrown, 4);
                                            }
                                            fs.Write(HashBrown, 0, HashBrown.Length);

                                            //For the compressed size.
                                            ComSize = enty.CompressedData.Length;
                                            string ComSizeHex = ComSize.ToString("X8");
                                            byte[] ComPacked = new byte[4];
                                            ComPacked = StringToByteArray(ComSizeHex);
                                            Array.Reverse(ComPacked);
                                            fs.Write(ComPacked, 0, ComPacked.Length);

                                            //For the unpacked size. No clue why all the entries "start" with 40.
                                            DecSize = enty.UncompressedData.Length;
                                            string DecSizeHex = DecSize.ToString("X8");
                                            byte[] DePacked = new byte[4];
                                            DePacked = StringToByteArray(DecSizeHex);
                                            Array.Reverse(DePacked);
                                            DePacked[3] = 0x40;
                                            fs.Write(DePacked, 0, DePacked.Length);

                                            //Starting Offset.
                                            string DataEntrySizeHex = DataEntryOffset.ToString("X8");
                                            byte[] DEOffed = new byte[4];
                                            DEOffed = StringToByteArray(DataEntrySizeHex);
                                            Array.Reverse(DEOffed);
                                            fs.Write(DEOffed, 0, DEOffed.Length);
                                            DataEntryOffset = DataEntryOffset + ComSize;
                                        }
                                        //Saving Textures.
                                        else if (treno.Tag as TextureEntry != null)
                                        {
                                            tenty = treno.Tag as TextureEntry;
                                            exportname = "";

                                            exportname = treno.FullPath;
                                            int inp = (exportname.IndexOf("\\")) + 1;
                                            exportname = exportname.Substring(inp, exportname.Length - inp);
                                            /*
                                            foreach (string s in enty.EntryDirs)
                                            {
                                                exportname = exportname + s + "\\";
                                            }
                                            */

                                            //exportname = exportname + enty.TrueName;


                                            int NumberChars = exportname.Length;
                                            byte[] namebuffer = Encoding.ASCII.GetBytes(exportname);
                                            int nblength = namebuffer.Length;

                                            //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                                            byte[] writenamedata = new byte[64];
                                            Array.Clear(writenamedata, 0, writenamedata.Length);


                                            for (int i = 0; i < namebuffer.Length; ++i)
                                            {
                                                writenamedata[i] = namebuffer[i];
                                            }

                                            fs.Write(writenamedata, 0, writenamedata.Length);

                                            //For the typehash.
                                            HashType = "241F5DEB";
                                            byte[] HashBrown = new byte[4];
                                            HashBrown = StringToByteArray(HashType);
                                            Array.Reverse(HashBrown);
                                            if (HashBrown.Length < 4)
                                            {
                                                byte[] PartHash = new byte[] { };
                                                PartHash = HashBrown;
                                                Array.Resize(ref HashBrown, 4);
                                            }
                                            fs.Write(HashBrown, 0, HashBrown.Length);

                                            //For the compressed size.
                                            ComSize = tenty.CompressedData.Length;
                                            string ComSizeHex = ComSize.ToString("X8");
                                            byte[] ComPacked = new byte[4];
                                            ComPacked = StringToByteArray(ComSizeHex);
                                            Array.Reverse(ComPacked);
                                            fs.Write(ComPacked, 0, ComPacked.Length);

                                            //For the unpacked size. No clue why all the entries "start" with 40.
                                            DecSize = tenty.UncompressedData.Length;
                                            string DecSizeHex = DecSize.ToString("X8");
                                            byte[] DePacked = new byte[4];
                                            DePacked = StringToByteArray(DecSizeHex);
                                            Array.Reverse(DePacked);
                                            DePacked[3] = 0x40;
                                            fs.Write(DePacked, 0, DePacked.Length);

                                            //Starting Offset.
                                            string DataEntrySizeHex = DataEntryOffset.ToString("X8");
                                            byte[] DEOffed = new byte[4];
                                            DEOffed = StringToByteArray(DataEntrySizeHex);
                                            Array.Reverse(DEOffed);
                                            fs.Write(DEOffed, 0, DEOffed.Length);
                                            DataEntryOffset = DataEntryOffset + ComSize;
                                        }
                                        else if (treno.Tag as ResourcePathListEntry != null)
                                        {
                                            lrpenty = treno.Tag as ResourcePathListEntry;
                                            exportname = "";

                                            exportname = treno.FullPath;
                                            int inp = (exportname.IndexOf("\\")) + 1;
                                            exportname = exportname.Substring(inp, exportname.Length - inp);

                                            int NumberChars = exportname.Length;
                                            byte[] namebuffer = Encoding.ASCII.GetBytes(exportname);
                                            int nblength = namebuffer.Length;

                                            //Space for name is 64 bytes so we make a byte array with that size and then inject the name data in it.
                                            byte[] writenamedata = new byte[64];
                                            Array.Clear(writenamedata, 0, writenamedata.Length);


                                            for (int i = 0; i < namebuffer.Length; ++i)
                                            {
                                                writenamedata[i] = namebuffer[i];
                                            }

                                            fs.Write(writenamedata, 0, writenamedata.Length);

                                            //For the typehash.
                                            HashType = "357EF6D4";
                                            byte[] HashBrown = new byte[4];
                                            HashBrown = StringToByteArray(HashType);
                                            Array.Reverse(HashBrown);
                                            if (HashBrown.Length < 4)
                                            {
                                                byte[] PartHash = new byte[] { };
                                                PartHash = HashBrown;
                                                Array.Resize(ref HashBrown, 4);
                                            }
                                            fs.Write(HashBrown, 0, HashBrown.Length);

                                            //For the compressed size.
                                            ComSize = lrpenty.CompressedData.Length;
                                            string ComSizeHex = ComSize.ToString("X8");
                                            byte[] ComPacked = new byte[4];
                                            ComPacked = StringToByteArray(ComSizeHex);
                                            Array.Reverse(ComPacked);
                                            fs.Write(ComPacked, 0, ComPacked.Length);

                                            //For the unpacked size. No clue why all the entries "start" with 40.
                                            DecSize = lrpenty.UncompressedData.Length;
                                            string DecSizeHex = DecSize.ToString("X8");
                                            byte[] DePacked = new byte[4];
                                            DePacked = StringToByteArray(DecSizeHex);
                                            Array.Reverse(DePacked);
                                            DePacked[3] = 0x40;
                                            fs.Write(DePacked, 0, DePacked.Length);

                                            //Starting Offset.
                                            string DataEntrySizeHex = DataEntryOffset.ToString("X8");
                                            byte[] DEOffed = new byte[4];
                                            DEOffed = StringToByteArray(DataEntrySizeHex);
                                            Array.Reverse(DEOffed);
                                            fs.Write(DEOffed, 0, DEOffed.Length);
                                            DataEntryOffset = DataEntryOffset + ComSize;

                                        }
                                        else
                                        { }
                                    }

                                    //This part goes to where the data offset begins and fills the in between areas with zeroes.
                                    fs.Position = 0;
                                    long CPos = fs.Seek(dataoffset, SeekOrigin.Current);

                                    foreach (TreeNode treno in Nodes)
                                    {
                                        if (treno.Tag as ArcEntry != null)
                                        {
                                            enty = treno.Tag as ArcEntry;
                                            byte[] CompData = enty.CompressedData;
                                            fs.Write(CompData, 0, CompData.Length);
                                        }
                                        else if(treno.Tag as TextureEntry != null)
                                        {
                                            tenty = treno.Tag as TextureEntry;
                                            byte[] CompData = tenty.CompressedData;
                                            fs.Write(CompData, 0, CompData.Length);
                                        }

                                        else if (treno.Tag as ResourcePathListEntry != null)
                                        {
                                            lrpenty = treno.Tag as ResourcePathListEntry;
                                            byte[] CompData = lrpenty.CompressedData;
                                            fs.Write(CompData, 0, CompData.Length);

                                        }

                                    }

                                    fs.Close();
                                    OpenFileModified = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception caught in process: {0}", ex);
                                return;
                            }
                            
                        }


                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show("Unable to save. Here's something to gander at: \n \n" + Ex.StackTrace, "Oh No");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Save What? You don't have a file open.");
            }
        }

        private void MenuAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ThreeWork Tool Alpha version 0.1.\n2021 By Eternal Yoshi", "About", MessageBoxButtons.OK);
        }

        private void MenuClose_Click(object sender, EventArgs e)
        {

            if (OpenFileModified == true)
            {
                DialogResult dlrs = MessageBox.Show("Want to save your changes to this file?", "Closing", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dlrs == DialogResult.Yes)
                {
                    MenuSaveAs_Click(sender, e);
                    picBoxA.Visible = false;
                    FlushAndClean();
                }
                if (dlrs == DialogResult.No)
                {
                    picBoxA.Visible = false;
                    FlushAndClean();
                }
                if (dlrs == DialogResult.Cancel)
                {
                    return;
                }
            }
            else
            {
                FlushAndClean();
            }
        }

        //Function for unloading all the assets from the previously open file.
        private static void FlushAndClean()
        {
            frename.Mainfrm.TreeSource.Nodes.Clear();
            frename.Mainfrm.TreeSource.SelectedNode = null;
            frename.Mainfrm.OpenFileModified = false;
            frename.Mainfrm.OFilename = null;
            frename.Mainfrm.FilePath = null;
            frename.Mainfrm.OFilename = null;
            frename.Mainfrm.txtBoxCurrentFile.Text = null;
            frename.Mainfrm.pGrdMain.SelectedObject = null;
            frename.Mainfrm.picBoxA.Image = null;
        }

        //Detects changes and asks to save them during closing.
        private void FrmMainThree_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (OpenFileModified == true)
            {
                DialogResult dlrs = MessageBox.Show("Want to save your changes to this file?", "Closing", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (dlrs == DialogResult.Yes)
                {
                    //Code to save the file goes here!
                    MenuSaveAs_Click(sender, e);
                }
                if (dlrs == DialogResult.No)
                {
                }
                if (dlrs == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }



        #endregion

        #region Tree Stuffs and Context Menus

        //Function to get TreeNode by name. Searches all over.
        private TreeNode GetNodeByName(TreeNodeCollection nodes, string searchtext)
        {
            TreeNode n_found_node = null;
            bool b_node_found = false;

            foreach (TreeNode node in nodes)
            {

                if (node.Name == searchtext)
                {
                    b_node_found = true;
                    n_found_node = node;

                    return n_found_node;
                }

                if (!b_node_found)
                {
                    n_found_node = GetNodeByName(node.Nodes, searchtext);

                    if (n_found_node != null)
                    {
                        return n_found_node;
                    }
                }
            }
            return null;
        }

        //Function to get TreeNode by name. Searches only in the current directory.
        private TreeNode GetNodeByNameForCurrentDirectoryOnly(TreeNode node, string searchtext)
        {
            foreach(TreeNode tn in node.Parent.Nodes)
            {
                if (tn.Text == TreeSource.SelectedNode.Text)
                {
                    return tn;
                }
            }
            return null;
        }

        //Function to find root node.
        private TreeNode FindRootNode(TreeNode treeNode)
        {
            while (treeNode.Parent != null)
            {
                treeNode = treeNode.Parent;
            }
            return treeNode;
        }

        //Adds Context Menu for folders.
        public static ContextMenu FolderContextAdder(TreeNode FolderNode, TreeView TreeV)
        {

            ContextMenu conmenu = new ContextMenu();

            conmenu.MenuItems.Add(new MenuItem("Rename Folder", MenuItemRenameFolder_Click));
            conmenu.MenuItems.Add(new MenuItem("Import Into Folder", MenuItemImportFileInFolder_Click));
            conmenu.MenuItems.Add("Delete Folder", MenuItemDeleteFolder_Click);

            return conmenu;

        }

        //Adds Context Menu for Texture files.
        public static ContextMenu TextureContextAdder(ArcEntryWrapper EntryNode, TreeView TreeV)
        {
            ContextMenu conmenu = new ContextMenu();

            conmenu.MenuItems.Add(new MenuItem("Export", MenuExportFile_Click));
            conmenu.MenuItems.Add(new MenuItem("Replace", MenuReplaceTexture_Click));
            conmenu.MenuItems.Add(new MenuItem("Rename", MenuItemRenameFile_Click));
            conmenu.MenuItems.Add(new MenuItem("Delete", MenuItemDeleteFile_Click));

            return conmenu;
        }
        
        //Adds Context Menu for undefined files & everything else.
        public static ContextMenu GenericFileContextAdder(ArcEntryWrapper EntryNode, TreeView TreeV)
        {
            ContextMenu conmenu = new ContextMenu();

            conmenu.MenuItems.Add(new MenuItem("Export", MenuExportFile_Click));
            conmenu.MenuItems.Add(new MenuItem("Replace", MenuReplaceFile_Click));
            conmenu.MenuItems.Add(new MenuItem("Rename", MenuItemRenameFile_Click));
            conmenu.MenuItems.Add(new MenuItem("Delete", MenuItemDeleteFile_Click));

            return conmenu;
        }

        private static void MenuExportFile_Click(Object sender, System.EventArgs e)
        {
            //Gets the Data from the SelectedNode's Tag and checks the data type so it can export with the correct filter.
            SaveFileDialog EXDialog = new SaveFileDialog();
            var tag = frename.Mainfrm.TreeSource.SelectedNode.Tag;

            string extension = tag.GetType().ToString();

            switch (extension)
            {
                //Textures.
                case "ThreeWorkTool.Resources.Wrappers.TextureEntry":
                case "ThreeWorkTool.Resources.Wrappers.TexEntryWrapper":
                    TextureEntry Tentry = new TextureEntry();
                    if (tag is TextureEntry)
                    {
                        Tentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as TextureEntry;
                        EXDialog.Filter = ExportFilters.GetFilter(Tentry.FileExt);
                        EXDialog.FileName = Tentry.FileName;
                    }
                    if (EXDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportFileWriter.TexEntryWriter(EXDialog.FileName, Tentry);
                    }
                    
                    //Writes to log file.
                    using (StreamWriter sw = File.AppendText("Log.txt"))
                    {
                        sw.WriteLine("Exported a file: " + frename.Mainfrm.TreeSource.SelectedNode.Name + " at " + EXDialog.FileName + "\n");
                    }
                    break;



                //Normal Entries inside Arc File.
                case "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper":
                case "ThreeWorkTool.Resources.Archives.ArcEntry":
                    ArcEntry Aentry = new ArcEntry();
                    if (tag is ArcEntry)
                    {

                        Aentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcEntry;
                        EXDialog.Filter = ExportFilters.GetFilter(Aentry.FileExt);
                    }
                    EXDialog.FileName = Aentry.FileName;

                    if (EXDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportFileWriter.ArcEntryWriter(EXDialog.FileName, Aentry);
                    }

                    //Writes to log file.
                    using (StreamWriter sw = File.AppendText("Log.txt"))
                    {
                        sw.WriteLine("Exported a file: " + frename.Mainfrm.TreeSource.SelectedNode.Name + " at " + EXDialog.FileName + "\n");
                    }
                    break;

                case "ThreeWorkTool.Resources.Wrappers.ResourcePathListEntry":
                    ResourcePathListEntry RPLentry = new ResourcePathListEntry();
                    if (tag is ResourcePathListEntry)
                    {

                        RPLentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ResourcePathListEntry;
                        EXDialog.Filter = ExportFilters.GetFilter(RPLentry.FileExt);
                    }
                    EXDialog.FileName = RPLentry.FileName + RPLentry.FileExt;

                    if (EXDialog.ShowDialog() == DialogResult.OK)
                    {
                        ExportFileWriter.RPListEntryWriter(EXDialog.FileName, RPLentry);
                    }

                    //Writes to log file.
                    using (StreamWriter sw = File.AppendText("Log.txt"))
                    {
                        sw.WriteLine("Exported a Resource Path List Entry:" + frename.Mainfrm.TreeSource.SelectedNode.Name + " at " + EXDialog.FileName + "\n");
                    }
                    break;

                default:
                    break;
            }



        }

        private static void MenuReplaceFile_Click(Object sender, System.EventArgs e)
        {
            ArcEntry Aentry = new ArcEntry();
            OpenFileDialog RPDialog = new OpenFileDialog();
            var tag = frename.Mainfrm.TreeSource.SelectedNode.Tag;
            if (tag is TextureEntry)
            {
                Aentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcEntry;
                RPDialog.Filter = ExportFilters.GetFilter(Aentry.FileExt);
                if (RPDialog.ShowDialog() == DialogResult.OK)
                {
                    string helper = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();

                    frename.Mainfrm.TreeSource.BeginUpdate();

                    switch (helper)
                    {
                        case "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper":
                            ArcEntryWrapper NewWrapper = new ArcEntryWrapper();
                            ArcEntryWrapper OldWrapper = new ArcEntryWrapper();

                            OldWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            string oldname = OldWrapper.Name;
                            ArcEntry Oldaent = new ArcEntry();
                            ArcEntry Newaent = new ArcEntry();
                            Oldaent = OldWrapper.entryfile as ArcEntry;
                            string[] paths = Oldaent.EntryDirs;
                            NewWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            int index = frename.Mainfrm.TreeSource.SelectedNode.Index;
                            NewWrapper.Tag = ArcEntry.ReplaceEntry(frename.Mainfrm.TreeSource, NewWrapper, RPDialog.FileName);
                            NewWrapper.ContextMenu = GenericFileContextAdder(NewWrapper, frename.Mainfrm.TreeSource);
                            frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);
                            //Takes the path data from the old node and slaps it on the new node.
                            Newaent = NewWrapper.entryfile as ArcEntry;
                            Newaent.EntryDirs = paths;
                            NewWrapper.entryfile = Newaent;

                            frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                            //Pathing.
                            foreach (string Folder in paths)
                            {
                                if (!frename.Mainfrm.TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                                {
                                    TreeNode folder = new TreeNode();
                                    folder.Name = Folder;
                                    folder.Tag = Folder;
                                    folder.Text = Folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(folder);
                                    frename.Mainfrm.TreeSource.SelectedNode = folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.ImageIndex = 2;
                                    frename.Mainfrm.TreeSource.SelectedNode.SelectedImageIndex = 2;
                                }
                                else
                                {
                                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.GetNodeByName(frename.Mainfrm.TreeSource.SelectedNode.Nodes, Folder);
                                }
                            }



                            //Removes the node and inserts the new one.
                            //TreeNode node = 
                            //frename.Mainfrm.TreeSource.SelectedNode.Remove();
                            //frename.Mainfrm.TreeSource.Nodes.Add(NewWrapper);

                            frename.Mainfrm.TreeSource.SelectedNode = NewWrapper;

                            break;

                        default:
                            break;
                    }


                    frename.Mainfrm.OpenFileModified = true;
                    frename.Mainfrm.TreeSource.SelectedNode.GetType();

                    string type = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                    frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                    frename.Mainfrm.TreeSource.EndUpdate();

                }

            }
            else if(tag is ResourcePathListEntry)
            {
                ResourcePathListEntry RPListEntry = new ResourcePathListEntry();
                RPListEntry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ResourcePathListEntry;
                RPDialog.Filter = ExportFilters.GetFilter(RPListEntry.FileExt);

                if (RPDialog.ShowDialog() == DialogResult.OK)
                {
                    string helper = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();

                    frename.Mainfrm.TreeSource.BeginUpdate();

                    switch (helper)
                    {
                        case "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper":
                            ArcEntryWrapper NewWrapper = new ArcEntryWrapper();
                            ArcEntryWrapper OldWrapper = new ArcEntryWrapper();

                            OldWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            string oldname = OldWrapper.Name;
                            ResourcePathListEntry Oldaent = new ResourcePathListEntry();
                            ResourcePathListEntry Newaent = new ResourcePathListEntry();
                            Oldaent = OldWrapper.entryfile as ResourcePathListEntry;
                            string[] paths = Oldaent.EntryDirs;
                            NewWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            int index = frename.Mainfrm.TreeSource.SelectedNode.Index;
                            NewWrapper.Tag = ResourcePathListEntry.ReplaceRPL(frename.Mainfrm.TreeSource, NewWrapper, RPDialog.FileName);
                            NewWrapper.ContextMenu = GenericFileContextAdder(NewWrapper, frename.Mainfrm.TreeSource);
                            frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);
                            //Takes the path data from the old node and slaps it on the new node.
                            Newaent = NewWrapper.entryfile as ResourcePathListEntry;
                            Newaent.EntryDirs = paths;
                            NewWrapper.entryfile = Newaent;

                            frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                            //Reloads the replaced file data in the text box.
                            frename.Mainfrm.txtRPList = ResourcePathListEntry.LoadRPLInTextBox(frename.Mainfrm.txtRPList, Newaent);
                            frename.Mainfrm.RPLBackup = frename.Mainfrm.txtRPList.Text;

                            //Pathing.
                            foreach (string Folder in paths)
                            {
                                if (!frename.Mainfrm.TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                                {
                                    TreeNode folder = new TreeNode();
                                    folder.Name = Folder;
                                    folder.Tag = Folder;
                                    folder.Text = Folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(folder);
                                    frename.Mainfrm.TreeSource.SelectedNode = folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.ImageIndex = 2;
                                    frename.Mainfrm.TreeSource.SelectedNode.SelectedImageIndex = 2;
                                }
                                else
                                {
                                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.GetNodeByName(frename.Mainfrm.TreeSource.SelectedNode.Nodes, Folder);
                                }
                            }



                            //Removes the node and inserts the new one.
                            //TreeNode node = 
                            //frename.Mainfrm.TreeSource.SelectedNode.Remove();
                            //frename.Mainfrm.TreeSource.Nodes.Add(NewWrapper);

                            frename.Mainfrm.TreeSource.SelectedNode = NewWrapper;

                            break;

                        default:
                            break;
                    }


                    frename.Mainfrm.OpenFileModified = true;
                    frename.Mainfrm.TreeSource.SelectedNode.GetType();

                    string type = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                    frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                    frename.Mainfrm.TreeSource.EndUpdate();

                }


            }
            else
            {
                Aentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcEntry;
                RPDialog.Filter = ExportFilters.GetFilter(Aentry.FileExt);
                if (RPDialog.ShowDialog() == DialogResult.OK)
                {
                    string helper = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();

                    frename.Mainfrm.TreeSource.BeginUpdate();

                    switch (helper)
                    {
                        case "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper":
                            ArcEntryWrapper NewWrapper = new ArcEntryWrapper();
                            ArcEntryWrapper OldWrapper = new ArcEntryWrapper();

                            OldWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            string oldname = OldWrapper.Name;
                            ArcEntry Oldaent = new ArcEntry();
                            ArcEntry Newaent = new ArcEntry();
                            Oldaent = OldWrapper.entryfile as ArcEntry;
                            string[] paths = Oldaent.EntryDirs;
                            NewWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            int index = frename.Mainfrm.TreeSource.SelectedNode.Index;
                            NewWrapper.Tag = ArcEntry.ReplaceEntry(frename.Mainfrm.TreeSource, NewWrapper, RPDialog.FileName);
                            NewWrapper.ContextMenu = GenericFileContextAdder(NewWrapper, frename.Mainfrm.TreeSource);
                            frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);
                            //Takes the path data from the old node and slaps it on the new node.
                            Newaent = NewWrapper.entryfile as ArcEntry;
                            Newaent.EntryDirs = paths;
                            NewWrapper.entryfile = Newaent;

                            frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                            //Pathing.
                            foreach (string Folder in paths)
                            {
                                if (!frename.Mainfrm.TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                                {
                                    TreeNode folder = new TreeNode();
                                    folder.Name = Folder;
                                    folder.Tag = Folder;
                                    folder.Text = Folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(folder);
                                    frename.Mainfrm.TreeSource.SelectedNode = folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.ImageIndex = 2;
                                    frename.Mainfrm.TreeSource.SelectedNode.SelectedImageIndex = 2;
                                }
                                else
                                {
                                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.GetNodeByName(frename.Mainfrm.TreeSource.SelectedNode.Nodes, Folder);
                                }
                            }



                            //Removes the node and inserts the new one.
                            //TreeNode node = 
                            //frename.Mainfrm.TreeSource.SelectedNode.Remove();
                            //frename.Mainfrm.TreeSource.Nodes.Add(NewWrapper);

                            frename.Mainfrm.TreeSource.SelectedNode = NewWrapper;

                            break;

                        default:
                            break;
                    }


                    frename.Mainfrm.OpenFileModified = true;
                    frename.Mainfrm.TreeSource.SelectedNode.GetType();

                    string type = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                    frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                    frename.Mainfrm.TreeSource.EndUpdate();

                }

            }

            //Writes to log file.
            using (StreamWriter sw = File.AppendText("Log.txt"))
            {
                sw.WriteLine("Replaced a file: " + frename.Mainfrm.FilePath + "\nCurrent File List:\n");
                sw.WriteLine("===============================================================================================================");
                int entrycount = 0;
                frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);
                sw.WriteLine("Current file Count: " + entrycount);
                sw.WriteLine("===============================================================================================================");
            }


        }

        private static void MenuReplaceTexture_Click(Object sender, System.EventArgs e)
        {
            //Gotta rewrite this to incorporate DDS Textures.

            TextureEntry Tentry = new TextureEntry();
            OpenFileDialog RPDialog = new OpenFileDialog();
            var tag = frename.Mainfrm.TreeSource.SelectedNode.Tag;
            if (tag is TextureEntry)
            {
                Tentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as TextureEntry;
                RPDialog.Filter = ExportFilters.GetFilter("ReplaceTexture");
                if (RPDialog.ShowDialog() == DialogResult.OK)
                {
                    string helper = Path.GetExtension(RPDialog.FileName);

                    frename.Mainfrm.TreeSource.BeginUpdate();

                    switch (helper)
                    {
                        case ".tex":
                            ArcEntryWrapper NewWrapper = new ArcEntryWrapper();
                            ArcEntryWrapper OldWrapper = new ArcEntryWrapper();

                            OldWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            string oldname = OldWrapper.Name;
                            TextureEntry Oldaent = new TextureEntry();
                            TextureEntry Newaent = new TextureEntry();
                            Oldaent = OldWrapper.entryfile as TextureEntry;
                            string[] paths = Oldaent.EntryDirs;
                            NewWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            int index = frename.Mainfrm.TreeSource.SelectedNode.Index;
                            NewWrapper.Tag = TextureEntry.ReplaceTextureEntry(frename.Mainfrm.TreeSource, NewWrapper, RPDialog.FileName);
                            NewWrapper.ContextMenu = TextureContextAdder(NewWrapper, frename.Mainfrm.TreeSource);
                            frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);
                            //Takes the path data from the old node and slaps it on the new node.
                            Newaent = NewWrapper.entryfile as TextureEntry;
                            Newaent.EntryDirs = paths;
                            NewWrapper.entryfile = Newaent;

                            frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                            //Pathing.
                            foreach (string Folder in paths)
                            {
                                if (!frename.Mainfrm.TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                                {
                                    TreeNode folder = new TreeNode();
                                    folder.Name = Folder;
                                    folder.Tag = Folder;
                                    folder.Text = Folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(folder);
                                    frename.Mainfrm.TreeSource.SelectedNode = folder;
                                    frename.Mainfrm.TreeSource.SelectedNode.ImageIndex = 2;
                                    frename.Mainfrm.TreeSource.SelectedNode.SelectedImageIndex = 2;
                                }
                                else
                                {
                                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.GetNodeByName(frename.Mainfrm.TreeSource.SelectedNode.Nodes, Folder);
                                }
                            }



                            //Removes the node and inserts the new one.
                            //TreeNode node = 
                            //frename.Mainfrm.TreeSource.SelectedNode.Remove();
                            //frename.Mainfrm.TreeSource.Nodes.Add(NewWrapper);

                            frename.Mainfrm.TreeSource.SelectedNode = NewWrapper;

                            break;

                        //DDS imports.
                        case ".dds":
                        case ".DDS":
                            {
                                //Creates and Spawns the Texture Encoder Dialog.
                                FrmTexEncodeDialog frmtexencode = FrmTexEncodeDialog.LoadDDSData(RPDialog.FileName, RPDialog);
                                frmtexencode.IsReplacing = true;
                                frmtexencode.ShowDialog();

                                if (frmtexencode.DialogResult == DialogResult.OK)
                                {
                                    ArcEntryWrapper NewWrapperDDS = new ArcEntryWrapper();
                                    ArcEntryWrapper OldWrapperDDS = new ArcEntryWrapper();

                                    OldWrapperDDS = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                                    string oldnameDDS = OldWrapperDDS.Name;
                                    TextureEntry OldaentDDS = new TextureEntry();
                                    TextureEntry NewaentDDS = new TextureEntry();
                                    OldaentDDS = OldWrapperDDS.entryfile as TextureEntry;
                                    string[] pathsDDS = OldaentDDS.EntryDirs;
                                    NewWrapperDDS = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                                    int indexDDS = frename.Mainfrm.TreeSource.SelectedNode.Index;
                                    NewWrapperDDS.Tag = TextureEntry.ReplaceTextureFromDDS(frename.Mainfrm.TreeSource, NewWrapperDDS, RPDialog.FileName, frmtexencode, frmtexencode.TexData);
                                    NewWrapperDDS.ContextMenu = TextureContextAdder(NewWrapperDDS, frename.Mainfrm.TreeSource);
                                    frename.Mainfrm.IconSetter(NewWrapperDDS, NewWrapperDDS.FileExt);
                                    //Takes the path data from the old node and slaps it on the new node.
                                    NewaentDDS = NewWrapperDDS.entryfile as TextureEntry;
                                    NewaentDDS.EntryDirs = pathsDDS;
                                    NewWrapperDDS.entryfile = NewaentDDS;

                                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                                    //Pathing.
                                    foreach (string Folder in pathsDDS)
                                    {
                                        if (!frename.Mainfrm.TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                                        {
                                            TreeNode folder = new TreeNode();
                                            folder.Name = Folder;
                                            folder.Tag = Folder;
                                            folder.Text = Folder;
                                            frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(folder);
                                            frename.Mainfrm.TreeSource.SelectedNode = folder;
                                            frename.Mainfrm.TreeSource.SelectedNode.ImageIndex = 2;
                                            frename.Mainfrm.TreeSource.SelectedNode.SelectedImageIndex = 2;
                                        }
                                        else
                                        {
                                            frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.GetNodeByName(frename.Mainfrm.TreeSource.SelectedNode.Nodes, Folder);
                                        }
                                    }

                                    frename.Mainfrm.TreeSource.SelectedNode = NewWrapperDDS;


                                }


                                break;
                            }

                        default:
                            break;
                    }


                    frename.Mainfrm.OpenFileModified = true;
                    frename.Mainfrm.TreeSource.SelectedNode.GetType();

                    string type = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                    frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                    frename.Mainfrm.TreeSource.EndUpdate();

                    //Writes to log file.
                    using (StreamWriter sw = File.AppendText("Log.txt"))
                    {
                        sw.WriteLine("Replaced a file: " + frename.Mainfrm.FilePath + "\nCurrent File List:\n");
                        sw.WriteLine("===============================================================================================================");
                        int entrycount = 0;
                        frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);
                        sw.WriteLine("Current file Count: " + entrycount);
                        sw.WriteLine("===============================================================================================================");
                    }

                    frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;
                    TextureEntry txentry = new TextureEntry();
                    txentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as TextureEntry;
                    frename.Mainfrm.picBoxA.Visible = true;
                    Bitmap bmx = BitmapBuilderDX(txentry.OutMaps, txentry, frename.Mainfrm.picBoxA);
                    if (bmx == null)
                    {
                        frename.Mainfrm.picBoxA.Image = frename.Mainfrm.picBoxA.ErrorImage;
                    }
                    else
                    {
                        ImageRescaler(bmx, frename.Mainfrm.picBoxA, txentry);
                    }

                }

            }




        }

        private static void MenuItemRenameFile_Click(Object sender, System.EventArgs e)
        {

            FrmRename frn = new FrmRename();
            //frename = frn;
            frn = frename;
            frn.ShowItItem();
            //frn.Show();

        }

        private static void MenuItemDeleteFile_Click(Object sender, System.EventArgs e)
        {

            DialogResult DelResult = MessageBox.Show("Are you sure you want to do this? This cannot be undone!", "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (DelResult == DialogResult.Yes)
            {
                //Writes to log file.
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Deleted a file: " + frename.Mainfrm.TreeSource.SelectedNode + "\nCurrent File List:\n");
                    sw.WriteLine("===============================================================================================================");
                    frename.Mainfrm.TreeSource.SelectedNode.Remove();
                    frename.Mainfrm.OpenFileModified = true;
                    int entrycount = 0;
                    frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);

                    TreeNode rootnode = new TreeNode();
                    rootnode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);
                    TreeNode selectednode = new TreeNode();
                    selectednode = frename.Mainfrm.TreeSource.SelectedNode;
                    frename.Mainfrm.TreeSource.SelectedNode = rootnode;

                    int filecount = 0;

                    ArcFile rootarc = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcFile;
                    if (rootarc != null)
                    {
                        filecount = rootarc.FileCount;
                        filecount--;
                        rootarc.FileCount--;
                        rootarc.FileAmount--;
                        frename.Mainfrm.TreeSource.SelectedNode.Tag = rootarc;
                    }

                    sw.WriteLine("Current file Count: " + filecount);
                    sw.WriteLine("===============================================================================================================");

                    frename.Mainfrm.TreeSource.SelectedNode = selectednode;
                    frename.Mainfrm.pGrdMain.SelectedObject = null;
                }
            }

        }

        private static void MenuItemRenameFolder_Click(Object sender, System.EventArgs e)
        {
            FrmRename frn = new FrmRename();
            //frename = frn;
            frn = frename;
            frn.ShowIt();
            //frn.Show();

        }

        private static void MenuItemDeleteFolder_Click(Object sender, System.EventArgs e)
        {

            DialogResult DelResult = MessageBox.Show("Deleting this will also erase anything inside this folder as well. \nAre you sure you want to do this? This cannot be undone!", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (DelResult == DialogResult.Yes)
            {
                using (StreamWriter sw = File.AppendText("Log.txt"))
                {
                    sw.WriteLine("Deleted a Folder and everything in it: " + frename.Mainfrm.TreeSource.SelectedNode + "\nCurrent File List:\n");
                    sw.WriteLine("===============================================================================================================");

                    int filecount = 0;
                    int filesremoved = 0;
                    filesremoved = frename.Mainfrm.TreeSource.SelectedNode.GetNodeCount(true);

                    TreeNode rootnode = new TreeNode();
                    rootnode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                    int FolderCount = 0;

                    //FolderCount = RecursiveFolderCount(rootnode, FolderCount);
                    filesremoved = filesremoved - FolderCount;

                    //Deletes the Folder and everything in it. Don't say I didn't warn you.
                    frename.Mainfrm.TreeSource.SelectedNode.Remove();
                    frename.Mainfrm.OpenFileModified = true;
                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.TopNode);
                    int entrycount = 0;
                    frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);

                    frename.Mainfrm.TreeSource.SelectedNode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);

                    //Gets File Count.
                    List<TreeNode> Nodes = new List<TreeNode>();
                    frename.Mainfrm.AddChildren(Nodes, frename.Mainfrm.TreeSource.SelectedNode);

                    int nowcount = 0;

                    foreach (TreeNode treno in Nodes)
                    {
                        if(treno.Tag as string != null && treno.Tag as string == "Folder")
                        {
                            
                        }
                        else
                        {
                            nowcount++;
                        }
                    } 

                    ArcFile rootarc = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcFile;
                    if (rootarc != null)
                    {
                        rootarc.FileCount = Convert.ToByte(nowcount);
                        rootarc.FileAmount = Convert.ToUInt16(nowcount);
                        frename.Mainfrm.TreeSource.SelectedNode.Tag = rootarc;
                    }

                    sw.WriteLine("Current file Count: " + nowcount);
                    sw.WriteLine("===============================================================================================================");
                    frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;


                }
            }

        }

        private static void MenuItemImportFileInFolder_Click(Object sender, System.EventArgs e)
        {
            ArcEntry Aentry = new ArcEntry();
            OpenFileDialog IMPDialog = new OpenFileDialog();
            var tag = frename.Mainfrm.TreeSource.SelectedNode.Tag;
            //Gotta rewrite this to incorporate Textures.            
            Aentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcEntry;
            if (IMPDialog.ShowDialog() == DialogResult.OK)
            {
                string helper = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                string otherhelper = Path.GetExtension(IMPDialog.FileName);

                switch (otherhelper)
                {
                    case ".tex":
                        frename.Mainfrm.TreeSource.BeginUpdate();
                        ArcEntryWrapper NewWrapperTEX = new ArcEntryWrapper();
                        TextureEntry Tentry = new TextureEntry();

                        Tentry = TextureEntry.InsertTextureEntry(frename.Mainfrm.TreeSource, NewWrapperTEX, IMPDialog.FileName);
                        NewWrapperTEX.Tag = Tentry;
                        NewWrapperTEX.Text = Tentry.TrueName;
                        NewWrapperTEX.Name = Tentry.TrueName;
                        NewWrapperTEX.FileExt = Tentry.FileExt;
                        NewWrapperTEX.entryData = Tentry;

                        frename.Mainfrm.IconSetter(NewWrapperTEX, NewWrapperTEX.FileExt);

                        NewWrapperTEX.ContextMenu = TextureContextAdder(NewWrapperTEX, frename.Mainfrm.TreeSource);

                        frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(NewWrapperTEX);

                        frename.Mainfrm.TreeSource.SelectedNode = NewWrapperTEX;

                        frename.Mainfrm.OpenFileModified = true;

                        string typeTEX = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                        frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                        frename.Mainfrm.TreeSource.EndUpdate();

                        TreeNode rootnodeTEX = new TreeNode();
                        TreeNode selectednodeTEX = new TreeNode();
                        selectednodeTEX = frename.Mainfrm.TreeSource.SelectedNode;
                        rootnodeTEX = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);
                        frename.Mainfrm.TreeSource.SelectedNode = rootnodeTEX;

                        int filecountTEX = 0;

                        ArcFile rootarcTEX = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcFile;
                        if (rootarcTEX != null)
                        {
                            filecountTEX = rootarcTEX.FileCount;
                            filecountTEX++;
                            rootarcTEX.FileCount++;
                            rootarcTEX.FileAmount++;
                            frename.Mainfrm.TreeSource.SelectedNode.Tag = rootarcTEX;
                        }



                        //Writes to log file.
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Inserted a file: " + IMPDialog.FileName + "\nCurrent File List:\n");
                            sw.WriteLine("===============================================================================================================");
                            int entrycount = 0;
                            frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);
                            sw.WriteLine("Current file Count: " + filecountTEX);
                            sw.WriteLine("===============================================================================================================");
                        }

                        frename.Mainfrm.TreeSource.SelectedNode = selectednodeTEX;

                        break;

                    case ".lrp":
                        frename.Mainfrm.TreeSource.BeginUpdate();
                        ArcEntryWrapper NewWrapperRPL = new ArcEntryWrapper();
                        ResourcePathListEntry RlistEntry = new ResourcePathListEntry();


                        //RlistEntry = ArcEntry.InsertEntry(frename.Mainfrm.TreeSource, NewWrapper, IMPDialog.FileName);
                        RlistEntry = ResourcePathListEntry.InsertRPL(frename.Mainfrm.TreeSource, NewWrapperRPL, IMPDialog.FileName);
                        NewWrapperRPL.Tag = RlistEntry;
                        NewWrapperRPL.Text = RlistEntry.TrueName;
                        NewWrapperRPL.Name = RlistEntry.TrueName;
                        NewWrapperRPL.FileExt = RlistEntry.FileExt;
                        NewWrapperRPL.entryData = RlistEntry;

                        frename.Mainfrm.IconSetter(NewWrapperRPL, NewWrapperRPL.FileExt);

                        NewWrapperRPL.ContextMenu = GenericFileContextAdder(NewWrapperRPL, frename.Mainfrm.TreeSource);

                        frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(NewWrapperRPL);

                        frename.Mainfrm.TreeSource.SelectedNode = NewWrapperRPL;

                        frename.Mainfrm.OpenFileModified = true;

                        //Reloads the replaced file data in the text box.
                        frename.Mainfrm.txtRPList = ResourcePathListEntry.LoadRPLInTextBox(frename.Mainfrm.txtRPList, RlistEntry);
                        frename.Mainfrm.RPLBackup = frename.Mainfrm.txtRPList.Text;

                        string typeRPL = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                        frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;
                        
                        frename.Mainfrm.TreeSource.EndUpdate();

                        TreeNode rootnodeRPL = new TreeNode();
                        TreeNode selectednodeRPL = new TreeNode();
                        selectednodeRPL = frename.Mainfrm.TreeSource.SelectedNode;
                        rootnodeRPL = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);
                        frename.Mainfrm.TreeSource.SelectedNode = rootnodeRPL;

                        int filecountRPL = 0;

                        ArcFile rootarcRPL = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcFile;
                        if (rootarcRPL != null)
                        {
                            filecountRPL = rootarcRPL.FileCount;
                            filecountRPL++;
                            rootarcRPL.FileCount++;
                            rootarcRPL.FileAmount++;
                            frename.Mainfrm.TreeSource.SelectedNode.Tag = rootarcRPL;
                        }



                        //Writes to log file.
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Inserted a file: " + IMPDialog.FileName + "\nCurrent File List:\n");
                            sw.WriteLine("===============================================================================================================");
                            int entrycount = 0;
                            frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);
                            sw.WriteLine("Current file Count: " + filecountRPL);
                            sw.WriteLine("===============================================================================================================");
                        }

                        frename.Mainfrm.TreeSource.SelectedNode = selectednodeRPL;
                        break;
                         


                        //break;



                    case ".dds":
                    case ".DDS":

                        //Creates and Spawns the Texture Encoder Dialog.
                        FrmTexEncodeDialog frmtexencode = FrmTexEncodeDialog.LoadDDSData(IMPDialog.FileName, IMPDialog);
                        frmtexencode.IsReplacing = false;

                        frmtexencode.ShowDialog();

                        if(frmtexencode.DialogResult == DialogResult.OK)
                        {
                            frename.Mainfrm.TreeSource.BeginUpdate();
                            ArcEntryWrapper NewWrapperDDS = new ArcEntryWrapper();
                            TextureEntry DDSentry = new TextureEntry();


                            DDSentry = TextureEntry.InsertTextureFromDDS(frename.Mainfrm.TreeSource, NewWrapperDDS,IMPDialog.FileName,frmtexencode, frmtexencode.TexData);
                            NewWrapperDDS.Tag = DDSentry;
                            NewWrapperDDS.Text = DDSentry.TrueName;
                            NewWrapperDDS.Name = DDSentry.TrueName;
                            NewWrapperDDS.FileExt = DDSentry.FileExt;
                            NewWrapperDDS.entryData = DDSentry;

                            frename.Mainfrm.IconSetter(NewWrapperDDS, NewWrapperDDS.FileExt);

                            NewWrapperDDS.ContextMenu = TextureContextAdder(NewWrapperDDS, frename.Mainfrm.TreeSource);

                            frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(NewWrapperDDS);

                            frename.Mainfrm.TreeSource.SelectedNode = NewWrapperDDS;

                            frename.Mainfrm.OpenFileModified = true;

                            string typeDDS = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                            frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                            frename.Mainfrm.TreeSource.EndUpdate();

                            TreeNode rootnodeDDS = new TreeNode();
                            TreeNode selectednodeDDS = new TreeNode();
                            selectednodeDDS = frename.Mainfrm.TreeSource.SelectedNode;
                            rootnodeDDS = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);
                            frename.Mainfrm.TreeSource.SelectedNode = rootnodeDDS;

                            int filecountDDS = 0;

                            ArcFile rootarcDDS = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcFile;
                            if (rootarcDDS != null)
                            {
                                filecountDDS = rootarcDDS.FileCount;
                                filecountDDS++;
                                rootarcDDS.FileCount++;
                                rootarcDDS.FileAmount++;
                                frename.Mainfrm.TreeSource.SelectedNode.Tag = rootarcDDS;
                            }



                            //Writes to log file.
                            using (StreamWriter sw = File.AppendText("Log.txt"))
                            {
                                sw.WriteLine("Inserted a file: " + IMPDialog.FileName + "\nCurrent File List:\n");
                                sw.WriteLine("===============================================================================================================");
                                int entrycount = 0;
                                frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);
                                sw.WriteLine("Current file Count: " + filecountDDS);
                                sw.WriteLine("===============================================================================================================");
                            }

                            frename.Mainfrm.TreeSource.SelectedNode = selectednodeDDS;

                        }

                        break;

                    //For everything else.
                    default:
                        frename.Mainfrm.TreeSource.BeginUpdate();
                        ArcEntryWrapper NewWrapper = new ArcEntryWrapper();
                        ArcEntry NEntry = new ArcEntry();

                        NEntry = ArcEntry.InsertEntry(frename.Mainfrm.TreeSource, NewWrapper, IMPDialog.FileName);
                        NewWrapper.Tag = NEntry;
                        NewWrapper.Text = NEntry.TrueName;
                        NewWrapper.Name = NEntry.TrueName;
                        NewWrapper.FileExt = NEntry.FileExt;
                        NewWrapper.entryData = NEntry;

                        frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);

                        NewWrapper.ContextMenu = GenericFileContextAdder(NewWrapper, frename.Mainfrm.TreeSource);

                        frename.Mainfrm.TreeSource.SelectedNode.Nodes.Add(NewWrapper);

                        frename.Mainfrm.TreeSource.SelectedNode = NewWrapper;

                        frename.Mainfrm.OpenFileModified = true;

                        string type = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();
                        frename.Mainfrm.pGrdMain.SelectedObject = frename.Mainfrm.TreeSource.SelectedNode.Tag;

                        frename.Mainfrm.TreeSource.EndUpdate();

                        TreeNode rootnode = new TreeNode();
                        TreeNode selectednode = new TreeNode();
                        selectednode = frename.Mainfrm.TreeSource.SelectedNode;
                        rootnode = frename.Mainfrm.FindRootNode(frename.Mainfrm.TreeSource.SelectedNode);
                        frename.Mainfrm.TreeSource.SelectedNode = rootnode;

                        int filecount = 0;

                        ArcFile rootarc = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcFile;
                        if (rootarc != null)
                        {
                            filecount = rootarc.FileCount;
                            filecount++;
                            rootarc.FileCount++;
                            rootarc.FileAmount++;
                            frename.Mainfrm.TreeSource.SelectedNode.Tag = rootarc;
                        }



                        //Writes to log file.
                        using (StreamWriter sw = File.AppendText("Log.txt"))
                        {
                            sw.WriteLine("Inserted a file: " + IMPDialog.FileName + "\nCurrent File List:\n");
                            sw.WriteLine("===============================================================================================================");
                            int entrycount = 0;
                            frename.Mainfrm.PrintRecursive(frename.Mainfrm.TreeSource.TopNode, sw, entrycount);
                            sw.WriteLine("Current file Count: " + filecount);
                            sw.WriteLine("===============================================================================================================");
                        }

                        frename.Mainfrm.TreeSource.SelectedNode = selectednode;
                        break;
                }


            }

            
        }

        private void TreeFill(string D, int E, ArcFile archivearc)
        {
            TreeSource.Nodes.Clear();
            //For ensuring the tree isn't redrawn per entry. Signifcantly lowers load times for an Archive.
            TreeSource.Visible = false;
            TreeSource.BeginUpdate();
            ArcFileWrapper parent = new ArcFileWrapper();

            CFile = Path.GetFileNameWithoutExtension(D);
            parent.Text = CFile;
            parent.Tag = archivearc;
            parent.Name = CFile;
            parent.archivefile = archivearc;
            TreeSource.Nodes.Add(parent);

            //Extension Checking.
            CExt = Path.GetExtension(D);
            TreeSource.ImageList = imageList1;
            //TreeSource.TopNode = parent;
            TreeSource.SelectedNode = parent;

            TreeSource.ImageIndex = 16;
            TreeSource.SelectedImageIndex = 16;

            if (CExt == ".mis")
            {
                TreeSource.SelectedNode.ImageIndex = 10;
                TreeSource.SelectedNode.SelectedImageIndex = 10;
            }
            else if (CExt == ".arc")
            {
                TreeSource.SelectedNode.ImageIndex = 1;
                TreeSource.SelectedNode.SelectedImageIndex = 1;
            }
            else
            {
                TreeSource.SelectedNode.ImageIndex = 16;
                TreeSource.SelectedNode.SelectedImageIndex = 16;
            }



        }

        public void TreeChildInsert(int E, string F, string G, string[] H, string I, object FEntry)
        {
            string type = FEntry.GetType().ToString();
            switch (type)
            {

                //For Textures.
                case "ThreeWorkTool.Resources.Wrappers.TextureEntry":
                ArcEntryWrapper tchild = new ArcEntryWrapper();


            TreeSource.BeginUpdate();

            //Fentry = Convert.ChangeType(Fentry, typeof(TextureEntry));

            tchild.Name = I;
            tchild.Tag = FEntry as TextureEntry;
            tchild.Text = I;
            tchild.entryfile = FEntry as TextureEntry;
            tchild.FileExt = G;

            //Checks for subdirectories. Makes folder if they don't exist already.
            foreach (string Folder in H)
            {
                if (!TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                {
                    TreeNode folder = new TreeNode();
                    folder.Name = Folder;
                    folder.Tag = "Folder";
                    folder.Text = Folder;
                    folder.ContextMenu = FolderContextAdder(folder, TreeSource);
                    TreeSource.SelectedNode.Nodes.Add(folder);
                    TreeSource.SelectedNode = folder;
                    TreeSource.SelectedNode.ImageIndex = 2;
                    TreeSource.SelectedNode.SelectedImageIndex = 2;
                }
                else
                {
                    TreeSource.SelectedNode = GetNodeByName(TreeSource.SelectedNode.Nodes, Folder);
                }
            }

            TreeSource.SelectedNode = tchild;

            TreeSource.SelectedNode.Nodes.Add(tchild);

            TreeSource.ImageList = imageList1;

            var rootNode = FindRootNode(tchild);

            TreeSource.SelectedNode = tchild;
            TreeSource.SelectedNode.ImageIndex = 15;
            TreeSource.SelectedNode.SelectedImageIndex = 15;
           

            tchild.ContextMenu = TextureContextAdder(tchild, TreeSource);

            TreeSource.SelectedNode = rootNode;

            tcount++;
            break;


                //For Resouce Path Lists.
                case "ThreeWorkTool.Resources.Wrappers.ResourcePathListEntry":
                    ArcEntryWrapper rplchild = new ArcEntryWrapper();


                    TreeSource.BeginUpdate();

                    //Fentry = Convert.ChangeType(Fentry, typeof(TextureEntry));

                    rplchild.Name = I;
                    rplchild.Tag = FEntry as ResourcePathListEntry;
                    rplchild.Text = I;
                    rplchild.entryfile = FEntry as ResourcePathListEntry;
                    rplchild.FileExt = G;

                    //Checks for subdirectories. Makes folder if they don't exist already.
                    foreach (string Folder in H)
                    {
                        if (!TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                        {
                            TreeNode folder = new TreeNode();
                            folder.Name = Folder;
                            folder.Tag = "Folder";
                            folder.Text = Folder;
                            folder.ContextMenu = FolderContextAdder(folder, TreeSource);
                            TreeSource.SelectedNode.Nodes.Add(folder);
                            TreeSource.SelectedNode = folder;
                            TreeSource.SelectedNode.ImageIndex = 2;
                            TreeSource.SelectedNode.SelectedImageIndex = 2;
                        }
                        else
                        {
                            TreeSource.SelectedNode = GetNodeByName(TreeSource.SelectedNode.Nodes, Folder);
                        }
                    }

                    TreeSource.SelectedNode = rplchild;

                    TreeSource.SelectedNode.Nodes.Add(rplchild);

                    TreeSource.ImageList = imageList1;

                    var rplrootNode = FindRootNode(rplchild);

                    TreeSource.SelectedNode = rplchild;
                    TreeSource.SelectedNode.ImageIndex = 17;
                    TreeSource.SelectedNode.SelectedImageIndex = 17;


                    rplchild.ContextMenu = GenericFileContextAdder(rplchild, TreeSource);
                    
                    TreeSource.SelectedNode = rplrootNode;

                    tcount++;
                    break;


                //Cases for future file supports go here. For example;
                //case ".mod":

                //For Undocumented file types. Anything else should have a case above.
                default:

                    ArcEntryWrapper child = new ArcEntryWrapper();


                    TreeSource.BeginUpdate();

                    ArcEntry caev = new ArcEntry();
                    caev = FEntry as ArcEntry;

                    child.Name = I;
                    child.Tag = caev;
                    child.Text = I;
                    child.entryfile = caev;
                    child.FileExt = G;

                    //Checks for subdirectories. Makes folder if they don't exist already.
                    foreach (string Folder in H)
                    {
                        if (!TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                        {
                            TreeNode folder = new TreeNode();
                            folder.Name = Folder;
                            folder.Tag = "Folder";
                            folder.Text = Folder;
                            folder.ContextMenu = FolderContextAdder(folder, TreeSource);
                            TreeSource.SelectedNode.Nodes.Add(folder);
                            TreeSource.SelectedNode = folder;
                            TreeSource.SelectedNode.ImageIndex = 2;
                            TreeSource.SelectedNode.SelectedImageIndex = 2;
                        }
                        else
                        {
                            TreeSource.SelectedNode = GetNodeByName(TreeSource.SelectedNode.Nodes, Folder);
                        }
                    }

                    TreeSource.SelectedNode = child;

                    TreeSource.SelectedNode.Nodes.Add(child);

                    TreeSource.ImageList = imageList1;

                    var trootNode = FindRootNode(child);

                    TreeSource.SelectedNode = child;

                    if (G == ".mis")
                    {
                        TreeSource.SelectedNode.ImageIndex = 10;
                        TreeSource.SelectedNode.SelectedImageIndex = 10;
                    }
                    else if (G == ".arc")
                    {
                        TreeSource.SelectedNode.ImageIndex = 1;
                        TreeSource.SelectedNode.SelectedImageIndex = 1;
                    }
                    else if (G == ".tex")
                    {
                        TreeSource.SelectedNode.ImageIndex = 15;
                        TreeSource.SelectedNode.SelectedImageIndex = 15;
                    }
                    else if (G == ".mod")
                    {
                        TreeSource.SelectedNode.ImageIndex = 11;
                        TreeSource.SelectedNode.SelectedImageIndex = 11;
                    }
                    else if (G == ".efl")
                    {
                        TreeSource.SelectedNode.ImageIndex = 8;
                        TreeSource.SelectedNode.SelectedImageIndex = 8;
                    }
                    else
                    {
                        TreeSource.SelectedNode.ImageIndex = 16;
                        TreeSource.SelectedNode.SelectedImageIndex = 16;
                    }

                    child.ContextMenu = GenericFileContextAdder(child, TreeSource);

                    TreeSource.SelectedNode = trootNode;

                    tcount++;
                    break;
            }
        }

        public ArcEntryWrapper IconSetter(ArcEntryWrapper wrapper, string extension)
        {

            if (extension == ".mis")
            {
                wrapper.ImageIndex = 10;
                wrapper.SelectedImageIndex = 10;
            }
            else if (extension == ".arc")
            {
                wrapper.ImageIndex = 1;
                wrapper.SelectedImageIndex = 1;
            }
            else if (extension == ".tex")
            {
                wrapper.ImageIndex = 15;
                wrapper.SelectedImageIndex = 15;
            }
            else if (extension == ".mod")
            {
                wrapper.ImageIndex = 11;
                wrapper.SelectedImageIndex = 11;
            }
            else if (extension == ".efl")
            {
                wrapper.ImageIndex = 8;
                wrapper.SelectedImageIndex = 8;
            }
            else if (extension == ".lrp")
            {
                wrapper.ImageIndex = 17;
                wrapper.SelectedImageIndex = 17;
            }
            else
            {
                wrapper.ImageIndex = 16;
                wrapper.SelectedImageIndex = 16;
            }


            return wrapper;
        }

        public void PathFinder(TreeView treesource, ArcEntryWrapper arcnode)
        {

        }

        private void TreeSource_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeSource.SelectedNode = e.Node;
            e.Node.GetType();

            string type = e.Node.Tag.GetType().ToString();


            switch (type)
            {
                case "ThreeWorkTool.Resources.Wrappers.ResourcePathListEntry":
                    FinishRPLRead = false;
                    pGrdMain.SelectedObject = e.Node.Tag;
                    ResourcePathListEntry rplentry = new ResourcePathListEntry();
                    rplentry = e.Node.Tag as ResourcePathListEntry;
                    picBoxA.Visible = false;
                    txtRPList.Text = "";
                    txtRPList.Dock = System.Windows.Forms.DockStyle.Fill;
                    txtRPList = ResourcePathListEntry.LoadRPLInTextBox(txtRPList,rplentry);
                    RPLBackup = txtRPList.Text;
                    txtRPList.Visible = true;
                    FinishRPLRead = true;
                    break;

                case "ThreeWorkTool.Resources.Wrappers.TexEntryWrapper":
                    pGrdMain.SelectedObject = e.Node.Tag;
                    TextureEntry tentry = new TextureEntry();
                    tentry = e.Node.Tag as TextureEntry;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    picBoxA.Visible = true;
                    Bitmap bm = BitmapBuilderDX(tentry.OutMaps, tentry, picBoxA);
                    if (bm == null)
                    {
                        picBoxA.Image = picBoxA.ErrorImage;
                        break;
                    }
                    else
                    {
                        ImageRescaler(bm, picBoxA, tentry);
                        break;
                    }

                case "ThreeWorkTool.Resources.Wrappers.TextureEntry":
                    pGrdMain.SelectedObject = e.Node.Tag;
                    TextureEntry txentry = new TextureEntry();
                    txentry = e.Node.Tag as TextureEntry;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    picBoxA.Visible = true;
                    Bitmap bmx = BitmapBuilderDX(txentry.OutMaps, txentry, picBoxA);
                    if (bmx == null)
                    {
                        picBoxA.Image = picBoxA.ErrorImage;
                        break;
                    }
                    else
                    {
                        ImageRescaler(bmx, picBoxA, txentry);
                        break;
                    }

                case "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper":
                    ArcEntry entry = new ArcEntry();
                    pGrdMain.SelectedObject = e.Node.Tag;
                    picBoxA.Visible = false;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    break;

                case "ThreeWorkTool.Resources.Archives.ArcEntry":
                    pGrdMain.SelectedObject = e.Node.Tag;
                    picBoxA.Visible = false;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    break;

                case "ThreeWorkTool.Resources.Wrappers.ArcFileWrapper":
                    ArcFile afile = new ArcFile();
                    pGrdMain.SelectedObject = e.Node.Tag;
                    picBoxA.Visible = false;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    break;

                case "ThreeWorkTool.Resources.Archives.ArcFile":
                    pGrdMain.SelectedObject = e.Node.Tag;
                    picBoxA.Visible = false;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    break;
                default:
                    pGrdMain.SelectedObject = null;
                    picBoxA.Visible = false;
                    txtRPList.Visible = false;
                    txtRPList.Dock = System.Windows.Forms.DockStyle.None;
                    break;
            }
        }

        private void TreeSource_SelectionChanged(object sender, EventArgs e)
        {
            ThreeSourceNodeBase b;
            ArcEntryWrapper aewrap;
            if ((TreeSource.SelectedNode is ArcEntryWrapper) && (TreeSource.SelectedNode != null))
            {
                //aewrap = TreeSource.SelectedNode as ArcEntryWrapper;
                //pGrdMain.SelectedObject = aewrap.entryData;
            }
        }

        //Makes Bitmap from byte array containing DDS file.
        public static Bitmap BitmapBuilderDX(byte[] ddsfile, TextureEntry textureEntry, PictureBox picbox)
        {
            if (textureEntry.OutMaps != null)
            {
                /*
                PixelFormat PixForm = PixelFormat.Format32bppArgb;
                //Gets the largest Mip Map Pixel Data.
                byte[] PData = textureEntry.OutMapsB[0];

                switch (textureEntry.TexType)
                {
                    case("13"):
                        break;

                    case ("17"):
                        break;

                    default:
                        break;
                }
                */
                
                Stream ztrim = new MemoryStream(textureEntry.OutMaps);
                //From the pfim website.
                using (var image = Pfim.Pfim.FromStream(ztrim))
                {
                    PixelFormat format;

                    // Convert from Pfim's backend agnostic image format into GDI+'s image format
                    switch (image.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        //case Pfim.ImageFormat.Rgb24:
                           // format = PixelFormat.Format24bppRgb;
                            //break;
                        default:
                            // see the sample for more details
                            throw new NotImplementedException();
                    }

                    // Pin pfim's data array so that it doesn't get reaped by GC, unnecessary
                    // in this snippet but useful technique if the data was going to be used in
                    // control like a picture box
                    var handle = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    try
                    {
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0);
                        var pmap = new Bitmap(image.Width, image.Height, image.Stride, format, data);
                        return pmap;
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
                
                

            }
            else
            {
                picbox.Image = picbox.ErrorImage;
                return null;
            }
        }

        //Attempts to resize PictureBox to actually make the image fit the original proportions.
        public static void ImageRescaler(Bitmap bm, PictureBox pb, TextureEntry te)
        {
            if (bm.Width > pb.Width || bm.Height > pb.Height)
            {
                    int OldX = pb.Width;
                    int OldY = pb.Height;
                    pb.Image = bm;
                    //pb.Size = frename.Mainfrm.pnlNew.Size;
                    pb.SizeMode = bm.Width > OldX || bm.Height > OldY ?
                    PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
            }
            else
            {
                pb.Image = bm;
                pb.SizeMode = bm.Width < pb.Width || bm.Height < pb.Height ?
                PictureBoxSizeMode.Zoom : PictureBoxSizeMode.CenterImage;
            }
        }


        #endregion

        #region Arc Stuffs

        private void ArcFill()
        {
            //string dirchecker;

            tcount = 0;

            List<string> subdirs = new List<String>();
            List<string> RPLNameList = new List<string>();

            ArcFile newArc = ArcFile.LoadArc(TreeSource, FilePath, subdirs, false);

            NCount = 0;

            TreeFill(newArc.Tempname, NCount, newArc);

            NCount = 1;
            //int RCount = 0;

            Arcsize = newArc.Totalsize;

            TreeSource.archivefile = newArc;

            //For whatever is inside the archive itself.
            foreach (var ArcEntry in newArc.arcfiles)
            {

                string type = ArcEntry.GetType().ToString();

                switch(type)
                {

                    case "ThreeWorkTool.Resources.Wrappers.TextureEntry":
                        TextureEntry te = new TextureEntry();
                        te = ArcEntry as TextureEntry;
                        if(te != null)
                        {
                            TreeChildInsert(NCount, te.EntryName, te.FileExt, te.EntryDirs, te.TrueName, te);
                            TreeSource.SelectedNode = FindRootNode(TreeSource.SelectedNode);
                            break;
                        }
                        else
                        {
                            MessageBox.Show("We got a read error here!", "YIKES");
                            break;
                        }

                    case "ThreeWorkTool.Resources.Wrappers.ResourcePathListEntry":
                        ResourcePathListEntry rple = new ResourcePathListEntry();
                        rple = ArcEntry as ResourcePathListEntry;
                        if (rple != null)
                        {
                            TreeChildInsert(NCount, rple.EntryName, rple.FileExt, rple.EntryDirs, rple.TrueName, rple);
                            TreeSource.SelectedNode = FindRootNode(TreeSource.SelectedNode);
                            break;
                        }
                        else
                        {
                            MessageBox.Show("We got a read error here!", "YIKES");
                            break;
                        }

                    default:
                        //Fills in child nodes, i.e. the filenames inside the archive.
                        ArcEntry ae = new ArcEntry();
                        ae = ArcEntry as ArcEntry;
                        if (ae != null)
                        {
                            TreeChildInsert(NCount, ae.EntryName, ae.FileExt, ae.EntryDirs, ae.TrueName, ae);
                            TreeSource.SelectedNode = FindRootNode(TreeSource.SelectedNode);
                            break;
                        }
                        else
                        {
                            MessageBox.Show("We got a read error here!","YIKES");
                            break;
                        }
                }
            }

            TreeSource.EndUpdate();
            TreeSource.Visible = true;
            //Fills in Arc Data and selects it on the grid.
            pGrdMain.SelectedObject = newArc;


            FrmRename frn = new FrmRename();
            frn.Mainfrm = this;
            frename = frn;

            TreeSource.Sort();

            //Writes to log file.
            using (StreamWriter sw = new StreamWriter("Log.txt"))
            {
                sw.WriteLine("Archive file: " + FilePath + " Opened.\nFile List:\n");
                sw.WriteLine("===============================================================================================================");
                int entrycount = 0;
                PrintRecursive(TreeSource.TopNode, sw, 0);
                entrycount = frename.Mainfrm.TreeSource.TopNode.GetNodeCount(true);
                sw.WriteLine("Current file Count: " + entrycount);
                sw.WriteLine("===============================================================================================================");
            }
            
        }


        //This is test stuff from the Microsoft website. Modified for my purposes.
        private void PrintRecursive(TreeNode WrapNode, StreamWriter sw, int count)
        {
            if (WrapNode.Tag is ArcEntry)
            {
                ArcEntry ae = new ArcEntry();
                ae = WrapNode.Tag as ArcEntry;
                //Outputs name to log.
                sw.WriteLine(ae.EntryName);
            }
            else if(WrapNode.Tag is TextureEntry)
            {
                TextureEntry ae = new TextureEntry();
                ae = WrapNode.Tag as TextureEntry;
                //Outputs name to log.
                sw.WriteLine(ae.EntryName);
            }
            else
            {


            }

            // Print each node recursively.  
            foreach (TreeNode tn in WrapNode.Nodes)
            {
                count++;
                PrintRecursive(tn, sw, count);
            }
        }

        //This is test stuff from the Microsoft website. Modified for my purposes.
        private void CountFiles(TreeNode WrapNode, StreamWriter sw, int count)
        {

            foreach (TreeNode tn in frename.Mainfrm.TreeSource.SelectedNode.Nodes)
            {
                count++;
            }

                int c = 0;
            if (WrapNode.Tag is ArcEntry)
            {
                foreach (TreeNode tn in WrapNode.Nodes)
                {
                    count++;
                    PrintRecursive(tn, sw, count);
                }
            }
        }

        private static int RecursiveFolderCount(TreeNode WrapNode, int foldercount)
        {
            foreach (TreeNode tn in frename.Mainfrm.TreeSource.SelectedNode.Nodes)
            {
                if (tn.Tag as string != null && tn.Tag as string == "Folder")
                {                    
                    foldercount++;
                    RecursiveFolderCount(tn, foldercount);
                }
            }
            return foldercount;
        }

        public void AddChildren(List<TreeNode> Nodes, TreeNode Node)
        {
            foreach (TreeNode thisNode in Node.Nodes)
            {
                Nodes.Add(thisNode);
                AddChildren(Nodes, thisNode);
            }
        }

        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }



        #endregion

        private void txtRPList_TextChanged(object sender, EventArgs e)
        {
            if (FinishRPLRead == true)
            {
                TextBoxUpdating();
            }
        }

        private void TextBoxUpdating()
        {
            ResourcePathListEntry rplentry = new ResourcePathListEntry();
            rplentry = TreeSource.SelectedNode.Tag as ResourcePathListEntry;
            if (rplentry != null)
            {
                rplentry = ResourcePathListEntry.RenewRPLList(txtRPList, rplentry);
                TreeSource.SelectedNode.Tag = rplentry;
                this.OpenFileModified = true;
            }
        }

        private void TextBoxLeaving()
        {

            ResourcePathListEntry rplentry = new ResourcePathListEntry();
            rplentry = TreeSource.SelectedNode.Tag as ResourcePathListEntry;
            if (rplentry != null)
            {

                string[] Stemp = new string[] { };
                string ST = txtRPList.Text;
                Stemp = ST.Split('\n');

                //Updates the text list.
                ResourcePathListEntry.UpdateRPLList(txtRPList, rplentry);
                TreeSource.SelectedNode.Tag = rplentry;

            }
        }

        private void txtRPList_Leave(object sender, EventArgs e)
        {
            //TextBoxLeaving();
        }
    }
}
