//By Eternal Yoshi.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources;

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
        public static FrmRename frename;

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
                                    List<int> offsets;


                                    ArcEntry enty = new ArcEntry();
                                    //This is for the filenames and everything after.
                                    foreach (TreeNode treno in Nodes)
                                    {
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
                                        else
                                        {

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
            MessageBox.Show("ThreeWork Tool Alpha version 0.00.\n2020-2021 By Eternal Yoshi", "About", MessageBoxButtons.OK);
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

                    //Application.Exit();
                }
                if (dlrs == DialogResult.No)
                {
                    //Application.Exit();
                }
                //else if(DialogResult.Cancel)
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

        //Adds Context Menu for undefined files.
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
            ArcEntry Aentry = new ArcEntry();
            SaveFileDialog EXDialog = new SaveFileDialog();
            var tag = frename.Mainfrm.TreeSource.SelectedNode.Tag;
            if (tag is ArcEntry)
            {
                Aentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcEntry;
                EXDialog.Filter = ExportFilters.GetFilter(Aentry.FileExt);
            }
            EXDialog.FileName = Aentry.FileName;

            if (EXDialog.ShowDialog() == DialogResult.OK)
            {
                ExportFileWriter.ArcEntryWriter(EXDialog.FileName,Aentry);
            }

            //Writes to log file.
            using (StreamWriter sw = File.AppendText("Log.txt"))
            {
                sw.WriteLine("Exported a file: " + frename.Mainfrm.TreeSource.SelectedNode.Name + " at " + EXDialog.FileName + "\n");
            }

        }

        private static void MenuReplaceFile_Click(Object sender, System.EventArgs e)
        {

            ArcEntry Aentry = new ArcEntry();
            OpenFileDialog RPDialog = new OpenFileDialog();
            var tag = frename.Mainfrm.TreeSource.SelectedNode.Tag;
            if (tag is ArcEntry)
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
                            string[] paths = OldWrapper.entryfile.EntryDirs;
                            NewWrapper = frename.Mainfrm.TreeSource.SelectedNode as ArcEntryWrapper;
                            int index = frename.Mainfrm.TreeSource.SelectedNode.Index;
                            NewWrapper.Tag = ArcEntry.ReplaceEntry(frename.Mainfrm.TreeSource, NewWrapper, RPDialog.FileName);
                            NewWrapper.ContextMenu = GenericFileContextAdder(NewWrapper, frename.Mainfrm.TreeSource);
                            frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);
                            //Takes the path data from the old node and slaps it on the new node.                            
                            NewWrapper.entryfile.EntryDirs = paths;

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

                Aentry = frename.Mainfrm.TreeSource.SelectedNode.Tag as ArcEntry;
                if (IMPDialog.ShowDialog() == DialogResult.OK)
                {
                    string helper = frename.Mainfrm.TreeSource.SelectedNode.GetType().ToString();

                    frename.Mainfrm.TreeSource.BeginUpdate();
                    ArcEntryWrapper NewWrapper = new ArcEntryWrapper();

                    NewWrapper.entryData = ArcEntry.InsertEntry(frename.Mainfrm.TreeSource,NewWrapper,IMPDialog.FileName);
                    NewWrapper.Tag = NewWrapper.entryData;
                    NewWrapper.Text = NewWrapper.entryData.TrueName;
                    NewWrapper.Name = NewWrapper.entryData.TrueName;
                    NewWrapper.FileExt = NewWrapper.entryData.FileExt;

                    frename.Mainfrm.IconSetter(NewWrapper, NewWrapper.FileExt);

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
                if(rootarc != null)
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

        public void TreeChildInsert(int E, string F, string G, string[] H, string I, ArcEntry FEntry)
        {

            switch (G)
            {
                /*
                case ".tex":

                    TexWrapNode Tchild = new TexWrapNode();

                    TreeSource.BeginUpdate();

                    Tchild.Name = I;
                    Tchild.Tag = I;
                    Tchild.Text = I;
                    Tchild.entryfile = FEntry;

                    //Checks for subdirectories. Makes folder if they don't exist already.
                    //int Findex = 0;
                    foreach (string Folder in H)
                    {
                        TreeSource.SelectedNode = FindRootNode(TreeSource.SelectedNode);
                        if (!TreeSource.SelectedNode.Nodes.ContainsKey(Folder))
                        {
                            TreeNode folder = new TreeNode();
                            folder.Name = Folder;
                            folder.Tag = Folder;
                            folder.Text = Folder;


                            //ContextM cm = new ContextM();
                            //folder.ContextMenu = ContextM.FolderContextAdder(folder, TreeSource);
                            folder.ContextMenu = FolderContextAdder(folder, TreeSource);

                            //TreeSource.SelectedNode.ContextMenuStrip.

                            TreeSource.SelectedNode.Nodes.Add(folder);
                            TreeSource.SelectedNode = folder;
                            TreeSource.SelectedNode.ImageIndex = 8;
                            TreeSource.SelectedNode.SelectedImageIndex = 8;
                        }
                        else
                        {
                            TreeSource.SelectedNode = GetNodeByName(TreeSource.SelectedNode.Nodes, Folder);
                        }
                    }

                    //Gotta code the above in if you want things to show up right.

                    TreeSource.SelectedNode.Nodes.Add(Tchild);

                    TreeSource.ImageList = imageList1;

                    var TrootNode = FindRootNode(Tchild);

                    //For testing. First one chooses root node, second one chooses the created child node.
                    TreeSource.SelectedNode = Tchild;
                    TreeSource.SelectedNode.ImageIndex = 15;
                    TreeSource.SelectedNode.SelectedImageIndex = 15;
                    TreeSource.SelectedNode = TrootNode;

                    break;
                    */
                //case ".mod":

                //For Undocumented file types. Anything else should have a case above.
                default:

                    ArcEntryWrapper child = new ArcEntryWrapper();


                    TreeSource.BeginUpdate();                    

                    child.Name = I;
                    child.Tag = FEntry;
                    child.Text = I;
                    child.entryfile = FEntry;
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

                    var rootNode = FindRootNode(child);

                    //For testing. First one chooses root node, second one chooses the created child node.
                    //TreeSource.SelectedNode = rootNode;

                    TreeSource.SelectedNode = child;

                    //TreeSource.SelectedNode = TreeSource.Nodes[E];
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

                    TreeSource.SelectedNode = rootNode;

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

            string type = e.Node.GetType().ToString();


            switch (type)
            {
                case "ThreeWorkTool.Resources.Wrappers.TexEntryWrapper":
                    pGrdMain.SelectedObject = e.Node;
                    break;

                case "ThreeWorkTool.Resources.Wrappers.ArcEntryWrapper":
                    ArcEntry entry = new ArcEntry();
                    pGrdMain.SelectedObject = e.Node.Tag;
                    break;

                case "ThreeWorkTool.Resources.Wrappers.ArcFileWrapper":
                    ArcFile afile = new ArcFile();
                    pGrdMain.SelectedObject = e.Node.Tag;
                    break;

                default:
                    pGrdMain.SelectedObject = null;
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

        #endregion

        #region Arc Stuffs

        private void ArcFill()
        {
            //string dirchecker;

            tcount = 0;

            List<string> subdirs = new List<String>();

            ArcFile newArc = ArcFile.LoadArc(TreeSource, FilePath, subdirs, false);

            NCount = 0;

            TreeFill(newArc.Tempname, NCount, newArc);

            NCount = 1;
            //int RCount = 0;

            Arcsize = newArc.Totalsize;

            TreeSource.archivefile = newArc;

            //For whatever is inside the archive itself.
            foreach (ArcEntry ArcEntry in newArc.arctable)
            {
                //Fills in child nodes, i.e. the filenames inside the archive.
                TreeChildInsert(NCount, ArcEntry.EntryName, ArcEntry.FileExt, ArcEntry.EntryDirs, ArcEntry.TrueName, ArcEntry);
                TreeSource.SelectedNode = FindRootNode(TreeSource.SelectedNode);
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

            #endregion

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

    }
}
