using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ThreeWorkTool.Resources.Geometry;

namespace ThreeWorkTool.Resources
{
    public class TheRenderer
    {

        private readonly GLControl GlControl;
        private int SProgram, Vao, Vbo;
        private Checkerboard Floor;
        private Matrix4 Projection, View;
        int VertexBufferObject;
        private CameraTake1 Cam;
        private KeyboardStateHandler KeyStateHandler;
        public DateTime PrevFrameTime;
        private readonly Stopwatch sClock = Stopwatch.StartNew();
        private double LastTime;
        //private bool IsDragging = false;
        
        private enum DragMode { None, Pan, Rotate }
        private DragMode DraggingMode = DragMode.None;
        private Point PrevMousePos;

        public void StartDrag(Point pos, bool CtrlHeld)
        {
            DraggingMode = CtrlHeld ? DragMode.Rotate : DragMode.Pan;
            PrevMousePos = pos;
        }

        //So this is a lambda expression for an anonymous function..?
        public void StopDrag()
        {
            DraggingMode = DragMode.None;
        }

        //This runs when the mouse moves.
        public void FastMove(Point position, bool control)
        {
            if (DraggingMode == DragMode.None) return;

            //Updates the Camera control mode based on the state of the CTRL key.
            DraggingMode = control ? DragMode.Rotate : DragMode.Pan;

            float FX = position.X - PrevMousePos.X;
            float FY = position.Y - PrevMousePos.Y;
            PrevMousePos = position;

            if(DraggingMode == DragMode.Rotate)
            {
                Cam.Rotation(FX, FY);
            }
            else if (DraggingMode == DragMode.Pan)
            {
                Cam.Pan(FX, FY);
            }
            
        }

        //From the OpenTK guide.
        float[] Testvertices = {
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
        };

        public TheRenderer(GLControl control, KeyboardStateHandler keyboard)
        {
            GlControl = control;
            KeyStateHandler = keyboard;
        }

        public void Load()
        {
            GL.ClearColor(0.1f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            //Setup Shaders and Buffers Here!
            //

            Cam = new CameraTake1();
            Floor = new Checkerboard
            {
                GridSize = 20,
                TileSize = 50.0f,
                ColorA = new Color4(0.9f, 0.9f, 0.9f, 1.0f),
                ColorB = new Color4(0.2f, 0.2f, 0.2f, 1.0f)
            };
            Floor.Load();

            

            PrevFrameTime = DateTime.Now;

            //Sets up View Matrix.
            View = Matrix4.LookAt(
            new Vector3(0, 10, 15),  // camera position
            new Vector3(0, 0, 0),    // looking at origin
            Vector3.UnitY);          // up direction

            // Set safe defaults so Render has valid matrices even before Resize fires. The last floats control the clip plane distance.
            Projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                (float)GlControl.Width / GlControl.Height,
                0.1f, 5000f);
        }

        public void Render(bool ShowFloor, bool ShowJoints, bool ShowPolygons)
        {

            // Calculate delta time for frame-rate independent movement
            //var now = DateTime.Now;
            //float DeltaT = (float)(now - PrevFrameTime).TotalSeconds;
            //PrevFrameTime = now;

            double now = sClock.Elapsed.TotalSeconds;
            float DeltaTm = (float)(now - LastTime);
            LastTime = now;

            Cam.UpdateCameraPosition(KeyStateHandler.HeldKeys, DeltaTm);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //Draw Model Here!

            var view = Cam.GetViewMatrix();

            if (ShowFloor)
            {
                Floor.Render(view, Projection);
            }
            //
            GlControl.SwapBuffers();
        }

        //Zooms bsaed on Mouse Wheel.
        public void Zoom(int DeltaT)
        {
            float Normalizer = DeltaT / 120.0f;
            Cam.Zoom(Normalizer);
        }

        public void Resize(int Width, int Height)
        {
            //Also controls the clip plane distance.
            GL.Viewport(0, 0, Width, Height);
            Projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                (float)Width / Height, 0.1f, 5000f);
        }

        //Sets the Camera position and Rotation back to default.
        public void ResetCamera()
        {
            Cam.Position = new Vector3(0, 25, 15);
            Cam.Yaw = -90f;
            Cam.Pitch = -20f;
        }

        public void Closing()
        {
            try
            {
                GlControl.MakeCurrent();

                Floor.Dispose();
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffer(VertexBufferObject);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GL cleanup failed!\n Details: {ex.Message}");
            }
        }


    }
}
