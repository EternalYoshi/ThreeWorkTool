using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ThreeWorkTool.Resources;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;
using static ThreeWorkTool.Resources.Wrappers.MaterialEntry;

namespace ThreeWorkTool
{
    public partial class FrmRename : Form
    {
        private static ThreeSourceTree treeview;
        //private static TreeView treeview;
        private static TreeNode node_;
        //private static ThreeSourceNodeBase node_;
        public static string OriginalName;
        public FrmMainThree Mainfrm { get; set; }

        public FrmRename()
        {
            InitializeComponent();
        }

        public void ShowIt()
        {
            txtRename.Text = Mainfrm.TreeSource.SelectedNode.Text;
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            OriginalName = Mainfrm.TreeSource.SelectedNode.Text;
            this.ShowDialog();
        }

        public void ShowItItem()
        {
            txtRename.Text = Mainfrm.TreeSource.SelectedNode.Text;
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            OriginalName = Mainfrm.TreeSource.SelectedNode.Text;
            this.ShowDialog();
        }

        public void LoadTree(TreeView maintree, TreeNode nodetouse)
        {
            TreeView treeview = maintree;
            TreeNode node_ = maintree.SelectedNode;
            OriginalName = maintree.SelectedNode.Text;
        }

        private void btnRCancel_Click(object sender, EventArgs e)
        {

            //Closes the form without making changes.
            DialogResult = DialogResult.Cancel;
            Hide();

        }

        private void btnROK_Click(object sender, EventArgs e)
        {
            string newname = txtRename.Text;

            /*
            //Checks the filename for legal characters.
            if (CFGHandler.ContainsInValidFilenameCharacters(newname) == true)
            {
                MessageBox.Show("The chosen name has invalid filename characters. Take them out.", "Uhhh");
                return;
            }
            */

            //Checks for blank/null names.
            if (newname == null || newname == "")
            {
                MessageBox.Show("This must have a name and cannot be null!", "Ahem");
                return;
            }

            if (OriginalName == txtRename.Text)
            {
                DialogResult = DialogResult.OK;
                Hide();
                return;
            }

            //Checks for existing name in directory.
            foreach (TreeNode c in node_.Parent.Nodes)
            {
                ArcEntry ae = new ArcEntry();
                ArcEntry se = new ArcEntry();
                if (node_.Tag is ArcEntry)
                {
                    ae = node_.Tag as ArcEntry;
                }

                if (c.Tag is ArcEntry)
                {
                    se = c.Tag as ArcEntry;
                }

                /*
                //Checks the nodes in the same directory for existing name AND extension and will stop if there's a node with the same type in the same directory.
                if (c.Text == txtRename.Text && c.Tag as string != "MaterialChildTexture")
                {
                    if (ae != se && ae.FileExt == se.FileExt)
                    {
                        MessageBox.Show("That name already exists on a resource of the same type on the same level. \nTry a different name.", "Oh Boy");
                        return;
                    }
                }
                */
            }

            //Changes the name to what was chosen. Should reflect on the Treeview.
            treeview.SelectedNode.Text = txtRename.Text;
            treeview.SelectedNode.Name = txtRename.Text;

            //Ensures the TrueName gets change so it gets reflected in the save.
            ArcEntry aey = new ArcEntry();
            if (treeview.SelectedNode.Tag is ArcEntry)
            {
                aey = treeview.SelectedNode.Tag as ArcEntry;
                aey.TrueName = txtRename.Text;

            }
            else if (treeview.SelectedNode.Tag != null && treeview.SelectedNode.Tag as string == "Folder")
            {

            }
            else if (treeview.SelectedNode.Tag != null && treeview.SelectedNode.Tag is MaterialTextureReference)
            {

                //Goes about accessing and updating the data inside the material in a roundabout way.
                MaterialTextureReference texref = treeview.SelectedNode.Tag as MaterialTextureReference;
                MaterialEntry mentry = new MaterialEntry();
                TreeNode parent = treeview.SelectedNode.Parent;
                TreeNode child = treeview.SelectedNode;
                treeview.SelectedNode = parent;
                parent = treeview.SelectedNode.Parent;
                treeview.SelectedNode = parent;
                mentry = treeview.SelectedNode.Tag as MaterialEntry;
                if (mentry != null)
                {
                    //Now for the actual file update.                    
                    List<byte> NameToInject = new List<byte>();
                    NameToInject.AddRange(Encoding.ASCII.GetBytes(txtRename.Text));
                    int OffsetToUse;
                    OffsetToUse = 64 + (88 * (texref.Index - 1));
                    byte[] NewName = new byte[64];
                    Array.Copy(NameToInject.ToArray(), 0, NewName, 0, NameToInject.ToArray().Length);
                    Array.Copy(NewName, 0, mentry.UncompressedData, OffsetToUse, NewName.Length);
                    mentry.CompressedData = Zlibber.Compressor(mentry.UncompressedData);
                    treeview.SelectedNode.Tag = mentry;
                }
                treeview.SelectedNode = child;
            }
            else if (treeview.SelectedNode.Tag as string == "Model Material Reference")
            {

                treeview.Update();
                ModelEntry mentry = new ModelEntry();
                TreeNode parent = treeview.SelectedNode.Parent;
                TreeNode child = treeview.SelectedNode;
                treeview.SelectedNode = parent;
                TreeNode folder = treeview.SelectedNode;
                parent = treeview.SelectedNode.Parent;
                treeview.SelectedNode = parent;
                mentry = treeview.SelectedNode.Tag as ModelEntry;
                int OffsetToUse;

                if (mentry != null)
                {
                    for(int w = 0; w < folder.Nodes.Count; w++)
                    {
                        //Now for the actual file update.                    
                        List<byte> NameToInject = new List<byte>();
                        NameToInject.AddRange(Encoding.ASCII.GetBytes(folder.Nodes[w].Text));
                        OffsetToUse = mentry.MaterialsOffset + (128 *(w));
                        byte[] NewName = new byte[128];

                        Array.Copy(NameToInject.ToArray(), 0, NewName, 0, NameToInject.ToArray().Length);
                        Array.Copy(NewName, 0, mentry.UncompressedData, OffsetToUse, NewName.Length);
                        mentry.CompressedData = Zlibber.Compressor(mentry.UncompressedData);
                        parent.Tag = mentry;

                    }
                    treeview.SelectedNode = child;

                    //Gets the names first.
                    //List<NamesToUse>

                    //Now for the actual file update.


                }

            }

            Mainfrm.OpenFileModified = true;
            treeview.EndUpdate();

            //Closes form with changes made above.
            DialogResult = DialogResult.OK;
            Hide();

        }
    }
}
