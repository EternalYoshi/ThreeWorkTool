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
using ThreeWorkTool.Resources.Wrappers.AnimNodes;

namespace ThreeWorkTool
{
    public partial class FrmTrackEditor : Form
    {
        private static ThreeSourceTree treeview;
        private static TreeNode nodeTxt;
        public LMTM3AEntry M3a;
        public LMTTrackNode tracks;
        bool EntryChanged = false;
        public FrmMainThree Mainfrm { get; set; }
        public bool isModified = false;

        public FrmTrackEditor()
        {
            InitializeComponent();
        }

        public void ShowTrackEditor()
        {
            treeview = Mainfrm.TreeSource;
            nodeTxt = treeview.SelectedNode;

            if (nodeTxt.Tag as LMTM3AEntry != null)
            {
                M3a = nodeTxt.Tag as LMTM3AEntry;
                this.Text = "Track Editor - " + nodeTxt.Text;
                this.ShowDialog();
            }

        }



    }
}
