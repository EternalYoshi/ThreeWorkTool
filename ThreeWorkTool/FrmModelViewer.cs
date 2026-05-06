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

namespace ThreeWorkTool
{
    public partial class FrmModelViewer : Form
    {
        private GLControl GlControl;
        public FrmMainThree Mainfrm { get; set; }
        private TheRenderer renderer;
        private KeyboardStateHandler Kboard;

        private System.Windows.Forms.Timer rTimer;

        //From the OpenTK guide.
        float[] Testvertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
        };

        public FrmModelViewer()
        {
            InitializeComponent();

            Kboard = new KeyboardStateHandler();

            //This creates and configures the GLControl.
            GlControl = new GLControl(new GraphicsMode(32, 24, 0, 4));
            GlControl.Dock = DockStyle.Fill;
            GlControl.Load += (s, e) => renderer.Load();
            GlControl.Paint += (s, e) => renderer.Render();
            GlControl.Resize += (s, e) => renderer.Resize(GlControl.Width, GlControl.Height);
            GlControl.KeyDown += (s, e) => Kboard.KeyDown(e.KeyCode);

            //Keyboard stuff.

            //Forwards Key Events to the tracker.
            //KeyPreview should catch key inputs before the form does.
            this.KeyPreview = true;
            this.KeyDown += (s, e) => Kboard.KeyDown(e.KeyCode);
            this.KeyUp += (s, e) => Kboard.KeyUp(e.KeyCode);

            //GlControl.KeyDown += (s, e) => Kboard.KeyDown(e.KeyCode);
            //GlControl.KeyUp += (s, e) => Kboard.KeyUp(e.KeyCode);

            this.Controls.Add(GlControl);

            renderer = new TheRenderer(GlControl, Kboard);

            rTimer = new System.Windows.Forms.Timer { Interval = 16 };
            rTimer.Tick += (s, e) => GlControl.Invalidate();
            rTimer.Start();

        }

        private void FrmModelViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            rTimer.Stop();
            //renderTimer.Stop();
            //floor.Dispose();
            base.OnFormClosing(e);
        }
    }
}
