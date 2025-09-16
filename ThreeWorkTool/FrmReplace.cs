using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.AnimNodes;
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
            Mainfrm.Focus();
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
                        || awrapper.Tag as EffectNode == null || awrapper.Tag as STQREventData == null || awrapper.Tag as STQRNode == null || awrapper.Tag as LMTTrackNode == null
                        || awrapper.Tag as ModelBoneEntry == null || awrapper.Tag as ModelPrimitiveEntry == null || awrapper.Tag as ModelGroupEntry == null 
                        || awrapper.Tag as ModelEnvelopeEntry == null || awrapper.Tag as MaterialAnimEntry == null || awrapper.Tag as EFLPathEntry == null)
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
                                    ShotListEntry lshenty = new ShotListEntry();
                                    RIFFEntry riffenty = new RIFFEntry();
                                    AtkInfoEntry atienty = new AtkInfoEntry();
                                    ShotEntry shtenty = new ShotEntry();
                                    AnmCmdEntry anmenty = new AnmCmdEntry();
                                    ChrBaseActEntry cbaenty = new ChrBaseActEntry();
                                    SoundBankEntry sbkrenty = new SoundBankEntry();
                                    SoundRequestEntry srqrenty = new SoundRequestEntry();

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
                                    else if (tno.Tag as RIFFEntry != null)
                                    {
                                        riffenty = tno.Tag as RIFFEntry;
                                        riffenty.EntryName = riffenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        riffenty.TrueName = riffenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        riffenty.FileName = riffenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = riffenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ShotListEntry != null)
                                    {
                                        lshenty = tno.Tag as ShotListEntry;
                                        lshenty.EntryName = lshenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        lshenty.TrueName = lshenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        lshenty.FileName = lshenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = lshenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as AtkInfoEntry != null)
                                    {
                                        atienty = tno.Tag as AtkInfoEntry;
                                        atienty.EntryName = atienty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        atienty.TrueName = atienty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        atienty.FileName = atienty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = atienty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ShotEntry != null)
                                    {
                                        shtenty = tno.Tag as ShotEntry;
                                        shtenty.EntryName = shtenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        shtenty.TrueName = shtenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        shtenty.FileName = shtenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = shtenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as AnmCmdEntry != null)
                                    {
                                        anmenty = tno.Tag as AnmCmdEntry;
                                        anmenty.EntryName = anmenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        anmenty.TrueName = anmenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        anmenty.FileName = anmenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = anmenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as ChrBaseActEntry != null)
                                    {
                                        cbaenty = tno.Tag as ChrBaseActEntry;
                                        cbaenty.EntryName = cbaenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        cbaenty.TrueName = cbaenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        cbaenty.FileName = cbaenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = cbaenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as SoundBankEntry != null)
                                    {
                                        sbkrenty = tno.Tag as SoundBankEntry;
                                        sbkrenty.EntryName = sbkrenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        sbkrenty.TrueName = sbkrenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        sbkrenty.FileName = sbkrenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = sbkrenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    else if (tno.Tag as SoundRequestEntry != null)
                                    {
                                        srqrenty = tno.Tag as SoundRequestEntry;
                                        srqrenty.EntryName = srqrenty.EntryName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        srqrenty.TrueName = srqrenty.TrueName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        srqrenty.FileName = srqrenty.FileName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Tag = srqrenty;
                                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                                    }
                                    RenameCount++;
                                }
                            }
                        }
                    }

                    if (tno.Tag as string != null && tno.Tag as string == "Folder")
                    {

                        //Replaces the Term in the folder.
                        string FolderName = tno.Text as string;
                        FolderName = FolderName.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        tno.Text = tno.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        tno.Name = tno.Name.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        RenameCount++;

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
                if (Mainfrm.TreeSource.SelectedNode.Tag is ChainListEntry)
                {
                    //Gets the text, then replaces every instance of the search term with the new term.
                    string text = Mainfrm.txtRPList.Text;
                    text = text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                    Mainfrm.txtRPList.Text = text;
                }
                else if (Mainfrm.TreeSource.SelectedNode.Tag is ShotListEntry)
                {
                    //Gets the text, then replaces every instance of the search term with the new term.
                    string text = Mainfrm.txtRPList.Text;
                    text = text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                    Mainfrm.txtRPList.Text = text;
                }
                else if (Mainfrm.TreeSource.SelectedNode.Tag is GemEntry)
                {
                    //Gets the text, then replaces every instance of the search term with the new term.
                    string text = Mainfrm.txtRPList.Text;
                    text = text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                    Mainfrm.txtRPList.Text = text;
                }
                else if (Mainfrm.TreeSource.SelectedNode.Tag is MaterialEntry)
                {

                    TreeNodeCollection TNoCollection = Mainfrm.TreeSource.SelectedNode.Nodes;

                    foreach (TreeNode node in TNoCollection)
                    {
                        if (node.Tag as string != null)
                        {
                            if (node.Text as string == "Textures")
                            {
                                Mainfrm.TreeSource.SelectedNode = node;
                                break;
                            }
                        }
                    }

                    TNoCollection = Mainfrm.TreeSource.SelectedNode.Nodes;
                    Mainfrm.TreeSource.BeginUpdate();

                    foreach (TreeNode node in TNoCollection)
                    {

                        string Namer = node.Name as string;
                        node.Text = node.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        Namer = Namer.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        node.Name = Namer;

                    }

                    Mainfrm.TreeSource.SelectedNode = Mainfrm.TreeSource.SelectedNode.Parent;

                    //Now to update the Material file with all these, Whether they're changed or not.
                    TreeNode Parentnode = Mainfrm.TreeSource.SelectedNode;
                    MaterialEntry ParentMateial = Parentnode.Tag as MaterialEntry;
                    MaterialTextureReference texref = new MaterialTextureReference();
                    string TermToInject = "";
                    //int count = 0;

                    for (int i = 0; i < TNoCollection.Count; i++)
                    {

                        texref = TNoCollection[i].Tag as MaterialTextureReference;

                        TermToInject = TNoCollection[i].Text;

                        //Now for the actual file update.                    
                        List<byte> NameToInject = new List<byte>();
                        NameToInject.AddRange(Encoding.ASCII.GetBytes(TermToInject));
                        int OffsetToUse;
                        OffsetToUse = 64 + (88 * (texref.Index - 1));
                        byte[] NewName = new byte[64];
                        Array.Copy(NameToInject.ToArray(), 0, NewName, 0, NameToInject.ToArray().Length);
                        Array.Copy(NewName, 0, ParentMateial.UncompressedData, OffsetToUse, NewName.Length);
                        ParentMateial.CompressedData = Zlibber.Compressor(ParentMateial.UncompressedData);

                    }

                    Mainfrm.TreeSource.SelectedNode.Tag = ParentMateial;

                    Mainfrm.OpenFileModified = true;
                    Mainfrm.TreeSource.Update();
                    Mainfrm.TreeSource.EndUpdate();


                }

                else if (Mainfrm.TreeSource.SelectedNode.Tag is EffectListEntry)
                {
                    TreeNodeCollection TNoCollection = Mainfrm.TreeSource.SelectedNode.Nodes;
                    Mainfrm.TreeSource.BeginUpdate();
                    Mainfrm.TreeSource.Hide();
                    EffectListEntry EffectList = Mainfrm.TreeSource.SelectedNode.Tag as EffectListEntry;

                    foreach (TreeNode node in TNoCollection)
                    {
                        string Namer = node.Name as string;
                        node.Text = node.Text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        Namer = Namer.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
                        node.Name = Namer;
                    }


                    //Time to update the EFL with all the terms, regardless of modified state.
                    EFLPathEntry eflpref = new EFLPathEntry();
                    string TermToInject = "";
                    int PathOff = 0;

                    for (int i = 0; i < TNoCollection.Count; i++)
                    {
                        eflpref = TNoCollection[i].Tag as EFLPathEntry;
                        TermToInject = TNoCollection[i].Text;
                        PathOff = eflpref.Offset;

                        //Now for the actual file update.                    
                        List<byte> NameToInject = new List<byte>();
                        NameToInject.AddRange(Encoding.ASCII.GetBytes(TermToInject));
                        byte[] NewName = new byte[64];
                        Array.Copy(NameToInject.ToArray(), 0, NewName, 0, NameToInject.ToArray().Length);
                        Array.Copy(NewName, 0, EffectList.UncompressedData, PathOff, NewName.Length);

                    }

                    EffectList.CompressedData = Zlibber.Compressor(EffectList.UncompressedData);
                    Mainfrm.TreeSource.SelectedNode.Tag = EffectList;

                    Mainfrm.OpenFileModified = true;
                    Mainfrm.TreeSource.EndUpdate();
                    Mainfrm.TreeSource.Show();
                }



            }

        }

    }
}
