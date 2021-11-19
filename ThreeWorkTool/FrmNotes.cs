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
    public partial class FrmNotes : Form
    {
        public FrmNotes()
        {
            InitializeComponent();
        }

        private void btnNoteOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }
    }
}
