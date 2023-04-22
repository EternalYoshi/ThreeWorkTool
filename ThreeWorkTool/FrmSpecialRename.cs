﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Archives;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool
{
    public partial class FrmSpecialRename : Form
    {
        public FrmMainThree Mainfrm { get; set; }
        private static ThreeSourceTree treeview;
        private static TreeNode node_;

        private List<string> MaterialNames = new List<string>
        { "XfB_N__E_m01_7",
"XfB_N__E_A0__m01_",
"XfBAN__E0__m00_7",
"XfBA_E0__m04_",
"XfB_N__E_m00_24",
"XfB_N__E_m00_22",
"XfB_N__E_m04_1",
"XfB_N__E_m00_3",
"XfBA_IW_02__m11_",
"XfB_N__E_m07_1",
"XfBAN__EW_0__m00_",
"XfB_N__E_m07_",
"XfB_N__E0__m00_",
"XfB__E_m00_9",
"XfBAN__E_I0__m00_",
"XfBAN_0__m00_2",
"XfB_N__m00_8",
"XfBA_IW_34__m04_",
"XfB__E_m00_",
"XfBA_IW_31__m31_",
"XfB_N__E_m10_1",
"XfB_N__E_m00_23",
"XfBAW_0__m00_",
"XfBAN__E0__m00_13",
"XfB__E_m00_4",
"XfB_N__m00_5",
"XfBA_IW_13__m22_",
"XfB__m01_1",
"XfB__E_m01_1",
"XfBA_EW_0__m00_1",
"XfB__E_m07_",
"XfBA_EW_0__m00_",
"XfBA0__m00_1",
"XfBAN__E0__m04_",
"XfBA_I0__m02_",
"XfB_N__E_m01_2",
"XfB_N__E_m00_21",
"XfBA_IW_03__m21_",
"XfBA_E0__m00_2",
"XfBA_IW_01__m30_",
"XfB__m00_1",
"XfB_N__E_m05_",
"XfB_N__E_m09_",
"XfB__I_m00_",
"XfB_N__E_m00_18",
"XfB_N__ED__m03_5",
"XfB_N__E_m00_7",
"XfB_N__E_m03_2",
"XfB_N__E_m02_4",
"XfB_N__m00_4",
"XfB_N__E_m00_26",
"XfB_N__ED__m03_1",
"XfB_N__E_m03_4",
"XfB_N__E_m00_12",
"XfB__E_m05_3",
"XfB_N__E_m01_3",
"XfB_N__E_m02_",
"XfBA_IW_13__m13_",
"XfB_N__E_m01_6",
"XfBA_IW_03__m03_",
"XfB_N__ED__m03_2",
"XfB_N__E_m00_1",
"XfB__E_m00_1",
"XfBA_IW_01__m01_",
"XfBA_IW_0__m00_",
"XfBAN__E0__m00_2",
"XfB__E_m05_1",
"XfB_N__E_m00_6",
"XfB__E_m05_7",
"XfBA_IW_23__m23_",
"XfB_N__E_A0__m00_",
"XfB_N__E_m08_1",
"XfBA0__m00_",
"XfBA_IW_0__m04_",
"XfB_N__E_m03_3",
"XfB__E_m00_6",
"XfB_N__E_m14_",
"XfBAN_N__E0_m01_6",
"XfB_N__EW__m00_",
"XfB_N__E_m00_25",
"XfBA_IW_0__m03_",
"XfB_N__m00_2",
"XfB_N__E_m00_28",
"XfB_N__E_m13_",
"XfBAN__EW_0__m02_",
"XfB_N__E_m00_20",
"XfB_N__E_m02_1",
"XfB__m08_",
"XfB_N__E_m00_16",
"XfBA_IW_00__m00_",
"XfB_N__ED__m03_",
"XfBA_IW_0__m06_1",
"XfB_N__E_m05_1",
"XfB_N__E_m04_",
"XfBAN__E1__m00_",
"XfB_W__m00_",
"XfBA_I0__m00_1",
"XfB_N__E_m01_4",
"XfBAN_0__m00_1",
"Scene_Material",
"XfB__m02",
"XfB__E_m04_",
"XfB_N__E_m01_5",
"XfB_N__E_m06_1",
"XfBAN_0__m00_",
"XfB_N__ED__m03_3",
"XfB__E_m00_8",
"XfBAN__E0__m01",
"XfBA0__m00_2",
"XfB_N__E_m00_10",
"XfB_N__E_A_I0__m01_1",
"XfB__E_m01_",
"XfB_N__E_m00_4",
"XfB_N__E0__m01_",
"XfB_N__E_A_I0__m01_",
"XfB_N__E_m00_5",
"XfBAN__E0__m00_1",
"XfB_N__E_m00_8",
"XfB_N__E_m02_5",
"XfB_N__E_m00_15",
"XfB_N__m00_",
"XfB_N__E_m00_14",
"XfBAN__E0__m00_4",
"XfBA0__m00_3",
"lambert1",
"XfBAW_0__m01_",
"XfB_N__E_m04_2",
"XfB__E_m06_2",
"XfB_N__E_m03_1",
"XfB_N__E_m00_13",
"XfB_N__E_m06_2",
"XfB_N__E_m02_3",
"XfB__E_m05_",
"XfB_N__E_m11_",
"XfB_N__ED__m03_6",
"XfBA_I00__m00_",
"XfB_N__E_m00_19",
"XfB__I_m00_6",
"XfB__E_m06_",
"XfB_N__E_m00_29",
"XfBAN__E0__m03_",
"XfBAN__E0__m00_",
"XfB_N__m00_6",
"XfBA_E0__m00_",
"XfB_N__m00_1",
"XfB__m00_4",
"XfBAN__E0__m06_",
"XfBA0__m02_",
"XfB_N__E_A0__m00_1",
"XfB_N__E_m08_",
"XfB__m00_3",
"XfBA_EW_0__m00_4",
"XfB_N__m02_",
"XfB_N__E_m01_1",
"XfB_N__m00_7",
"XfB_N__E_m00_11",
"XfB_N__E_m02_2",
"XfBAN__E0__m01_6",
"XfB_N__E_m01",
"XfB_N__E_m00_30",
"XfB__E_m08_1",
"XfB_N__E_m12_",
"XfB_N__E_m00_27",
"XfB_N__m00_3",
"XfB_N__ED__m03_4",
"XfB_N__E_m00_",
"XfB__E_m00_7",
"XfB__E_m08_",
"XfB_N__E_m1",
"XfB__I_m05_",
"XfBA0__m03_",
"XfB__m01_",
"XfB__E_m00_3",
"XfB__E_m00_2",
"XfB_N__E_m15_",
"XfB_N__E_m01_8",
"XfB_N__E_m00_9",
"XfBA_IW_11__m02_",
"XfBAN__R0__m00_",
"XfB_N__ED__m03_7",
"XfBAN__E0__m00_10",
"XfB_N__E_m00_2",
"XfB__E_m00_5",
"XfBAN__E0__m00_5",
"XfB__m00_",
"XfB__E_m05_5",
"XfB_N__E_m01_",
"XfBAN__E0__m00_3",
"XfB__m06_",
"XfB_N__E_m14_1",
"XfB_N__E_m00_17",
"XfB__E_I_m00_",
"XfB__I_m00_4",
"XfB__E_m07_1",
"XfBA_IW_12__m12_",
"XfB_N__E_m05_2",
"XfB_N__E_m06_",
"XfBAW_0__m04_1",
"XfB_N__E_m03_",
"XfB__m00_2",
"XfBAN__E0__m00_12",
"XfBA_E0__m00_1"
 };

        public FrmSpecialRename()
        {
            InitializeComponent();
        }

        public void ShowIt()
        {
            cmbSpcRename.Items.AddRange(MaterialNames.ToArray());
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            this.ShowDialog();
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            //frm = this as FrmSpecialRename;
        }

        private void btnSpcRenameCancel_Click(object sender, EventArgs e)
        {
            //Closes the form without making changes.
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnSpcRenameOK_Click(object sender, EventArgs e)
        {
            string newname = cmbSpcRename.Text;

            //Checks for blank/null names.
            if (newname == null || newname == "")
            {
                MessageBox.Show("This must have a name and cannot be null!", "Ahem");
                return;
            }

            //Changes the name to what was chosen. Should reflect on the Treeview.
            treeview.SelectedNode.Text = cmbSpcRename.Text;
            treeview.SelectedNode.Name = cmbSpcRename.Text;

            //Ensures the TrueName gets change so it gets reflected in the save.
            ArcEntry aey = new ArcEntry();
            if (treeview.SelectedNode.Tag != null && treeview.SelectedNode.Tag is MaterialMaterialEntry)
            {
                MaterialMaterialEntry matref = treeview.SelectedNode.Tag as MaterialMaterialEntry;
                MaterialEntry mentry = new MaterialEntry();
                TreeNode parent = treeview.SelectedNode.Parent;
                TreeNode child = treeview.SelectedNode;
                treeview.SelectedNode = parent;
                parent = treeview.SelectedNode.Parent;
                treeview.SelectedNode = parent;
                mentry = treeview.SelectedNode.Tag as MaterialEntry;

                //Let's update the typehash.
                string newhash = "";
                newhash = CFGHandler.NameToMaterialHash(newhash, newname);
                long newlong = Convert.ToInt64(newhash);
                string finhash = "";
                finhash = newlong.ToString("X8");
                matref.TypeHash = finhash;
                matref.NameHash = finhash;

                if (mentry != null)
                {

                    mentry.Materials[matref.Index] = matref;


                }


            }

            Mainfrm.OpenFileModified = true;

            //Closes form with changes made above.
            DialogResult = DialogResult.OK;
            Hide();

        }
    }
}
