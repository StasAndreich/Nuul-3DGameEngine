using NuulEngine.Graphics.Infrastructure;
using SharpDX.Windows;
using System;

namespace NuulEngine.Graphics
{
    internal sealed class prototypeRenderer
    {
        private RenderForm _renderForm;

        // Not init.
        private Camera _camera;

        private DirectX3DGraphics _directX3DGraphics;

        //private DirectX2DGraphics _directX2DGraphics;

        private GraphicsRenderer _graphicsRenderer;

        public event EventHandler SwapChainResized;

        public event EventHandler SwapChainResizing;

        public prototypeRenderer(RenderForm renderForm)
        {
            _renderForm = renderForm;
            _renderForm.UserResized += OnRenderFormResized;
            _directX3DGraphics = new DirectX3DGraphics(_renderForm);
            _graphicsRenderer = new GraphicsRenderer(_directX3DGraphics);
        }

        public void RunRenderLoop(RenderLoop.RenderCallback renderCallback)
        {
            RenderLoop.Run(_renderForm, renderCallback);
        }

        public void RenderScene()
        {

        }

        private void OnRenderFormResized(object sender, EventArgs args)
        {
            SwapChainResizing?.Invoke(this, null);
            _directX3DGraphics.Resize();
            _camera.AspectRatio = _renderForm.ClientSize.Width
                / (float)_renderForm.ClientSize.Height;
            SwapChainResized?.Invoke(this, null);
        }
    }
}
