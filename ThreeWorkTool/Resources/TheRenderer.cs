using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ThreeWorkTool.Resources.Geometry;
using ThreeWorkTool.Resources.Utility;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.ModelNodes;

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
        private List<ModelEntry> Models;
        private List<ModelBoneEntry> Joints;
        private List<ModelPrimitiveEntry> Polygons;
        private List<Sphere> BoneSpheres;
        private List<Matrix4> Matrices;
        private List<BoneRectangle> BoneRectangles;
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

            if (DraggingMode == DragMode.Rotate)
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

        public void Load(FrmModelViewer modelViewer)
        {
            //Starting Background Color. Uses the sky color from that one stage.
            GL.ClearColor(0.4706f, 0.5765f, 0.7608f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            Models = new List<ModelEntry>();
            BoneSpheres = new List<Sphere>();
            Matrices = new List<Matrix4>();
            BoneRectangles = new List<BoneRectangle>();
            Models.Add(modelViewer.modelEntry);
            Joints = modelViewer.Joints;
            Polygons = modelViewer.Polygons;

            //Setup Shaders and Buffers Here!
            //

            Cam = new CameraTake1();

            //Checkerboard data here!
            Floor = new Checkerboard
            {
                GridSize = 20,
                TileSize = 50.0f,
                ColorA = new Color4(0.7765f, 0.8588f, 1.0000f, 1.0f),
                ColorB = new Color4(0.3529f, 0.3882f, 0.4510f, 1.0f)
            };
            Floor.Load();

            //Bones. Need to calculate new matrices for child bones and stuff because of how Marvel 3 calculates bone matrices.
            for (int v = 0; v < modelViewer.Joints.Count; v++)
            {
                Matrix4x4 TargetMatrix = modelViewer.modelEntry.Bones[v].LocalMatrix;

                //Time to get the parent index.
                int ParentIndex = modelViewer.modelEntry.Bones[v].Parent;
                if (ParentIndex != 255 && ParentIndex != -1)
                {
                    TargetMatrix = TargetMatrix * modelViewer.modelEntry.Bones[ParentIndex].MatrixForViewer;
                    modelViewer.modelEntry.Bones[v].MatrixForViewer = TargetMatrix;

                    //For the Bone Lines.
                    BoneRectangle rectangle = new BoneRectangle
                    {
                        LineColor = new Color4(0.1f, 0.10f, 0.10f, 1.0f)
                    };
                    rectangle.StartPos = ByteUtilitarian.GetPosition(modelViewer.modelEntry.Bones[ParentIndex].MatrixForViewer);
                    rectangle.EndPos = ByteUtilitarian.GetPosition(modelViewer.modelEntry.Bones[v].MatrixForViewer);
                    BoneRectangles.Add(rectangle);
                    rectangle.Load();
                }
                else
                {
                    modelViewer.modelEntry.Bones[v].MatrixForViewer = TargetMatrix;
                }
                //MatrixForViewer

            }

            //Bones. Need the view matrix.
            for (int v = 0; v < Joints.Count; v++)
            {
                Matrix4 TKMatrix = ByteUtilitarian.FromSystemNumericsMatrixToOpenTKMatrix(Joints[v].MatrixForViewer);
                Sphere BoneSphere = new Sphere
                {
                    Color = new Color4(0.5f, 0.5f, 0.5f, 1.0f)
                };
                BoneSpheres.Add(BoneSphere);
                Matrices.Add(TKMatrix);
                BoneSphere.Load();
            }

            PrevFrameTime = DateTime.Now;

            //Sets up View Matrix.
            View = Matrix4.LookAt(
            new OpenTK.Vector3(0, 10, 15),  // camera position
            new OpenTK.Vector3(0, 0, 0),    // looking at origin
            OpenTK.Vector3.UnitY);          // up direction

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

            if (ShowJoints)
            {
                for (int v = 0; v < BoneSpheres.Count; v++)
                {
                    BoneSpheres[v].Render(Matrices[v], view, Projection);

                    //
                }

                for (int w = 0; w < BoneRectangles.Count; w++)
                {
                    BoneRectangles[w].Render(BoneRectangles[w].StartPos, BoneRectangles[w].EndPos, Cam.Position, view, Projection);
                }
            }
            ////Bones. Need the view matrix.
            //for (int v = 0; v < Joints.Count; v++)
            //{
            //    Matrix4 TKMatrix = ByteUtilitarian.FromSystemNumericsMatrixToOpenTKMatrix(Joints[v].LocalMatrix);
            //    Sphere BoneSphere = new Sphere();
            //    //BoneSpheres.Add(BoneSphere);
            //    BoneSphere.Render(TKMatrix, View, Projection);

            //}


            //foreach (Sphere bone in BoneSpheres)
            //{
            //    bone.Render(TKMatrix, View, Projection);
            //}

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
            Cam.Position = new OpenTK.Vector3(0, 25, 15);
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GL cleanup failed!\n Details: {ex.Message}");
            }
        }


    }
}
