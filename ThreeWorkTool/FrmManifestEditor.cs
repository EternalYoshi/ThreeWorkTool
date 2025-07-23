using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.AnimNodes;
using ThreeWorkTool.Resources.Wrappers.ExtraNodes;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;
using static ThreeWorkTool.Resources.Utility.ENumerators;

namespace ThreeWorkTool
{
    public partial class FrmManifestEditor : Form
    {
        public FrmManifestEditor()
        {
            InitializeComponent();
        }

        public FrmMainThree Mainfrm { get; set; }

        private void btnUseManifest_Click(object sender, EventArgs e)
        {
            //From the editor text to the ManifestText.
            //Mainfrm.Manifest = txtManifest.Text.Split(new string[] {"\\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
            Mainfrm.Manifest = txtManifest.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

            /*
            for(int i = 0; i < Mainfrm.Manifest.Count; i++)
            {
//                Mainfrm.Manifest[i] = Mainfrm.Manifest[i].Substring(Mainfrm.Manifest[i].Length - 2);
            }
            */

            //Closes form with changes made above.
            DialogResult = DialogResult.OK;
            Hide();
            txtManifest.Text = "";
        }

        private void btnDontUseManifest_Click(object sender, EventArgs e)
        {

            //Closes the form without making changes.
            DialogResult = DialogResult.Cancel;
            Hide();

        }

        public void LoadText(List<string> ManifestText, FrmMainThree frmMain)
        {
            txtManifest.Text = "";
            string Temp = "";
            Mainfrm = frmMain;
            foreach (string str in ManifestText)
            {
                Temp = str;
                txtManifest.Text = txtManifest.Text + Temp + "\n";
            }

            this.ShowDialog();

        }

        public static int GetIntFromEnumName(string name)
        {
            return (int)Enum.Parse(typeof(KnownExtensions), name);
        }

        public void RefreshManifest(List<string> ManifestText, FrmMainThree frmMain)
        {

            //Goes to top node to begin iteration.
            TreeNode tn = frmMain.FindRootNode(frmMain.TreeSource.SelectedNode);
            frmMain.TreeSource.SelectedNode = tn;

            List<TreeNode> Nodes = new List<TreeNode>();
            frmMain.AddChildren(Nodes, frmMain.TreeSource.SelectedNode);
            string Temp = "";
            int nowcount = 0;

            //Empties old manifest and fills it in with the current files inside the currently opened arc.
            Mainfrm.Manifest.Clear();
            foreach (TreeNode treno in Nodes)
            {
                if ((treno.Tag as string != null && treno.Tag as string == "Folder") || treno.Tag as string == "MaterialChildMaterial" || treno.Tag as string == "Model Material Reference" ||
                    treno.Tag as string == "Model Primitive Group" || treno.Tag is MaterialTextureReference || treno.Tag is LMTM3AEntry || treno.Tag is ModelBoneEntry
                    || treno.Tag is MaterialMaterialEntry || treno.Tag is ModelGroupEntry || treno.Tag is Mission || treno.Tag is EffectNode || treno.Tag is EffectFieldTextureRefernce 
                    || treno.Tag is ModelPrimitiveEntry || treno.Tag is ModelEnvelopeEntry || treno.Tag is StageObjLayoutGroup || treno.Tag is STQRNode || treno.Tag is STQREventData || treno.Tag is LMTTrackNode)
                {

                }
                else
                {
                    nowcount++;
                    ArcEntryWrapper File = treno as ArcEntryWrapper;
                    if (File != null)
                    {
                        Temp = File.FullPath + File.FileExt;
                        frmMain.Manifest.Add(Temp);
                    }

                }

            }


            txtManifest.Text = "";
            string Temstr = "";
            Mainfrm = frmMain;
            foreach (string str in Mainfrm.Manifest)
            {
                Temstr = str.Substring(str.IndexOf("\\") + 1);
                txtManifest.Text = txtManifest.Text + Temstr + "\n";
            }

            //Corrects File Order.
            List<string> Ordered = (Mainfrm.Manifest.OrderBy(fn => GetIntFromEnumName((Path.GetExtension(fn).Substring(1))))).ToList();
            txtManifest.Text = "";
            foreach (string str in Ordered)
            {
                Temstr = str.Substring(str.IndexOf("\\") + 1);
                txtManifest.Text = txtManifest.Text + Temstr + "\n";
            }
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            DialogResult refsh = MessageBox.Show("Refreshing this list means the program will check all nodes for files and list them in that order. You will lose any unique ordering you have now. Are you sure you want to do this?", "Caution", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            if (refsh == DialogResult.Yes)
            {
                RefreshManifest(Mainfrm.Manifest, Mainfrm);

            }
            else
            {

            }

        }

    }
}
