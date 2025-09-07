using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
//using ThreeWorkTool.Resources.Geometry;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using ThreeWorkTool.Resources.Wrappers;
using SharpDX.D3DCompiler;
using SharpDX.Mathematics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using ThreeWorkTool.Resources.Geometry;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;

namespace ThreeWorkTool
{

    public partial class ModelViewer : Form
    {
        public FrmMainThree Mainfrm { get; set; }
        private SharpDX.Color BGColor { get; set; }
        private static ThreeSourceTree treeview;
        private static TreeNode node_;
        private Buffer vertBuffer;
        private int vertCount;
        private CameraTake1 Camera = new CameraTake1();
        private HashSet<Keys> KeysDown = new HashSet<Keys>();
        private System.Drawing.Point lastMousePos;
        private bool rotating = false;
        private bool panning = false;
        //private System.Windows.Forms.Panel panelRender;
        Color4 RealBGColor = new Color4(0.825f, 0.95f, 0.95f, 1.0f);
        SharpDX.Direct3D11.Device device;
        SwapChain swapChain;
        SharpDX.Direct3D11.DeviceContext context;
        RenderTargetView renderTargetView;
        DepthStencilView depthStencilView = null;
        DepthStencilState depthStencilState = null;
        RasterizerState rasterState = null;

        public ModelViewer()
        {
            InitializeComponent();
            this.Load += ModelViewer_Load;
            this.FormClosing += ModelViewer_FormClosing;

            //        treeview = Mainfrm.TreeSource;
            //        node_ = treeview.SelectedNode;
            //        Device device;
            //        SwapChain swapChain;
            //        this.Text = "SharpDX D3D11 Model Viewer - Under Construction - " + node_.Text;
            //        BGColor = SharpDX.Color.LightCyan;

        }

        private void ModelViewer_Load(object sender, EventArgs e)
        {
            //Initializes D3D Layer.
            InitialzeD3DDevice();



            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            this.Text = "SharpDX D3D11 Model Viewer - Under Construction - " + node_.Text;
            BGColor = SharpDX.Color.LightCyan;

            this.KeyPreview = true;
            this.KeyDown += (s, ex) => KeysDown.Add(ex.KeyCode);

            this.KeyUp += (s, ex) => KeysDown.Remove(ex.KeyCode);

            ////20x20 Checkboard.
            TheCheckerboardFloor(2000, 2000, 1.0f);

            Application.Idle += RenderLoop;

        }

        private void ModelViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Hide();
            Application.Idle -= RenderLoop;

            renderTargetView?.Dispose();
            swapChain?.Dispose();
            device?.Dispose();
            context?.Dispose();
        }

        private void InitialzeD3DDevice()
        {
            //Time to setup the Swapchain Description Structure.
            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(
                    this.pnl3DView.Width, this.pnl3DView.Height,
                    new Rational(60, 1), Format.R8G8B8A8_UNorm),
                Usage = Usage.RenderTargetOutput,
                OutputHandle = this.pnl3DView.Handle,
                SampleDescription = new SampleDescription(1, 0),
                IsWindowed = true,
                SwapEffect = SwapEffect.Discard,
                Flags = SwapChainFlags.None
            };

            Device.CreateWithSwapChain(
                SharpDX.Direct3D.DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                swapChainDesc,
                out device,
                out swapChain);

            context = device.ImmediateContext;

