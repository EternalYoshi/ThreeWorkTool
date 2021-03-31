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

namespace ThreeWorkTool
{
    public partial class FrmRename : Form
    {
        private static ThreeSourceTree treeview;
        //private static TreeView treeview;
        private static TreeNode node_;
        //private static ThreeSourceNodeBase node_;
        private static ArcEntryWrapper entry_;
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

                //Checks the nodes in the same directory for existing name AND extension and will stop if there's a node with the same type in the same directory.
                if (c.Text == txtRename.Text)
                {
                    if (ae != se && ae.FileExt == se.FileExt)
                    {
                        MessageBox.Show("That name already exists on a resource of the same type on the same level. \nTry a different name.", "Oh Boy");
                        return;
                    }
                }
            }

            //Changes the name to what was chosen. Should reflect on the Treeview.
            treeview.SelectedNode.Text = txtRename.Text;
            treeview.SelectedNode.Name = txtRename.Text;

            //Ensures the TrueName gets change so it gets reflected in the save.
            ArcEntry aey = new ArcEntry();
            if(treeview.SelectedNode.Tag is ArcEntry)
            {
                aey = treeview.SelectedNode.Tag as ArcEntry;
                aey.TrueName = txtRename.Text;
                
            }
            else if (treeview.SelectedNode.Tag != null && treeview.SelectedNode.Tag as string == "Folder")
            {

            }


            Mainfrm.OpenFileModified = true;

            //Closes form with changes made above.
            DialogResult = DialogResult.OK;
            Hide();

        }
    }
}
