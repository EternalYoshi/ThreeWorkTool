using ScintillaNET;
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
        public MaterialEntry material;
        public ModelEntry model;
        public FrmMainThree Mainfrm { get; set; }
        public bool isModified = false;
        public bool CaseSensitive = false;

        public FrmTxtEditor()
        {
            InitializeComponent();
        }

        private void CheckCase()
        {
            if (checkBoxCaseSensitive.Checked)
            {
                CaseSensitive = true;
            }
            else
            {
                CaseSensitive = false;
            }
        }

        public void ShowTxtEditor()
        {
            treeview = Mainfrm.TreeSource;
            nodeTxt = treeview.SelectedNode;

            if (nodeTxt.Tag as MSDEntry != null)
            {
                msd = nodeTxt.Tag as MSDEntry;
                txtMSDBoxV2.ClearAll();
                MSDEntry.LoadMSDInTexEditorForm(txtMSDBoxV2, msd);
                this.Text = "MSD Editor - " + nodeTxt.Text;
                this.ShowDialog();
            }
            else if (nodeTxt.Tag as MaterialEntry != null)
            {
                material = nodeTxt.Tag as MaterialEntry;
                //Starts the proccess of converting the Material file to a .yml file.

                //
                //this.Text = "YML Editor - " + nodeTxt.Text;
                //this.ShowDialog();
            }

            CheckCase();
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
            if (isModified == true)
            {
                Mainfrm.UpdateMSD(this, this.txtMSDBoxV2);
            }
        }

        private void FrmTxtEditor_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            int indexttotext = -1;
            if (CaseSensitive)
            {
                indexttotext = txtMSDBoxV2.FindText(ScintillaNET.SearchFlags.MatchCase, txtFind.Text, txtMSDBoxV2.CurrentPosition, 0);
                if (indexttotext != -1)
                {
                    this.txtMSDBoxV2.SetSelection(indexttotext, indexttotext);
                    this.txtMSDBoxV2.ScrollCaret();
                    this.txtMSDBoxV2.Focus();
                }
            }
            else
            {
                indexttotext = txtMSDBoxV2.FindText(ScintillaNET.SearchFlags.None, txtFind.Text, txtMSDBoxV2.CurrentPosition, 0);
                if (indexttotext != -1)
                {
                    this.txtMSDBoxV2.SetSelection(indexttotext, indexttotext);
                    this.txtMSDBoxV2.ScrollCaret();
                    this.txtMSDBoxV2.Focus();
                }
            }
            if (indexttotext == -1)
            {
                lblFind.Text = "That specified text was not found.";
            }

        }

        private void btnLineJump_Click(object sender, EventArgs e)
        {

            var isNumeric = int.TryParse(txtLineNumber.Text, out int LineToJump);

            //If the inputted text is numeric, scrolls to that line number.
            if (isNumeric == true)
            {

                if (LineToJump > txtMSDBoxV2.Lines.Count())
                {
                    lblFind.Text = "There are not that many lines here.";
                }
                else if (LineToJump < 1)
                {
                    lblFind.Text = "That's not a valid line number.";
                }
                else
                {
                    this.txtMSDBoxV2.GotoPosition(txtMSDBoxV2.Lines[(LineToJump - 1)].Position);
                    this.txtMSDBoxV2.SetSelection(txtMSDBoxV2.Lines[(LineToJump - 1)].Position, txtMSDBoxV2.Lines[(LineToJump - 1)].Position);
                    this.txtMSDBoxV2.ScrollCaret();
                    this.txtMSDBoxV2.Focus();

                    //Empties the text up there.
                    lblFind.Text = "";

                    //int index = this.txtMSDBoxV2.GetFirstCharIndexFromLine(LineToJump);
                    //this.txtMSDBoxV2.Select(index, 0);
                    //this.txtMSDBoxV2.ScrollToCaret();

                }

            }
            else
            {
                lblFind.Text = "That is not a valid number. Enter a valid line number to jump to.";

            }

        }

        private void txtMSDBoxV2_Click(object sender, EventArgs e)
        {

        }

        private void btnFindDown_Click(object sender, EventArgs e)
        {
            int indexttotext = -1;
            if (CaseSensitive)
            {
                indexttotext = txtMSDBoxV2.FindText(ScintillaNET.SearchFlags.MatchCase, txtFind.Text, txtMSDBoxV2.CurrentPosition, txtMSDBoxV2.TextLength);
                if (indexttotext != -1)
                {
                    txtMSDBoxV2.GotoPosition(indexttotext);
                    this.txtMSDBoxV2.SetSelection(indexttotext, indexttotext);
                    this.txtMSDBoxV2.ScrollCaret();
                    this.txtMSDBoxV2.Focus();
                }
            }
            else
            {
                indexttotext = txtMSDBoxV2.FindText(ScintillaNET.SearchFlags.None, txtFind.Text, txtMSDBoxV2.CurrentPosition, txtMSDBoxV2.TextLength);
                if (indexttotext != -1)
                {
                    txtMSDBoxV2.GotoPosition(indexttotext);
                    this.txtMSDBoxV2.SetSelection(indexttotext, indexttotext);
                    this.txtMSDBoxV2.ScrollCaret();
                    this.txtMSDBoxV2.Focus();
                }
            }
            if (indexttotext == -1)
            {
                lblFind.Text = "That specified text was not found.";
            }

            ////int indexttotext = txtMSDBoxV2.Find(txtFind.Text);
            //txtMSDBoxV2.FindText()
            //if (indexttotext == -1)
            //{
            //    lblFind.Text = "That specified text was not found.";
            //}
            //else
            //{
            //    //return indexttotext;
            //    lblFind.Text = " ";
            //    this.txtMSDBox.Select(indexttotext, txtFind.Text.Length);
            //    this.txtMSDBox.Focus();
            //}
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            CheckCase();
        }

        private void FrmTxtEditor_Load(object sender, EventArgs e)
        {
            //For Making the line count visible.
            txtMSDBoxV2.Margins[0].Type = ScintillaNET.MarginType.Number;
            txtMSDBoxV2.Margins[0].Width = 45;

            //For making the margin darker.
            txtMSDBoxV2.SetFoldMarginColor(true, Color.FromArgb(42, 42, 42));
            txtMSDBoxV2.SetFoldMarginHighlightColor(true, Color.FromArgb(42, 42, 42));

            txtMSDBoxV2.Styles[Style.LineNumber].BackColor = Color.FromArgb(20, 20, 20);
            txtMSDBoxV2.Styles[Style.LineNumber].ForeColor = Color.Gray;


        }

        private void txtMSDBoxV2_TextChanged(object sender, EventArgs e)
        {
            Mainfrm.OpenFileModified = true;
            isModified = true;
        }
    }
}
