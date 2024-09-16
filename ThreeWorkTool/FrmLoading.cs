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
    public partial class FrmLoading : Form
    {
        public FrmMainThree Mainfrm { get; set; }

        //public Action Worker { get; set; }

        public FrmLoading()
        {
            InitializeComponent();
        }

        public void ActivateForm(int FileCount)
        {
            lblFileCount.Text = "/" + Convert.ToString(FileCount);
            lblCurIndex.Text = "0"; 
            this.Show();

        }

        public void TextUpdate(int CurrentIndex, string CurrentFile)
        {

            lblCurFile.Text = CurrentFile;
            lblCurIndex.Text = CurrentIndex.ToString();
            //return 0;
        }

        public void Finish()
        {
            Hide();
        }

    }
}
