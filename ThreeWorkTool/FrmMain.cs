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

        //Replace Dialogue
        OpenFileDialog RPDialog = new OpenFileDialog();

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
                        byte[] OFBArray = File.ReadAllBytes(OFDialog.FileName);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            stream.Write(OFBArray, 0, (int)OFBArray.Length);
                            {

                            }
                            File.WriteAllBytes(SFDialog.FileName, stream.ToArray());
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
            conmenu.MenuItems.Add("Delete Folder", MenuItemDeleteFolder_Click);

            return conmenu;

        }

        //Adds Context Menu for undefined files.
        public static ContextMenu GenericFileContextAdder(ArcEntryWrapper EntryNode, TreeView TreeV)
        {
            ContextMenu conmenu = new ContextMenu();

            conmenu.MenuItems.Add(new MenuItem("Export", MenuExportFile_Click));
            conmenu.MenuItems.Add(new MenuItem("Export", MenuReplaceFile_Click));
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
                //MessageBox.Show("Check the directory to see if it was successful.");
        }

        private static void MenuReplaceFile_Click(Object sender, System.EventArgs e)
        {
            OpenFileDialog RPDialog = new OpenFileDialog();

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

            DialogResult DelResult = MessageBox.Show("Are you sure you want to do this? This cannot be undone!", "Hey!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (DelResult == DialogResult.Yes)
            {
                frename.Mainfrm.TreeSource.SelectedNode.Remove();
                frename.Mainfrm.OpenFileModified = true;
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
                frename.Mainfrm.TreeSource.SelectedNode.Remove();
                //form2
                //form1.tree
                frename.Mainfrm.OpenFileModified = true;
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
                            folder.Tag = Folder;
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
                    //entry = e.Node;\
                    //TreeSource.SelectedNode.
                    pGrdMain.SelectedObject = e.Node.Tag;
                    //pGrdMain.SelectedObject = e.Node;
                    break;

                default:
                    pGrdMain.SelectedObject = e.Node;
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

        }




        #endregion



    }
}
