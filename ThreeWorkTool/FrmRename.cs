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
            this.Show();
        }

        public void ShowItItem()
        {
            txtRename.Text = Mainfrm.TreeSource.SelectedNode.Text;
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            OriginalName = Mainfrm.TreeSource.SelectedNode.Text;
            this.Show();
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
                if (c.Text == txtRename.Text)
                {
                    MessageBox.Show("That name already exists on a resource on the same level. \nTry a different name.", "Oh Boy");
                    return;
                }
            }

            //Changes the name to what was chosen. Should reflect on the Treeview.
            treeview.SelectedNode.Text = txtRename.Text;
            treeview.SelectedNode.Name = txtRename.Text;

            Mainfrm.OpenFileModified = true;

            //Closes form with changes made above.
            DialogResult = DialogResult.OK;
            Hide();

        }
    }
}
