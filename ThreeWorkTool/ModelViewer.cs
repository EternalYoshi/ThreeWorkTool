using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Windows;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

using Device = SharpDX.Direct3D11.Device;
using System.Windows.Forms;

namespace ThreeWorkTool
{
    public class ModelViewer : Form
    {
        public FrmMainThree Mainfrm { get; set; }

        [STAThread]
        public static void MainForm(string[] args)
        {
            var renderForm = new RenderForm("Model Viewer - Under Construction");
            renderForm.Width = 1280;
            renderForm.Height = 720;

            Device device;
            SwapChain swapChain;

            var swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(
                renderForm.Width,
                renderForm.Height,
                new Rational(60, 1),
                Format.R8G8B8A8_UNorm),
                IsWindowed = !renderForm.IsFullscreen,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.None,
                swapChainDesc,
                out device,
                out swapChain);

            var backBuffer = Texture2D.FromSwapChain<SharpDX.Direct3D11.Texture2D>(swapChain, 0);
            var renderView = new RenderTargetView(device, backBuffer);

            var context = device.ImmediateContext;

            context.OutputMerger.SetRenderTargets(renderView);
            context.Rasterizer.SetViewport(
               new Viewport(0, 0, renderForm.Width, renderForm.Height));

            RenderLoop.Run(renderForm, ()=>
            {

                // When we render a frame, we have to clear it first
                context.ClearRenderTargetView(renderView, Color.Cyan);

                // Render anything here

                // Present the rendering on screen
                swapChain.Present(0, 0);

            });

            renderView.Dispose();
            backBuffer.Dispose();
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();


        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ModelViewer
            // 
            this.ClientSize = new System.Drawing.Size(1264, 681);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ModelViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}
