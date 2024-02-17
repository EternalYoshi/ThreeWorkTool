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
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();
        }

        private void FrmAbout_Load(object sender, EventArgs e)
        {

        }

        private void btnAboutOK_Click(object sender, EventArgs e)
        {
            Hide();
            return;
        }

        private void lblURLText_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/EternalYoshi/ThreeWorkTool");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
