using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreeWorkTool
{
    public partial class FrmReplace : Form
    {

        private static ThreeSourceTree treeview;
        //private static TreeView treeview;
        private static TreeNode node_;
        //private static ThreeSourceNodeBase node_;
        public FrmMainThree Mainfrm { get; set; }

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
            //Gets the text, then replaces every instance of the search term with the new term.
            string text = Mainfrm.txtRPList.Text;
            text = text.Replace(txtReplaceFind.Text, txtReplaceReplace.Text);
            Mainfrm.txtRPList.Text = text;


        }


    }
}
