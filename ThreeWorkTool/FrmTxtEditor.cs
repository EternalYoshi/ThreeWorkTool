using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool
{
    public partial class FrmTxtEditor : Form
    {
        private static ThreeSourceTree treeview;
        private static TreeNode nodeTxt;
        public MSDEntry msd;
        public FrmMainThree Mainfrm { get; set; }
        public bool isModified = false;


        public FrmTxtEditor()
        {
            InitializeComponent();
        }

        public void ShowTxtEditor()
        {
            treeview = Mainfrm.TreeSource;
            nodeTxt = treeview.SelectedNode;
            msd = nodeTxt.Tag as MSDEntry;
            MSDEntry.LoadMSDInTexEditorForm(txtMSDBox,msd);
            this.Text = "MSD Editor - " + nodeTxt.Text;
            this.ShowDialog();
        }

        private void FrmTxtEditor_TextChanged(object sender, EventArgs e)
        {
            //Mainfrm.OpenFileModified = true;
        }

        private void txtMSDBox_TextChanged(object sender, EventArgs e)
        {
            Mainfrm.OpenFileModified = true;
            isModified = true;
        }

        private void FrmTxtEditor_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FrmTxtEditor_FormClosed(object sender, FormClosedEventArgs e)
        {            
            if (isModified == true)
            {
                Mainfrm.UpdateMSD(txtMSDBox);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            int indexttotext = txtMSDBox.Find(txtFind.Text);
            if(indexttotext == -1)
            {
                lblFind.Text = "That specified text was not found.";
            }
            else
            {
                //return indexttotext;
                lblFind.Text = " ";
                this.txtMSDBox.Select(indexttotext, txtFind.Text.Length);
                this.txtMSDBox.Focus();
            }

        }

        private void btnLineJump_Click(object sender, EventArgs e)
        {

            var isNumeric = int.TryParse(txtLineNumber.Text, out int LineToJump);

            //If the inputted text is numeric, scrolls to that line number.
            if (isNumeric == true)
            {

                if (LineToJump > txtMSDBox.Lines.Count())
                {
                    lblFind.Text = "There are not that many lines here.";
                }
                else
                {

                    int index = this.txtMSDBox.GetFirstCharIndexFromLine(LineToJump);
                    this.txtMSDBox.Select(index, 0);
                    this.txtMSDBox.ScrollToCaret();

                }

            }
            else
            {
                lblFind.Text = "That is not a valid number. Enter a valid line number to jump to.";

            }

        }
    }
}