            using (var backBuffer = swapChain.GetBackBuffer<Texture2D>(0))
            {
                renderTargetView = new RenderTargetView(device, backBuffer);
            }



        }

        private bool AppStillIdle
        {
            get
            {
                NativeMethods.PeekMessage(out var msg, IntPtr.Zero, 0, 0, 0);
                return msg.message == 0;
            }
        }

        //void InitializeTextOverlay()
        //{
        //    dwFactory = new SharpDX.DirectWrite.Factory();
        //    d2dFactory = new SharpDX.Direct2D1.Factory();

        //    // Get the back buffer surface
        //    using (var dxgiBackBuffer = swapChain.GetBackBuffer<Surface>(0))
        //    {
        //        var renderTargetProperties = new RenderTargetProperties(
        //            new PixelFormat(Format.Unknown, AlphaMode.Premultiplied));

        //        d2dRenderTarget = new WindowRenderTarget(d2dFactory,
        //            new HwndRenderTargetProperties()
        //            {
        //                Hwnd = panelRender.Handle,
        //                PixelSize = new Size2(panelRender.Width, panelRender.Height),
        //                PresentOptions = PresentOptions.Immediately
        //            },
        //            renderTargetProperties);
        //    }

        //    textBrush = new SolidColorBrush(d2dRenderTarget, new RawColor4(1f, 1f, 1f, 1f)); // White
        //    textFormat = new TextFormat(dwFactory, "Segoe UI", FontWeight.Normal, FontStyle.Normal, 16);
        //}

        private void RenderLoop(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                Render();
            }
        }

        private void Render()
        {
            context.OutputMerger.SetRenderTargets(renderTargetView);
            context.ClearRenderTargetView(renderTargetView, RealBGColor);

            //Controls.
            float deltaTime = 1f / 60f; // estimate or use Stopwatch

            Vector3 movement = Vector3.Zero;
            if (KeysDown.Contains(Keys.W)) movement.Z += 1;
            if (KeysDown.Contains(Keys.S)) movement.Z -= 1;
            if (KeysDown.Contains(Keys.A)) movement.X -= 1;
            if (KeysDown.Contains(Keys.D)) movement.X += 1;
            if (KeysDown.Contains(Keys.E)) movement.Y += 1;
            if (KeysDown.Contains(Keys.Q)) movement.Y -= 1;

            if (movement != Vector3.Zero)
                Camera.Move(movement, deltaTime);

            Vector3 camPos = Camera.Position;
            lblCameraCoords.Text = $"Camera:\n X={camPos.X:F2}, Y={camPos.Y:F2}, Z={camPos.Z:F2}"+ "\n" + $"Yaw: {Camera.Yaw * 180 / Math.PI:F1}°\n" + $"Pitch: {Camera.Pitch * 180 / Math.PI:F1}°";

            // This is where Model Drawing Code should go.
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertBuffer, Utilities.SizeOf<Vertex>(), 0));

            // This is where Shaders and constant buffers should be set.
            //From mohammed sameeh and Vimal CK.
            string workingDirectory = Environment.CurrentDirectory;
            string CurrentPath = Directory.GetParent(workingDirectory).Parent.FullName;

            CurrentPath = CurrentPath + "\\ShaderBase.hlsl";

            var vsBytecode = ShaderBytecode.CompileFromFile(CurrentPath, "VS", "vs_4_0");
            var psBytecode = ShaderBytecode.CompileFromFile(CurrentPath, "PS", "ps_4_0");

            var vertexShader = new VertexShader(device, vsBytecode);
            var pixelShader = new PixelShader(device, psBytecode);

            var layout = new InputLayout(device, ShaderSignature.GetInputSignature(vsBytecode), new[]
            {
                new SharpDX.Direct3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new SharpDX.Direct3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),
            });

            context.InputAssembler.InputLayout = layout;
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            var matrixBuffer = new Buffer(device, Utilities.SizeOf<MatrixBuffer>(),
                ResourceUsage.Default, BindFlags.ConstantBuffer,
                CpuAccessFlags.None, ResourceOptionFlags.None, 0);


            float aspectRatio = (float)pnl3DView.Width / (float)pnl3DView.Height;

            // Set once (or update per frame)
            MatrixBuffer matrices = new MatrixBuffer
            {
                World = Matrix.Identity,
                View = Camera.View,
                Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4f, aspectRatio, 0.1f, 100f)
            };

            // Transpose (HLSL expects column-major by default)
            matrices.World.Transpose();
            matrices.View.Transpose();
            matrices.Projection.Transpose();

            context.UpdateSubresource(ref matrices, matrixBuffer);
            context.VertexShader.SetConstantBuffer(0, matrixBuffer);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertBuffer, Utilities.SizeOf<Vertex>(), 0));
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.InputAssembler.InputLayout = layout;
            context.VertexShader.SetConstantBuffer(0, matrixBuffer);

            context.Draw(vertCount, 0);

            swapChain.Present(1, PresentFlags.None);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Changes the background color.
        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                RealBGColor = new Color4((colorDlg.Color.R / 255f), (colorDlg.Color.G / 255f), (colorDlg.Color.B / 255f), 1.0f);
            }


        }

        private void ModelViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            FrmMainThree.ModelViewerClosed(sender, e);
        }

        private void ModelViewer_Resize(object sender, EventArgs e)
        {

        }

        private void ModelViewer_SizeChanged(object sender, EventArgs e)
        {

        }

        private void TheCheckerboardFloor(int XTiles, int ZTiles, float TileSize)
        {
            List<Vertex> vertices = new List<Vertex>();

            for (int x = 0; x < XTiles; x++)
            {
                for (int z = 0; z < ZTiles; z++)
                {
                    // Calculate quad origin
                    float originX = x * TileSize;
                    float originZ = z * TileSize;

                    // Alternate color (like a chessboard)
                    bool isBlack = (x + z) % 2 == 0;
                    Color4 tileColor = isBlack ? Color4.Black : Color4.White;

                    // Each quad = 2 triangles = 6 vertices (triangle list)
                    Vector3 v0 = new Vector3(originX, 0, originZ);
                    Vector3 v1 = new Vector3(originX + TileSize, 0, originZ);
                    Vector3 v2 = new Vector3(originX, 0, originZ + TileSize);
                    Vector3 v3 = new Vector3(originX + TileSize, 0, originZ + TileSize);

                    // Triangle 1
                    vertices.Add(new Vertex(v0, tileColor));
                    vertices.Add(new Vertex(v2, tileColor));
                    vertices.Add(new Vertex(v1, tileColor));

                    // Triangle 2
                    vertices.Add(new Vertex(v1, tileColor));
                    vertices.Add(new Vertex(v2, tileColor));
                    vertices.Add(new Vertex(v3, tileColor));
                }
            }

            vertCount = vertices.Count;

            // Create vertex buffer
            vertBuffer?.Dispose();
            vertBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices.ToArray());
        }

        private void pnl3DView_Resize(object sender, EventArgs e)
        {

        }

        private void pnl3DView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                rotating = true;
            if (e.Button == MouseButtons.Right)
                panning = true;

            lastMousePos = e.Location;
        }

        private void pnl3DView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                rotating = false;
            if (e.Button == MouseButtons.Right)
                panning = false;
        }

        private void pnl3DView_MouseMove(object sender, MouseEventArgs e)
        {
            int dx = e.X - lastMousePos.X;
            int dy = e.Y - lastMousePos.Y;

            if (rotating)
                Camera.Rotate(dx, -dy);
            else if (panning)
                Camera.Pan(dx, -dy);

            lastMousePos = e.Location;
        }

        private void pnl3DView_MouseWheel(object sender, MouseEventArgs e)
        {
            Camera.Zoom(e.Delta / 120f); // 120 is one scroll tick?
        }

        private void ModelViewer_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void ModelViewer_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Message
        {
            public IntPtr hWnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool PeekMessage(out Message lpMsg, IntPtr hWnd,
            uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;
        public Color4 Color;

        public Vertex(Vector3 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MatrixBuffer
    {
        public Matrix World;
        public Matrix View;
        public Matrix Projection;
    }

}