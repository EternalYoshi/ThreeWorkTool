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

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using ThreeWorkTool.Resources.Wrappers;

namespace ThreeWorkTool
{
    public partial class ModelViewer : RenderForm
    {
        public FrmMainThree Mainfrm { get; set; }
        private SharpDX.Color BGColor { get; set; }
        private static ThreeSourceTree treeview;
        private static TreeNode node_;
        SwapChain swapChain = null;
        Texture2D backBuffer = null;
        Texture2D depthStencil = null;
        RenderTargetView renderView = null;
        DepthStencilView DStencilView = null;
        DepthStencilState DStencilState = null;
        RasterizerState rasterState = null;
        public ModelEntry model;
        string modeldata;

        public ModelViewer()
        {
            InitializeComponent();
        }

        //Camera stuff.
        //Camera camera = null;
        
        public void ShowMV(TreeNode node)
        {
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            Device device;
            SwapChain swapChain;

            this.Text = "SharpDX D3D11 Model Viewer - Under Construction - " + node_.Text;
            BGColor = SharpDX.Color.LightCyan;
            // Creates the device and swapchain. From the packtub site tutorial.
            Device.CreateWithSwapChain(
                SharpDX.Direct3D.DriverType.Hardware,
                DeviceCreationFlags.None,
                new[] {
            SharpDX.Direct3D.FeatureLevel.Level_11_1,
            SharpDX.Direct3D.FeatureLevel.Level_11_0,
            SharpDX.Direct3D.FeatureLevel.Level_10_1,
            SharpDX.Direct3D.FeatureLevel.Level_10_0,
                },
                new SwapChainDescription()
                {
                    ModeDescription =
                        new ModeDescription(
                            this.ClientSize.Width,
                            this.ClientSize.Height,
                            new Rational(60, 1),
                            Format.R8G8B8A8_UNorm
                        ),
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = SharpDX.DXGI.Usage.BackBuffer | Usage.RenderTargetOutput,
                    BufferCount = 1,
                    Flags = SwapChainFlags.None,
                    IsWindowed = true,
                    OutputHandle = this.Handle,
                    SwapEffect = SwapEffect.Discard,
                },
                out device, out swapChain
            );

            var backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            var renderTargetView = new RenderTargetView(device, backBuffer);


            /*
            device.DebugName = "The Device";
            swapChain.DebugName = "The SwapChain";
            backBuffer.DebugName = "The Backbuffer";
            renderTargetView.DebugName = "The RenderTargetView";
            */




            // Create and run the render loop
            RenderLoop.Run(this, () =>
            {
                // Clear the render target with light blue
                device.ImmediateContext.ClearRenderTargetView(
                  renderTargetView,
                  BGColor);
                // Execute rendering commands here...

                // Present the frame
                swapChain.Present(0, PresentFlags.None);
            });
            
            // Release the device and any other resources created
            renderTargetView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            swapChain.Dispose();


        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void backgroundColorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                //BGColor = 

            }


        }

        private void ModelViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Hide();
        }

        private void ModelViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            FrmMainThree.ModelViewerClosed(sender,e);
        }

        private void ModelViewer_Resize(object sender, EventArgs e)
        {

        }

        private void ModelViewer_SizeChanged(object sender, EventArgs e)
        {

        }


    }
}
