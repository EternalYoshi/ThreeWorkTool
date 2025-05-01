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
    public partial class FrmFontSlider : Form
    {
        private static ThreeSourceTree treeview;
        public FrmMainThree Mainfrm { get; set; }

        public FrmFontSlider()
        {
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Mainfrm.TreeSource.Font = new Font("Microsoft Sans Serif",trackBar1.Value);
        }

        private void btnFontSliderClose_Click(object sender, EventArgs e)
        {
            Hide();
            return;
        }

        private void trackBarNodeSpacing_Scroll(object sender, EventArgs e)
        {
            //Mainfrm.TreeSource.ClientSize = new Size(
            //trackBarNodeSpacing.Value,
            //trackBarNodeSpacing.Value);

            Mainfrm.TreeSource.ItemHeight = trackBarNodeSpacing.Value;
            Mainfrm.TreeSource.ExpandAll();
        }

        public void ShowIt()
        {
            //Loads current values into sliders.
            trackBarNodeSpacing.Value = Mainfrm.TreeSource.ItemHeight;
            trackBar1.Value = Convert.ToInt32(Mainfrm.TreeSource.Font.Size);

            this.ShowDialog();
        }

        private void FrmFontSlider_Load(object sender, EventArgs e)
        {

        }
    }
}
