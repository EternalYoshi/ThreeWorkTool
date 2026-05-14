using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ThreeWorkTool.Resources;
using ThreeWorkTool.Resources.Geometry;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

namespace ThreeWorkTool
{
    public partial class FrmModelViewer : Form
    {
        private GLControl GlControl;
        public FrmMainThree Mainfrm { get; set; }
        private TheRenderer renderer;
        private KeyboardStateHandler Kboard;
        public ModelEntry modelEntry;
        public List<ModelBoneEntry> Joints;
        public List<ModelPrimitiveEntry> Polygons;
        public bool ShowFloor = true;
        public bool UseTypeB = false;
        public bool ShowJoints = true;
        public bool ShowPolygons = true;
        private System.Windows.Forms.Timer rTimer;

        //From the OpenTK guide.
        float[] Testvertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
        };

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys bare = keyData & Keys.KeyCode;

            if (bare == Keys.Up || bare == Keys.Down ||
                bare == Keys.Left || bare == Keys.Right)
            {
                Kboard.KeyDown(bare);
                return true; // tells WinForms "I handled this, stop processing it"
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public FrmModelViewer()
        {
            InitializeComponent();

            Kboard = new KeyboardStateHandler();

            //This creates and configures the GLControl.
            GlControl = new GLControl(new GraphicsMode(32, 24, 0, 4));
            GlControl.Dock = DockStyle.Fill;
            GlControl.Load += (s, e) => renderer.Load(this);
            GlControl.Paint += (s, e) => renderer.Render(ShowFloor, ShowJoints, ShowPolygons);
            GlControl.Resize += (s, e) => renderer.Resize(GlControl.Width, GlControl.Height);
            GlControl.KeyDown += (s, e) => Kboard.KeyDown(e.KeyCode);

            //Meant to hopefully fix crashes when closing.
            GlControl.HandleDestroyed += (s, e) => renderer.Closing();
            //Keyboard stuff.

            //Forwards Key Events to the tracker.
            //KeyPreview should catch key inputs before the form does.
            this.KeyPreview = true;
            this.KeyDown += (s, e) => Kboard.KeyDown(e.KeyCode);
            this.KeyUp += (s, e) => Kboard.KeyUp(e.KeyCode);

            //This is for zooming.
            this.MouseWheel += (s, e) => renderer.Zoom(e.Delta);

            //Meant to pass the Ctrl key to replicate a certain modding tool's camera controls.
            GlControl.MouseDown += (s, e) => {
                if (e.Button == MouseButtons.Right)
                    renderer.StartDrag(e.Location, ModifierKeys.HasFlag(Keys.Control));
            };

            GlControl.MouseUp += (s, e) => {
                if (e.Button == MouseButtons.Right)
                    renderer.StopDrag();
            };

            GlControl.MouseMove += (s, e) => renderer.FastMove(e.Location, ModifierKeys.HasFlag(Keys.Control));

            //Meant to prevent the mouse from moving the camera when the user clicks on the menu bar or anything on it.
            GlControl.MouseLeave += (s, e) => renderer.StopDrag();

            //GlControl.KeyDown += (s, e) => Kboard.KeyDown(e.KeyCode);
            //GlControl.KeyUp += (s, e) => Kboard.KeyUp(e.KeyCode);

            this.Controls.Add(GlControl);

            renderer = new TheRenderer(GlControl, Kboard);

            rTimer = new System.Windows.Forms.Timer { Interval = 16 };
            rTimer.Tick += (s, e) => GlControl.Invalidate();
            rTimer.Start();

        }

        public void GetModelDetails(ModelEntry model)
        {
            modelEntry = model;
            Joints = modelEntry.Bones;
            Polygons = modelEntry.Primitives;

            //Now to change the window name.
            this.Text =  "Model Viewer - Under Construction - " + modelEntry.TrueName + ".mod";
        }

        private void FrmModelViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            rTimer.Stop();
            //renderer.Closing();
            //base.OnFormClosing(e);
        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog cDialog = new ColorDialog())
            {
                if (cDialog.ShowDialog() == DialogResult.OK)
                {
                    //Gets the proper GLControl context.
                    GlControl.MakeCurrent();
                    GL.ClearColor(cDialog.Color);
                    //Activates a repaint.
                    GlControl.Invalidate();
                    //GlControl.BackColor = cDialog.Color;
                }
            }
        }

        private void floorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFloor = floorToolStripMenuItem.Checked;
        }

        private void jointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowJoints = jointsToolStripMenuItem.Checked;
        }

        private void polygonsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPolygons = polygonsToolStripMenuItem.Checked;
        }

        private void btnResetCamera_Click(object sender, EventArgs e)
        {
            renderer.ResetCamera();
        }
    }
}
