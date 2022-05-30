using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

namespace ThreeWorkTool
{
    public partial class FrmReplace : Form
    {

        private static ThreeSourceTree treeview;
        //private static TreeView treeview;
        private static TreeNode node_;
        //private static ThreeSourceNodeBase node_;
        public FrmMainThree Mainfrm { get; set; }
        public bool AllFiles;

        public FrmReplace()
        {
            InitializeComponent();
        }

        public void ShowItItem()
        {
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            this.ShowDialog();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnReplaceExit_Click(object sender, EventArgs e)
        {
            //Closes the form without making changes.
            DialogResult = DialogResult.Cancel;
            Hide();
        }


        private void btnReplaceReplace_Click(object sender, EventArgs e)
        {
            if (AllFiles == true)
            {

                //Checks All filenames for searched term. Gets to the parent node, checks all the children nodes, their tags, filenames, etc. for the term.

                //Goes to top node to begin iteration.
                TreeNode tn = Mainfrm.FindRootNode(Mainfrm.TreeSource.SelectedNode);
                Mainfrm.TreeSource.SelectedNode = tn;

                List<TreeNode> Nodes = new List<TreeNode>();
                Mainfrm.AddChildren(Nodes, Mainfrm.TreeSource.SelectedNode);

                Mainfrm.TreeSource.BeginUpdate();

                int RenameCount = 0;

                foreach (TreeNode tno in Nodes)
                {

                    //Gets the node as a ArcEntryWrapper to allow access to all the variables and data.
                    ArcEntryWrapper awrapper = tno as ArcEntryWrapper;

                    if (awrapper != null)
                    {
                        if (awrapper.Tag as MaterialTextureReference == null || awrapper.Tag as LMTM3AEntry == null || awrapper.Tag as ModelBoneEntry == null
                        || awrapper.Tag as MaterialMaterialEntry == null || awrapper.Tag as ModelGroupEntry == null || awrapper.Tag as Mission == null
                        || awrapper.Tag as EffectNode == null)
                        {
                            {
                                if (awrapper.Tag as string != null)
                                {
                                    //Replaces the Term in the folder.
                                    string FolderName = awrapper.Tag as string;
                                    FolderName = FolderName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    RenameCount++;
                                }
                                else
                                {
                                    //Goes through the treenode to replace all the names of the node.
                                    ArcEntry enty = new ArcEntry();
                                    TextureEntry tenty = new TextureEntry();
                                    ResourcePathListEntry lrpenty = new ResourcePathListEntry();
                                    MSDEntry msdenty = new MSDEntry();
                                    MaterialEntry matent = new MaterialEntry();
                                    LMTEntry lmtenty = new LMTEntry();
                                    ChainListEntry cstenty = new ChainListEntry();
                                    ChainEntry chnenty = new ChainEntry();
                                    ChainCollisionEntry cclentry = new ChainCollisionEntry();
                                    ModelEntry mdlentry = new ModelEntry();
                                    MissionEntry misenty = new MissionEntry();
                                    GemEntry gementy = new GemEntry();
                                    EffectListEntry eflenty = new EffectListEntry();

                                    if (tno.Tag as ArcEntry != null)
                                    {
                                        enty = tno.Tag as ArcEntry;
                                        enty.EntryName = enty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        enty.TrueName = enty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        enty.FileName = enty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = enty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as TextureEntry != null)
                                    {
                                        tenty = tno.Tag as TextureEntry;
                                        tenty.EntryName = tenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tenty.TrueName = tenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tenty.FileName = tenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = tenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ResourcePathListEntry != null)
                                    {
                                        lrpenty = tno.Tag as ResourcePathListEntry;
                                        lrpenty.EntryName = lrpenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text); 
                                        lrpenty.TrueName = lrpenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        lrpenty.FileName = lrpenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = lrpenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as LMTEntry != null)
                                    {
                                        lmtenty = tno.Tag as LMTEntry;
                                        lmtenty.EntryName = lmtenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        lmtenty.TrueName = lmtenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        lmtenty.FileName = lmtenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = lmtenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as MaterialEntry != null)
                                    {
                                        matent = tno.Tag as MaterialEntry;
                                        matent.EntryName = matent.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        matent.TrueName = matent.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        matent.FileName = matent.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = matent;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as MSDEntry != null)
                                    {
                                        msdenty = tno.Tag as MSDEntry;
                                        msdenty.EntryName = msdenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        msdenty.TrueName = msdenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        msdenty.FileName = msdenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = msdenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ChainListEntry != null)
                                    {
                                        cstenty = tno.Tag as ChainListEntry;
                                        cstenty.EntryName = cstenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        cstenty.TrueName = cstenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        cstenty.FileName = cstenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = cstenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ChainEntry != null)
                                    {
                                        chnenty = tno.Tag as ChainEntry;
                                        chnenty.EntryName = chnenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        chnenty.TrueName = chnenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        chnenty.FileName = chnenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = chnenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ChainCollisionEntry != null)
                                    {
                                        cclentry = tno.Tag as ChainCollisionEntry;
                                        cclentry.EntryName = cclentry.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        cclentry.TrueName = cclentry.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        cclentry.FileName = cclentry.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = cclentry;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ModelEntry != null)
                                    {
                                        mdlentry = tno.Tag as ModelEntry;
                                        mdlentry.EntryName = mdlentry.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        mdlentry.TrueName = mdlentry.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        mdlentry.FileName = mdlentry.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = mdlentry;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as MissionEntry != null)
                                    {
                                        misenty = tno.Tag as MissionEntry;
                                        misenty.EntryName = misenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        misenty.TrueName = misenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        misenty.FileName = misenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = misenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as GemEntry != null)
                                    {
                                        gementy = tno.Tag as GemEntry;
                                        gementy.EntryName = gementy.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        gementy.TrueName = gementy.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        gementy.FileName = gementy.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = gementy;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as EffectListEntry != null)
                                    {
                                        eflenty = tno.Tag as EffectListEntry;
                                        eflenty.EntryName = eflenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        eflenty.TrueName = eflenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        eflenty.FileName = eflenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = eflenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }

                                    RenameCount++;
                                }
                            }
                        }
                    }

                }

                Mainfrm.TreeSource.Update();

                Mainfrm.TreeSource.EndUpdate();

                MessageBox.Show("Replaced " + txtReplaceFind.Text + " with " + txtReplaceReplace.Text + " in " + RenameCount + " file and folder names.");

                Mainfrm.OpenFileModified = true;
                RenameCount = 0;
            }
            else
            {
                if (Mainfrm.TreeSource.SelectedNode.Tag is ResourcePathListEntry)
                {
                    //Gets the text, then replaces every instance of the search term with the new term.
                    string text = Mainfrm.txtRPList.Text;
                    text = text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                    Mainfrm.txtRPList.Text = text;
                }
                else if (Mainfrm.TreeSource.SelectedNode.Tag is MaterialEntry)
                {



                }
            }

        }
    }
}
